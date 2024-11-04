using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Ui.Interfaces.Factories;
using Twizzar.Design.Ui.Interfaces.Parser;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Parser;
using Twizzar.Design.Ui.Validator;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional;

namespace Twizzar.VsAddin.Factory
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class FixtureItemValueViewModelFactory : IFixtureItemValueViewModelFactory
    {
        #region fields

        private readonly IBaseTypeService _baseTypeService;
        private readonly Factory _factory;
        private readonly IAssignableTypesQuery _assignableTypesQuery;
        private readonly IShortTypesConverter _shortTypesConverter;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemValueViewModelFactory"/> class.
        /// </summary>
        /// <param name="baseTypeService">The base type service.</param>
        /// <param name="factory">Delegate factory.</param>
        /// <param name="assignableTypesQuery"></param>
        /// <param name="shortTypesConverter"></param>
        public FixtureItemValueViewModelFactory(
            IBaseTypeService baseTypeService,
            Factory factory,
            IAssignableTypesQuery assignableTypesQuery,
            IShortTypesConverter shortTypesConverter)
        {
            this.EnsureMany()
                .Parameter(factory, nameof(factory))
                .Parameter(assignableTypesQuery, nameof(assignableTypesQuery))
                .Parameter(baseTypeService, nameof(baseTypeService))
                .Parameter(shortTypesConverter, nameof(shortTypesConverter))
                .ThrowWhenNull();

            this._factory = factory;
            this._assignableTypesQuery = assignableTypesQuery;
            this._shortTypesConverter = shortTypesConverter;
            this._baseTypeService = baseTypeService;
        }

        #endregion

        #region delegates

        /// <summary>
        /// Autofac factory.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parser"></param>
        /// <param name="validator"></param>
        /// <param name="isReadOnly"></param>
        /// <returns>A new instance of <see cref="IFixtureItemNodeValueViewModel"/>.</returns>
        public delegate IFixtureItemNodeValueViewModel Factory(
            NodeId id,
            IParser parser,
            IValidator validator,
            bool isReadOnly);

        #endregion

        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public IFixtureItemNodeValueViewModel CreateWithType(
            NodeId id,
            IBaseDescription typeDescription,
            IFixtureItemInformation fixtureItemInformation,
            ICompilationTypeQuery compilationTypeQuery) =>
            (fixtureItemInformation.MemberConfiguration.Source is FromCode || fixtureItemInformation.MemberConfiguration is CodeValueMemberConfiguration)
                ? this._factory(
                    id,
                    new CodeParser(),
                    new CodeValidator(typeDescription, fixtureItemInformation.MemberConfiguration),
                    true)
                : this.GetByFixtureKind(id, typeDescription, fixtureItemInformation, compilationTypeQuery);

        /// <inheritdoc />
        public IFixtureItemNodeValueViewModel CreateForCtor(NodeId id, IBaseDescription baseDescription) =>
            this._factory(id, new CtorParser(), new CtorValidator(baseDescription), true);

        private IFixtureItemNodeValueViewModel GetByFixtureKind(
            NodeId id,
            IBaseDescription typeDescription,
            IFixtureItemInformation fixtureItemInformation,
            ICompilationTypeQuery compilationTypeQuery) =>
            this._baseTypeService.GetKind(typeDescription) switch
            {
                BaseTypeKind.Number =>
                    this._factory(
                        id,
                        new NumericParser(),
                        new NumericValidator(typeDescription),
                        fixtureItemInformation.MemberConfiguration.Source is FromCode),
                BaseTypeKind.Char =>
                    this._factory(
                        id,
                        new CharParser(),
                        new CharValidator(typeDescription),
                        fixtureItemInformation.MemberConfiguration.Source is FromCode),
                BaseTypeKind.String =>
                    this._factory(
                        id,
                        new StringParser(),
                        new StringValidator(typeDescription),
                        fixtureItemInformation.MemberConfiguration.Source is FromCode),
                BaseTypeKind.Boolean =>
                    this._factory(
                        id,
                        new BoolParser(),
                        new BoolValidator(typeDescription),
                        fixtureItemInformation.MemberConfiguration.Source is FromCode),
                BaseTypeKind.Enum =>
                    this._factory(
                        id,
                        new EnumParser(),
                        new EnumValidator(typeDescription),
                        fixtureItemInformation.MemberConfiguration.Source is FromCode),
                BaseTypeKind.Complex =>
                    this._factory(
                        id,
                        GetComplexParser(typeDescription),
                        this.GetComplexValidator(typeDescription, fixtureItemInformation, compilationTypeQuery),
                        fixtureItemInformation.MemberConfiguration.Source is FromCode),
                _ => throw new PatternErrorBuilder(nameof(this._baseTypeService.GetKind))
                    .IsNotOneOf(nameof(BaseTypeKind)),
            };

        private static IParser GetComplexParser(IBaseDescription description) =>
            description switch
            {
                IMethodDescription { IsGeneric: true } => new GenericTypeParser(),
                { IsStruct: true } => new ComplexParser(),
                _ => new NullableComplexParser(),
            };

        private IValidator GetComplexValidator(
            IBaseDescription description,
            IFixtureItemInformation fixtureItemInformation,
            ICompilationTypeQuery compilationTypeQuery) =>
            description switch
            {
                IMethodDescription { IsGeneric: true } => new GenericTypeValidator(description, compilationTypeQuery, this._assignableTypesQuery, this._shortTypesConverter),
                _ => new ComplexValidator(description, fixtureItemInformation, this._assignableTypesQuery),
            };

        #endregion
    }
}