using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <inheritdoc cref="IBuilderInformation" />
    public record BuilderInformation : IBuilderInformation
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderInformation"/> class.
        /// </summary>
        /// <param name="configClassSymbol"></param>
        /// <param name="itemConfigSymbol"></param>
        /// <param name="classDeclarationSyntax"></param>
        /// <param name="objectCreationExpression"></param>
        public BuilderInformation(
            ITypeSymbol configClassSymbol,
            INamedTypeSymbol itemConfigSymbol,
            ClassDeclarationSyntax classDeclarationSyntax,
            ObjectCreationExpressionSyntax objectCreationExpression)
        {
            this.CustomItemBuilderSymbol = configClassSymbol;
            this.ItemBuilderSymbol = itemConfigSymbol;
            this.ClassDeclarationSyntax = classDeclarationSyntax;
            this.ObjectCreationExpression = objectCreationExpression;
            this.FixtureItemSymbol = itemConfigSymbol.TypeArguments.FirstOrNone()
                .SomeOrProvided(() => throw new InternalException("Cannot find the fixture item type."));
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ITypeSymbol CustomItemBuilderSymbol { get; init; }

        /// <inheritdoc />
        public INamedTypeSymbol ItemBuilderSymbol { get; init; }

        /// <inheritdoc />
        public ITypeSymbol FixtureItemSymbol { get; init; }

        /// <inheritdoc />
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; init; }

        /// <inheritdoc />
        public ObjectCreationExpressionSyntax ObjectCreationExpression { get; init; }

        #endregion
    }
}