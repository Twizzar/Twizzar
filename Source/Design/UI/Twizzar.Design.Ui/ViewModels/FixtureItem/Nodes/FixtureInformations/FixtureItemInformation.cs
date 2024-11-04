using System;
using System.Collections.Generic;
using System.Linq;
using Twizzar.Design.Shared.CoreInterfaces.FixtureItem.Description;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Enums;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using PatternErrorBuilder = ViCommon.Functional.PatternErrorBuilder;

namespace Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.FixtureInformations
{
    /// <inheritdoc cref="IFixtureItemInformation" />
    public class FixtureItemInformation : ValueObject, IFixtureItemInformation
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemInformation"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentPath"></param>
        /// <param name="fixtureDescription"></param>
        /// <param name="memberConfiguration"></param>
        public FixtureItemInformation(
            FixtureItemId id,
            string parentPath,
            IBaseDescription fixtureDescription,
            IMemberConfiguration memberConfiguration)
            : this(
                id,
                fixtureDescription,
                memberConfiguration,
                parentPath,
                CheckIfIsExpandable(fixtureDescription, memberConfiguration))
        {
        }

        private FixtureItemInformation(
            FixtureItemId id,
            IBaseDescription fixtureDescription,
            IMemberConfiguration memberConfiguration,
            string parentPath,
            bool canBeExpanded)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(id, nameof(id))
                .Parameter(parentPath, nameof(parentPath))
                .Parameter(fixtureDescription, nameof(fixtureDescription))
                .Parameter(memberConfiguration, nameof(memberConfiguration))
                .ThrowWhenNull();

            this.Id = id;
            this.FixtureDescription = fixtureDescription;
            this.MemberConfiguration = memberConfiguration;
            this.TypeFullName = fixtureDescription.TypeFullName;
            this.ParentPath = parentPath;
            var prefix = parentPath != string.Empty ? parentPath + "." : parentPath;
            this.Path = prefix + fixtureDescription.GetMemberPathName();
            this.CanBeExpanded = canBeExpanded;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public FixtureItemId Id { get; }

        /// <inheritdoc />
        public string ParentPath { get; }

        /// <inheritdoc />
        public string Path { get; }

        /// <inheritdoc />
        public IBaseDescription FixtureDescription { get; }

        /// <inheritdoc />
        public IMemberConfiguration MemberConfiguration { get; }

        /// <inheritdoc />
        public ITypeFullName TypeFullName { get; }

        /// <inheritdoc />
        public string FriendlyTypeFullName =>
            this.FixtureDescription switch
            {
                IMethodDescription m =>
                    this.GetMethodTypeDescription(m, name => name.GetFriendlyCSharpFullName()),
                _ => this.TypeFullName.GetFriendlyCSharpFullName(),
            };

        /// <inheritdoc />
        public string FriendlyTypeName =>
            this.FixtureDescription switch
            {
                IMethodDescription m =>
                    this.GetMethodTypeDescription(m, name => name.GetFriendlyCSharpTypeName()),
                _ => this.TypeFullName.GetFriendlyCSharpTypeName(),
            };

        /// <inheritdoc />
        public bool IsDefault =>
            this.MemberConfiguration switch
            {
                MethodConfiguration x =>
                    x.ReturnValue.Source is FromSystemDefault,
                _ => this.MemberConfiguration.Source is FromSystemDefault,
            };

        /// <inheritdoc />
        public bool CanBeExpanded { get; }

        /// <inheritdoc />
        public MemberKind Kind =>
            this.FixtureDescription switch
            {
                IFieldDescription _ => MemberKind.Field,
                IMethodDescription _ => MemberKind.Method,
                IPropertyDescription _ => MemberKind.Property,
                IParameterDescription _ => MemberKind.Field,
                ITypeDescription _ => MemberKind.Field,
                _ => throw new PatternErrorBuilder(nameof(this.FixtureDescription))
                    .IsNotOneOf(
                        nameof(IFieldDescription),
                        nameof(IMethodDescription),
                        nameof(IPropertyDescription),
                        nameof(IParameterDescription),
                        nameof(ITypeDescription)),
            };

