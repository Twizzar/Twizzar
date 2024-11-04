using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;

using Twizzar.Analyzer.Core.Interfaces;
using Twizzar.Analyzer2022.App.Interfaces;
using Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Analyzer.Core.SourceTextGenerators
{
    /// <inheritdoc cref="IPathSourceTextMemberGenerator" />
    public partial class PathSourceTextMemberGenerator : IPathSourceTextMemberGenerator
    {
        #region fields

        private readonly ICtorSelector _ctorSelector;
        private readonly IRoslynDescriptionFactory _descriptionFactory;
        private readonly ISymbolService _symbolService;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PathSourceTextMemberGenerator"/> class.
        /// </summary>
        /// <param name="ctorSelector"></param>
        /// <param name="descriptionFactory"></param>
        /// <param name="symbolService"></param>
        public PathSourceTextMemberGenerator(
            ICtorSelector ctorSelector,
            IRoslynDescriptionFactory descriptionFactory,
            ISymbolService symbolService)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(ctorSelector, nameof(ctorSelector))
                .Parameter(descriptionFactory, nameof(descriptionFactory))
                .Parameter(symbolService, nameof(symbolService))
                .ThrowWhenNull();

            this._ctorSelector = ctorSelector;
            this._descriptionFactory = descriptionFactory;
            this._symbolService = symbolService;
        }

        #endregion

        #region members

        /// <inheritdoc/>
        public string GenerateMembers(
            ITypeDescription typeDescription,
            Maybe<IPathNode> parent,
            string fixtureItemTypeName,
            HashSet<string> usingStatements,
            HashSet<string> reservedMembers,
            bool generateFuturePaths,
            Compilation compilation,
            ISymbol sourceType,
            in List<MemberVerificationInfo> membersForVerification,
            string declaredType,
            bool isRoot = true,
            CancellationToken cancellationToken = default)
        {
            var sb = new StringBuilder();
            var parentInstance = isRoot ? "RootPath" : "this";

            var builder = new MemberBuilder(
                this,
                this._descriptionFactory,
                usingStatements,
                new HashSet<string>(reservedMembers),
                fixtureItemTypeName,
                parent,
                generateFuturePaths,
                compilation,
                sourceType,
                parentInstance,
                declaredType,
                membersForVerification,
                cancellationToken);

            bool IsMemberAccessible(IBaseDescription memberDescription) =>
                this._symbolService.IsSymbolAccessibleWithin(compilation, memberDescription, sourceType);

            foreach (var propertyDescription in typeDescription.GetDeclaredProperties()
                         .Where(IsMemberAccessible)
                         .OrderBy(description => description.Name))
            {
                sb.Append(builder.GenerateMembers(propertyDescription));
            }

            foreach (var fieldDescription in typeDescription.GetDeclaredFields()
                         .Where(IsMemberAccessible)
                         .OrderBy(description => description.Name))
            {
                sb.Append(builder.GenerateMembers(fieldDescription));
            }

            foreach (var methodDescription in typeDescription
                         .GetDeclaredTwizzarSelectableMethod()
                         .Where(IsMemberAccessible)
                         .OrderBy(description => description.DeclaredParameters.Length))
            {
                sb.Append(builder.GenerateMembers(methodDescription));
            }

            if (!typeDescription.IsBaseType && !typeDescription.IsAbstract && !typeDescription.IsInterface)
            {
                var selectedCtor = this._ctorSelector
                    .GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

                selectedCtor.IfSuccess(
                    description =>
                    {
                        if (description.DeclaredParameters.Length <= 0 || !IsMemberAccessible(description))
                        {
                            return;
                        }

                        var ctorParamBody = new StringBuilder();
                        const string ctorPropertyName = "Ctor";
                        var ctorParent = parent.Bind(node => node.Children.GetMaybe(ctorPropertyName));
                        var ctorBuilder = builder with
                        {
                            Parent = ctorParent, ReservedMembers = new HashSet<string>(reservedMembers),
                            DeclaredType = $"{builder.DeclaredType}.ConstructorMemberPath",
                        };

                        foreach (var parameterDescription in description.DeclaredParameters
                                     .Where(parameterDescription =>
                                         IsMemberAccessible(parameterDescription.GetReturnTypeDescription())))
                        {
                            ctorParamBody.Append(
                                ctorBuilder.GenerateMembers(parameterDescription));
                        }

                        sb.Append(
                            $@"
public ConstructorMemberPath {ctorPropertyName} => new ConstructorMemberPath({parentInstance});

public class ConstructorMemberPath
{{
    private MemberPath<{fixtureItemTypeName}> TzParent;

    public ConstructorMemberPath(MemberPath<{fixtureItemTypeName}> parent)
    {{
        this.TzParent = parent;
    }}

    {ctorParamBody}
}}");
                    });
            }

            return sb.ToString();
        }

        #endregion
    }
}