using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;
using static ViCommon.Functional.Monads.MaybeMonad.Maybe;
using static ViCommon.Functional.Monads.ResultMonad.Result;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.Models
{
    /// <summary>
    /// Represents a configuration member path.
    /// </summary>
    public class RoslynMemberPath
    {
        #region fields

        private readonly ImmutableArray<IPathSegment> _items;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynMemberPath"/> class.
        /// </summary>
        /// <param name="items"></param>
        public RoslynMemberPath(ImmutableArray<IPathSegment> items)
        {
            this._items = items;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the number of segments.
        /// </summary>
        public int Count => this._items.Length;

        /// <summary>
        /// Gets the last segment.
        /// </summary>
        public IPathSegment Last => this._items.Last();

        /// <summary>
        /// Gets the segment at position i.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>The <see cref="IPathSegment"/>.</returns>
        public IPathSegment this[int i] => this._items[i];

        #endregion

        #region members

        /// <summary>
        /// Create a new <see cref="RoslynMemberPath"/>.
        /// </summary>
        /// <param name="identifiers"></param>
        /// <param name="context"></param>
        /// <returns>Returns a failure when something went wng else a success.</returns>
        public static IResult<RoslynMemberPath, Failure> Create(
            IReadOnlyList<IdentifierNameSyntax> identifiers,
            IRoslynContext context)
        {
            var currentItem = None<IPathSegment>();
            var builder = ImmutableArray.CreateBuilder<IPathSegment>();

            foreach (var identifier in identifiers)
            {
                IPathSegment segment = null;

                if (identifier.Identifier.Text == "Constructor")
                {
                    segment = new CtorPathSegment(identifier.Identifier.Text, currentItem, identifier);
                }
                else
                {
                    var itemResult = CreatePathItem(identifier, context, currentItem);

                    if (itemResult.AsResultValue() is FailureValue<Failure> failure)
                    {
                        return Failure<RoslynMemberPath, Failure>(failure);
                    }

                    segment = itemResult.GetSuccessUnsafe();
                }

                builder.Add(segment);
                currentItem = Some(segment);
            }

            return Success<RoslynMemberPath, Failure>(new RoslynMemberPath(builder.ToImmutable()));
        }

        private static IResult<MemberPathSegment, Failure> CreatePathItem(
            IdentifierNameSyntax identifier,
            IRoslynContext context,
            Maybe<IPathSegment> parent) =>
            GetPathSymbolAndReturnSymbol(context, identifier)
                .MapSuccess(
                    tuple => new MemberPathSegment(
                        identifier.Identifier.Text,
                        parent,
                        tuple.PathSymbol,
                        tuple.PathReturnSymbol,
                        identifier));

        private static IResult<(INamedTypeSymbol PathSymbol, ITypeSymbol PathReturnSymbol), Failure> GetPathSymbolAndReturnSymbol(
                IRoslynContext context,
                SimpleNameSyntax identifierNameSyntax) =>
            ToMaybe(context.SemanticModel.GetSymbolInfo(identifierNameSyntax).Symbol)
                .ToResult(
                    new Failure(
                        $"Cannot resolve the type form the path segment {identifierNameSyntax.Identifier.Text}"))
                .Bind(GetPathType)
                .Bind(
                    symbol => ToMaybe(symbol as INamedTypeSymbol)
                        .ToResult(new Failure($"{symbol} cannot be converted to {nameof(INamedTypeSymbol)}")))
                .Bind(
                    symbol => GetPathReturnType(symbol)
                        .MapSuccess(typeSymbol => (symbol, typeSymbol)));

        private static IResult<ITypeSymbol, Failure> GetPathType(ISymbol symbol) =>
            symbol switch
            {
                IFieldSymbol fieldSymbol =>
                    Success<ITypeSymbol, Failure>(fieldSymbol.Type),

                IPropertySymbol propertySymbol =>
                    Success<ITypeSymbol, Failure>(propertySymbol.Type),

                _ => Failure<ITypeSymbol, Failure>(new Failure("The Path symbol is not a property or a field.")),
            };

        // The Property return type is IFixtureItemPath<TRoot> but we need the one with two type arguments.
        // So we need to get the get method of the property which will return FixtureItemPath<TRoot, TReturnType>
        private static IResult<ITypeSymbol, Failure> GetPathReturnType(INamedTypeSymbol symbol)
        {
            if (symbol.TypeArguments.Length != 2)
            {
                var maybeSymbol = symbol.AllInterfaces
                    .FirstOrNone(
                        namedTypeSymbol => namedTypeSymbol.Name.Contains("nameof(IMemberPath<int, int>)") &&
                                           namedTypeSymbol.TypeArguments.Length == 2)
                    .Map(typeSymbol => typeSymbol.TypeArguments[1]);

                return maybeSymbol.ToResult(
                    new Failure($"Cannot find the interface IMemberPath for the type {symbol.ToDisplayParts()}"));
            }
            else
            {
                return Success<ITypeSymbol, Failure>(symbol.TypeArguments[1]);
            }
        }

        #endregion
    }
}