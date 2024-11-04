using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Config;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces.Roslyn;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure.Extension;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.ConfigWriter
{
    /// <inheritdoc cref="IRoslynConfigWriter" />
    public class RoslynConfigWriter : IRoslynConfigWriter
    {
        #region fields

        private readonly IBuildInvocationSpanQuery _buildInvocationSpanQuery;
        private readonly IRoslynContextQuery _roslynContextQuery;
        private readonly IDocumentFileNameQuery _documentFileNameQuery;
        private readonly IRoslynConfigFinder _roslynConfigFinder;
        private readonly IDiscoverer _discoverer;
        private readonly ITypeSymbolQuery _typeSymbolQuery;
        private readonly Workspace _workspace;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynConfigWriter"/> class.
        /// </summary>
        /// <param name="buildInvocationSpanQuery"></param>
        /// <param name="roslynContextQuery"></param>
        /// <param name="documentFileNameQuery"></param>
        /// <param name="roslynConfigFinder"></param>
        /// <param name="workspace"></param>
        /// <param name="discoverer"></param>
        /// <param name="typeSymbolQuery"></param>
        public RoslynConfigWriter(
            IBuildInvocationSpanQuery buildInvocationSpanQuery,
            IRoslynContextQuery roslynContextQuery,
            IDocumentFileNameQuery documentFileNameQuery,
            IRoslynConfigFinder roslynConfigFinder,
            Workspace workspace,
            IDiscoverer discoverer,
            ITypeSymbolQuery typeSymbolQuery)
        {
            this.EnsureMany()
                .Parameter(buildInvocationSpanQuery, nameof(buildInvocationSpanQuery))
                .Parameter(roslynContextQuery, nameof(roslynContextQuery))
                .Parameter(documentFileNameQuery, nameof(documentFileNameQuery))
                .Parameter(roslynConfigFinder, nameof(roslynConfigFinder))
                .Parameter(workspace, nameof(workspace))
                .Parameter(discoverer, nameof(discoverer))
                .Parameter(typeSymbolQuery, nameof(typeSymbolQuery))
                .ThrowWhenNull();

            this._buildInvocationSpanQuery = buildInvocationSpanQuery;
            this._roslynContextQuery = roslynContextQuery;
            this._documentFileNameQuery = documentFileNameQuery;
            this._roslynConfigFinder = roslynConfigFinder;
            this._workspace = workspace;
            this._discoverer = discoverer;
            this._typeSymbolQuery = typeSymbolQuery;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async Task UpdateConfigAsync(FixtureItemId id, IMemberConfiguration memberConfiguration)
        {
            using var operation =
                ViMonitor.StartOperation($"{nameof(RoslynConfigWriter)}.{nameof(this.UpdateConfigAsync)}");

            operation.Telemetry.Properties["FixtureItemIdHash"] = id.GetHashCode().ToString();
            operation.Telemetry.Properties["memberName"] = memberConfiguration.Name;
            operation.Telemetry.Properties["memberConfigurationType"] = memberConfiguration.GetType().Name;

            await this.GetContextAsync(id.RootItemPath)
                .MapSuccessAsync(context => (context, GetOrCreateCtor(context)))
                .DoAsync(
                    t => this.AddMemberConfigAsync(t.Item2, t.context, memberConfiguration, id),
                    failure => Task.Run(() => this.Log(failure.Message, LogLevel.Error)));
        }

        private async Task AddMemberConfigAsync(
            ConstructorDeclarationSyntax ctorNode,
            Context context,
            IMemberConfiguration memberConfiguration,
            FixtureItemId id)
        {
            var ctorUpdater = new CtorUpdater(ctorNode, this._discoverer, context);

            ctorUpdater.Update(this.CreateStatements(context, memberConfiguration, id));

            if (memberConfiguration is UndefinedMemberConfiguration)
            {
                var rootPath = memberConfiguration.Name.ToSourceVariableCodeFriendly() + ".";
                ctorUpdater.RemovePathWithChildren(rootPath);
            }

            context.CurrentConfigNode = context.CurrentConfigNode.ReplaceNode(ctorNode, ctorUpdater.UpdatedCtorNode);
            await context.SyncAsync(this._workspace);
        }

        private IEnumerable<IConfigurationMemberEdit> CreateStatements(
            Context context,
            IMemberConfiguration memberConfiguration,
            FixtureItemId id)
        {
            var rootItemPath = id.RootItemPath
                .SomeOrProvided(
                    () => throw new InternalException($"{nameof(FixtureItemId)}.{nameof(id.RootItemPath)} is none"));

            var rootPath =
                id.Name
                    .SomeOrProvided(
                        () => throw new InternalException($"{nameof(FixtureItemId)}.{nameof(id.Name)} is none"))
                    .Replace($"{rootItemPath}", string.Empty);

            if (!string.IsNullOrEmpty(rootPath))
            {
                rootPath = rootPath.Remove(0, 1); // remote the . at the start.
                rootPath += ".";
            }

            return this._typeSymbolQuery.GetTypeSymbol(context.Compilation, id.TypeFullName)
                .Match(
                    typeSymbol => memberConfiguration switch
                    {
                        CtorMemberConfiguration ctorMemberConfiguration => this.CreateCtorStatements(
                            ctorMemberConfiguration,
                            rootPath,
                            context),
                        MethodConfiguration methodConfiguration => this.CreateMethodStatements(
                            methodConfiguration,
                            typeSymbol,
                            rootPath,
                            context),
                        _ => this.CreateSimpleStatements(memberConfiguration, rootPath, context),
                    },
                    failure =>
                    {
                        this.Log(failure.Message, LogLevel.Error);
                        return Enumerable.Empty<IConfigurationMemberEdit>();
                    });
        }

        private IEnumerable<IConfigurationMemberEdit> CreateMethodStatements(
            MethodConfiguration methodConfiguration,
            ITypeSymbol typeSymbol,
            string rootPath,
            Context context)
        {
            var memberSymbol = typeSymbol.GetAllMembers(methodConfiguration.MethodName)
                .OfType<IMethodSymbol>()
                .FirstOrNone(symbol => symbol.Parameters.Select(parameterSymbol => parameterSymbol.Type.MetadataName).SequenceEqual(methodConfiguration.ParameterTypes));

            if (memberSymbol.IsNone)
            {
                this.Log(
                    $"Cannot find method with the name {methodConfiguration.MethodName} and the parameters type {methodConfiguration.ParameterTypes.ToCommaSeparated()}",
                    LogLevel.Error);

                yield break;
            }

            var path =
                $"{rootPath}{methodConfiguration.Name.ToSourceVariableCodeFriendly()}";

            if (methodConfiguration.ReturnValue.Source is FromSystemDefault)
            {
                yield return new RemoveConfigurationMemberEdit(path);

                yield break;
            }

            switch (this.CreateStatement(methodConfiguration.ReturnValue, path, context)
                        .AsResultValue())
            {
                case SuccessValue<StatementSyntax> successValue:
                    yield return new AddConfigurationMemberEdit(successValue, path);

                    break;
                case FailureValue<Failure> failureValue:
                    this.Log(failureValue.Value.Message, LogLevel.Error);
                    break;
            }
        }

        private IEnumerable<IConfigurationMemberEdit> CreateCtorStatements(
            CtorMemberConfiguration ctorMemberConfiguration,
            string rootPath,
            Context context)
        {
            foreach (var configuration in ctorMemberConfiguration.ConstructorParameters.Values)
            {
                var path = $"{rootPath}Ctor.{configuration.Name.ToSourceVariableCodeFriendly()}";

                if (configuration.Source is FromSystemDefault)
                {
                    yield return new RemoveConfigurationMemberEdit(path);
                }
                else
                {
                    switch (this.CreateStatement(configuration, path, context).AsResultValue())
                    {
                        case SuccessValue<StatementSyntax> successValue:
                            yield return new AddConfigurationMemberEdit(successValue, path);

                            break;
                        case FailureValue<Failure> failureValue:
                            this.Log(failureValue.Value.Message, LogLevel.Error);
                            break;
                    }
                }
            }
        }

        private IEnumerable<IConfigurationMemberEdit> CreateSimpleStatements(
            IMemberConfiguration memberConfiguration,
            string rootPath,
            Context context)
        {
            var path =
                $"{rootPath}{memberConfiguration.Name.ToSourceVariableCodeFriendly()}";

            if (memberConfiguration.Source is FromSystemDefault)
            {
                yield return new RemoveConfigurationMemberEdit(path);
            }
            else
            {
                switch (this.CreateStatement(memberConfiguration, path, context).AsResultValue())
                {
                    case SuccessValue<StatementSyntax> successValue:
                        yield return new AddConfigurationMemberEdit(successValue, path);

                        break;
                    case FailureValue<Failure> failureValue:
                        this.Log(failureValue.Value.Message, LogLevel.Error);
                        break;
                }
            }
        }

        private IResult<StatementSyntax, Failure> CreateStatement(
            IMemberConfiguration memberConfiguration,
            string path,
            Context context) =>
            this.CreateValueConfiguration(memberConfiguration, context)
                .MapSuccess(valueConfig => ParseStatement($"this.With(p => p.{path}.{valueConfig});"));

        private IResult<string, Failure> CreateValueConfiguration(
            IMemberConfiguration memberConfiguration,
            Context context) =>
            memberConfiguration switch
            {
                LinkMemberConfiguration linkMemberConfiguration => this.GetLinkValueConfig(linkMemberConfiguration, context),
                NullValueMemberConfiguration _ => Result.Success("Value(null)").WithFailure<Failure>(),
                UndefinedMemberConfiguration _ => Result.Success("Undefined()").WithFailure<Failure>(),
                UniqueValueMemberConfiguration _ => Result.Success("Unique()").WithFailure<Failure>(),
                ValueMemberConfiguration valueMemberConfiguration => GetValueConfig(valueMemberConfiguration, context),
                _ => Result.Failure(
                        new Failure(
                            $"Failed to create value configuration because the memberConfiguration {memberConfiguration} is not a valid type."))
                    .WithSuccess<string>(),
            };

        private static IResult<string, Failure> GetValueConfig(
            ValueMemberConfiguration valueMemberConfiguration,
            Context context)
        {
            if (valueMemberConfiguration.Value is EnumValue e)
            {
                context.UpdateUsings(e.TypeFullName.GetNameSpace());
            }

            return Result
                .Success($"Value({valueMemberConfiguration.DisplayValue})")
                .WithFailure<Failure>();
        }

        private IResult<string, Failure> GetLinkValueConfig(
            LinkMemberConfiguration linkMemberConfiguration,
            Context context)
        {
            var typeFullName = linkMemberConfiguration.ConfigurationLink.TypeFullName;

            var memberTypeResult = this._typeSymbolQuery.GetTypeSymbol(
                context.Compilation,
                typeFullName);

            if (memberTypeResult.IsFailure)
            {
                return Result.Failure(new Failure($"Cannot resolve type {typeFullName.FullName}"))
                    .WithSuccess<string>();
            }

            var memberType = memberTypeResult.GetSuccessUnsafe();

            context.UpdateUsings(typeFullName.GetNameSpace());
            typeFullName.GenericTypeArguments().ForEach(name => context.UpdateUsings(name.GetNameSpace()));

            return
                Result.Success(
                        memberType.TypeKind == TypeKind.Interface
                            ? $"Stub<{typeFullName.GetFriendlyCSharpTypeName()}>()"
                            : $"InstanceOf<{typeFullName.GetFriendlyCSharpTypeName()}>()")
                    .WithFailure<Failure>();
        }

        private async Task<IResult<Context, Failure>> GetContextAsync(Maybe<string> rootItemPath)
        {
            var spanResult = await this._buildInvocationSpanQuery.GetSpanAsync(rootItemPath);
            var filePathResult = await this._documentFileNameQuery.GetDocumentFileName(rootItemPath);

            return await (
                from span in spanResult
                from filePath in filePathResult
                from context in this._roslynContextQuery.GetContextAsync(filePath)
                from configInformation in this._roslynConfigFinder.FindConfigClass(span, context)
                    .ToResult(new Failure("Cannot find the config class."))
                select new Context(context, configInformation));
        }

        private static ConstructorDeclarationSyntax GetOrCreateCtor(Context context) =>
            context.CurrentConfigNode.ChildNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .FirstOrNone()
                .SomeOrProvided(() => CreateCtor(context));

        private static ConstructorDeclarationSyntax CreateCtor(Context context)
        {
            // precondition no ctor exists

            var ctorNode = ConstructorDeclaration(context.CurrentConfigNode.Identifier)
                .AddModifiers(Token(SyntaxKind.PublicKeyword));

            context.CurrentConfigNode = context.CurrentConfigNode.AddMembers(ctorNode);
            return context.CurrentConfigNode.ChildNodes().OfType<ConstructorDeclarationSyntax>().Single();
        }

        #endregion
    }
}