        /// <inheritdoc />
        public MemberModifier Modifier =>
            this.FixtureDescription switch
            {
                IMethodDescription methodDescription => methodDescription.AccessModifier.MapToMemberModifier(),
                IMemberDescription memberDescription => memberDescription.AccessModifier.MapToMemberModifier(),
                IParameterDescription _ => MemberModifier.NotDefined,
                ITypeDescription typeDescription => typeDescription.AccessModifier.MapToMemberModifier(),
                _ => throw new PatternErrorBuilder(nameof(this.FixtureDescription))
                    .IsNotOneOf(
                        nameof(IMethodDescription),
                        nameof(IMemberDescription),
                        nameof(IParameterDescription),
                        nameof(ITypeDescription)),
            };

        /// <inheritdoc />
        public string DisplayValue =>
            this.MemberConfiguration switch
            {
                CtorMemberConfiguration _ when this.FixtureDescription is IDesignMethodDescription methodDescription =>
                    $"({methodDescription.FriendlyParameterTypes})",

                MethodConfiguration x when this.FixtureDescription is IDesignMethodDescription =>
                    GetDisplayValue(x.ReturnValue),

                _ => GetDisplayValue(this.MemberConfiguration),
            };

        /// <inheritdoc />
        public string DisplayName =>
            this.MemberConfiguration switch
            {
                MethodConfiguration m => m.MethodName,
                _ => this.MemberConfiguration.Name,
            };

        #endregion

        #region members

        /// <inheritdoc />
        public IFixtureItemInformation With(IMemberConfiguration configuration) =>
            new FixtureItemInformation(this.Id, this.ParentPath, this.FixtureDescription, configuration);

        /// <inheritdoc />
        public IFixtureItemInformation With(FixtureItemId id) =>
            new FixtureItemInformation(
                id,
                this.FixtureDescription,
                this.MemberConfiguration,
                this.ParentPath,
                this.CanBeExpanded);

        /// <inheritdoc />
        public IFixtureItemInformation WithNotExpandable() =>
            new FixtureItemInformation(
                this.Id,
                this.FixtureDescription,
                this.MemberConfiguration,
                this.ParentPath,
                false);

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Id;
            yield return this.FixtureDescription;
            yield return this.MemberConfiguration;
            yield return this.TypeFullName;
            yield return this.Path;
        }

        /// <summary>
        /// Get the return type for a method and the parameters types.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="getTypeNameFuc"></param>
        /// <returns>[ReturnType] ([Param1Type] [Param1Name], [Param2Type] [Param2Name]).</returns>
        private string GetMethodTypeDescription(IMethodDescription m, Func<ITypeFullName, string> getTypeNameFuc)
        {
            var returnType = getTypeNameFuc(this.TypeFullName);

            var parameters = m.DeclaredParameters
                .Select(description => $"{getTypeNameFuc(description.TypeFullName)} {description.Name}")
                .ToDisplayString(", ");

            return $"{returnType} ({parameters})";
        }

        private static string GetDisplayValue(IMemberConfiguration memberConfiguration) =>
            memberConfiguration switch
            {
                LinkMemberConfiguration x => x.ConfigurationLink.TypeFullName.GetFriendlyCSharpFullName(),
                UndefinedMemberConfiguration _ => KeyWords.Undefined,
                UniqueValueMemberConfiguration _ => KeyWords.Unique,
                ValueMemberConfiguration x => x.DisplayValue,
                NullValueMemberConfiguration _ => KeyWords.Null,
                CodeValueMemberConfiguration x => x.SourceCode,
                _ => KeyWords.Undefined,
            };

        private static bool CheckIfIsExpandable(
            IBaseDescription fixtureDescription,
            IMemberConfiguration memberConfiguration) =>
            memberConfiguration switch
            {
                MethodConfiguration methodConfiguration =>
                    CheckIfIsExpandableInternal(
                        fixtureDescription.GetReturnTypeDescription(),
                        methodConfiguration.ReturnValue),
                _ => CheckIfIsExpandableInternal(fixtureDescription, memberConfiguration),
            };

        private static bool CheckIfIsExpandableInternal(
            IBaseDescription fixtureDescription,
            IMemberConfiguration memberConfiguration) =>
            (memberConfiguration is LinkMemberConfiguration ||
             memberConfiguration is UndefinedMemberConfiguration ||
             memberConfiguration is CtorMemberConfiguration) &&
            !fixtureDescription.IsBaseType &&
            !fixtureDescription.IsNullableBaseType &&
            !(fixtureDescription.IsTypeParameter && memberConfiguration is not LinkMemberConfiguration) &&
            !(fixtureDescription is ITypeDescription { IsGeneric: true } && memberConfiguration is not LinkMemberConfiguration);

        #endregion
    }
}