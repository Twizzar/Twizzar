using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.CoreInterfaces.Query.Services.ReadModel;
using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.Queries;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations;
using Twizzar.SharedKernel.CoreInterfaces.Exceptions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Queries
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage] // Gets tested in the next PR
    public class FixtureItemNodeViewModelQuery : IFixtureItemNodeViewModelQuery
    {
        private readonly IFixtureItemNodeFactory _factory;
        private readonly IFixtureItemReadModelQuery _readModelQuery;
        private readonly IFixtureItemValueViewModelFactory _valueViewModelFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemNodeViewModelQuery"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="readModelQuery">The read model query.</param>
        /// <param name="valueViewModelFactory">The value view model factory.</param>
        public FixtureItemNodeViewModelQuery(
            IFixtureItemNodeFactory factory,
            IFixtureItemReadModelQuery readModelQuery,
            IFixtureItemValueViewModelFactory valueViewModelFactory)
        {
            this.EnsureMany()
                .Parameter(factory, nameof(factory))
                .Parameter(readModelQuery, nameof(readModelQuery))
                .Parameter(valueViewModelFactory, nameof(valueViewModelFactory))
                .ThrowWhenNull();

            this._factory = factory;
            this._readModelQuery = readModelQuery;
            this._valueViewModelFactory = valueViewModelFactory;
        }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IMemberDefinitionViewModelQuery

        /// <inheritdoc />
        public async Task<IResult<IEnumerable<IFixtureItemNodeViewModel>, Failure>> GetFixtureItemNodeViewModels(
            FixtureItemId id,
            Maybe<string> memberName,
            Maybe<IFixtureItemNode> parent,
            ICompilationTypeQuery compilationTypeQuery)
        {
            this.EnsureParameter(id, nameof(id)).ThrowWhenNull();

            var model = await this._readModelQuery.GetFixtureItem(id);

            return model.MapSuccess(
                m =>
                    m switch
                    {
                        BaseTypeFixtureItemModel baseTypeFixtureItemModel => new List<IFixtureItemNodeViewModel>()
                        {
                            this.CreateBaseType(baseTypeFixtureItemModel, parent, compilationTypeQuery),
                        },
                        ObjectFixtureItemModel objectFixtureItemModel => this.GetMemberDefinitionViewModels(
                            objectFixtureItemModel,
                            parent,
                            compilationTypeQuery),
                        _ => throw new PatternErrorBuilder(nameof(model))
                            .IsNotOneOf(nameof(BaseTypeFixtureItemModel), nameof(ObjectFixtureItemModel)),
                    });
        }

        private IFixtureItemNodeViewModel CreateBaseType(BaseTypeFixtureItemModel model, Maybe<Interfaces.ViewModels.FixtureItem.Nodes.IFixtureItemNode> parent, ICompilationTypeQuery compilationTypeQuery)
        {
            var nodeId = new NodeId();
            var parentPath = parent
                .Map(node => node.FixtureItemInformation.Path)
                .BindNone(() => model.Id.RootItemPath)
                .SomeOrProvided(() => throw new InternalException($"{model.Id.RootItemPath} is None."));

            var sender = this._factory.CreateSender();
            var receiverFunc = this._factory.CreateReceiverFunc(true);
            var information = new FixtureItemInformation(model.Id, parentPath, model.Description, model.Value);
            var valueVm = this._valueViewModelFactory.CreateWithType(
                nodeId, model.Description, information, compilationTypeQuery);

            return this._factory.CreateViewModelNode(
                nodeId,
                receiverFunc,
                this.EmptyChildrenQueryAsync,
                sender,
                information,
                valueVm,
                parent);
        }

        private IEnumerable<IFixtureItemNodeViewModel> GetMemberDefinitionViewModels(
            ObjectFixtureItemModel model,
            Maybe<IFixtureItemNode> parent,
            ICompilationTypeQuery compilationTypeQuery)
        {
            var parentPath = parent
                .Map(node => node.FixtureItemInformation.Path)
                .BindNone(() => model.Id.RootItemPath)
                .SomeOrProvided(() => throw new InternalException($"{model.Id.RootItemPath} is None."));

            var sender = this._factory.CreateSender();
            var methodSender = this._factory.CreateMethodSender();

            if (model.UsedConstructor.IsSome)
            {
                var nodeId = new NodeId();
                var ctor = model.UsedConstructor.GetValueUnsafe();

                var ctorReceiverFunc = this._factory.CreateCtorReceiverFunc(ctor.Parameters.Length > 0);
                var information = new FixtureItemInformation(model.Id, parentPath, ctor.MethodDescription, ctor.Configuration);
                var valueVm = this._valueViewModelFactory.CreateForCtor(nodeId, ctor.MethodDescription);

                yield return this._factory.CreateViewModelNode(
                    nodeId,
                    ctorReceiverFunc,
                    (p, fixtureInformation) =>
                        Task.FromResult(Result.Success<IEnumerable<IFixtureItemNodeViewModel>, Failure>(
                            this.CreateCtorChildren(ctor, fixtureInformation.Id, p, compilationTypeQuery))),
                    sender,
                    information,
                    valueVm,
                    parent);
            }

            foreach (var value in GetMembersOrdered(model))
            {
                var nodeId = new NodeId();
                var usedSender = value.Description is IMethodDescription ? methodSender : sender;
                var receiverFunc = this._factory.CreateReceiverFunc(true);
                var information = new FixtureItemInformation(model.Id, parentPath, value.Description, value.Configuration);
                var valueVm = this._valueViewModelFactory.CreateWithType(nodeId, value.Description, information, compilationTypeQuery);
                yield return this._factory.CreateViewModelNode(
                    nodeId,
                    receiverFunc,
                    (parent1, fixtureItemInformation) => this.CreateChildrenAsync(parent1, fixtureItemInformation, compilationTypeQuery),
                    usedSender,
                    information,
                    valueVm,
                    parent);
            }
        }

        private static IEnumerable<FixtureItemMemberModel>
            GetMembersOrdered(ObjectFixtureItemModel model) =>
                model.Properties.OrderBy(elem => elem.Key).Select(pair => pair.Value)
                    .Concat(
                        model.Fields.OrderBy(elem => elem.Key).Select(pair => pair.Value))
                    .Concat(
                        model.Methods.OrderBy(elem => elem.Key).Select(pair => pair.Value));

        private IEnumerable<IFixtureItemNodeViewModel> CreateCtorChildren(
            FixtureItemConstructorModel ctorModel,
            FixtureItemId id,
            Maybe<IFixtureItemNode> parent,
            ICompilationTypeQuery compilationTypeQuery)
        {
            var parentPath = parent
                .Map(node => node.FixtureItemInformation.Path)
                .SomeOrProvided(() => throw new InternalException($"{parent} is None."));

            var sender = this._factory.CreateParameterSender();
            var receiverFunc = this._factory.CreateReceiverFunc(true);

            foreach (var param in ctorModel.Parameters)
            {
                var nodeId = new NodeId();
                var information = new FixtureItemInformation(id, parentPath, param.Description, param.Configuration);
                var valueVm = this._valueViewModelFactory.CreateWithType(nodeId, param.Description, information, compilationTypeQuery);

                yield return this._factory.CreateViewModelNode(
                    nodeId,
                    receiverFunc,
                    (parent1, fixtureItemInformation) => this.CreateChildrenAsync(parent1, fixtureItemInformation, compilationTypeQuery),
                    sender,
                    information,
                    valueVm,
                    parent);
            }
        }

        private async Task<IResult<IEnumerable<IFixtureItemNodeViewModel>, Failure>> CreateChildrenAsync(
            Maybe<IFixtureItemNode> parent,
            IFixtureItemInformation fixtureItemInformation,
            ICompilationTypeQuery compilationTypeQuery)
        {
            var memberConfiguration =
                (fixtureItemInformation.MemberConfiguration is MethodConfiguration methodConfiguration)
                    ? methodConfiguration.ReturnValue
                    : fixtureItemInformation.MemberConfiguration;

            if (memberConfiguration is LinkMemberConfiguration linkMemberConfiguration)
            {
                return await this.GetFixtureItemNodeViewModels(
                        linkMemberConfiguration.ConfigurationLink, linkMemberConfiguration.Name, parent, compilationTypeQuery);
            }
            else if (memberConfiguration is UndefinedMemberConfiguration undefinedMemberConfiguration)
            {
                var id = FixtureItemId.CreateNameless(undefinedMemberConfiguration.Type)
                    .WithRootItemPath(fixtureItemInformation.Id.RootItemPath);

                return await this.GetFixtureItemNodeViewModels(
                        id, undefinedMemberConfiguration.Name, parent, compilationTypeQuery);
            }

            return Result.Success<IEnumerable<IFixtureItemNodeViewModel>, Failure>(Enumerable.Empty<IFixtureItemNodeViewModel>());
        }

        private Task<IResult<IEnumerable<IFixtureItemNodeViewModel>, Failure>> EmptyChildrenQueryAsync(
            Maybe<Interfaces.ViewModels.FixtureItem.Nodes.IFixtureItemNode> parent,
            IFixtureItemInformation fixtureItemInformation) =>
            Task.FromResult(
                Result.Success<IEnumerable<IFixtureItemNodeViewModel>, Failure>(
                    Enumerable.Empty<IFixtureItemNodeViewModel>()));

        #endregion
    }
}
