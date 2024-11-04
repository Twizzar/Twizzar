using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.Util;
using Twizzar.SharedKernel.NLog.Logging;
using ViCommon.Functional.Monads.MaybeMonad;
using Exception = System.Exception;

namespace Twizzar.Runtime.Core.FixtureItem.Description
{
    /// <summary>
    /// The type description build with reflection. For more infos see <see cref="ITypeDescription"/>.
    /// </summary>
    public sealed class ReflectionTypeDescription : TypeDescription, IRuntimeTypeDescription
    {
        #region static fields and constants

        private const BindingFlags DefaultBindingFlags = BindingFlags.Public |
                                                         BindingFlags.Instance |
                                                         BindingFlags.NonPublic |
                                                         BindingFlags.Static;

        #endregion

        #region fields

        private readonly Lazy<ImmutableArray<IMethodDescription>> _declaredConstructors;
        private readonly Lazy<ImmutableArray<IFieldDescription>> _declaredFields;
        private readonly Lazy<ImmutableArray<IMethodDescription>> _declaredMethods;
        private readonly Lazy<ImmutableArray<IPropertyDescription>> _declaredProperties;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionTypeDescription"/> class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseTypeService"></param>
        public ReflectionTypeDescription(Type type, IBaseTypeService baseTypeService)
            : base(baseTypeService)
        {
            this.Type = type;
            this.TypeFullName = type.ToTypeFullName();
            this.AccessModifier = new AccessModifier(!type.IsPublic, type.IsPublic, false, false);
            this.BaseType = Maybe.ToMaybe(type.BaseType).Map(CoreInterfaces.FixtureItem.Name.TypeFullName.Create).Map(name => (ITypeFullName)name);
            this.AccessModifier = new AccessModifier(false, type.IsPublic, false, false);

            this.Type = type;
            this.IsClass = this.Type.IsClass;
            this.IsInterface = this.Type.IsInterface;
            this.IsEnum = this.Type.IsEnum;
            this.IsStruct = this.Type.IsStruct();
            this.IsArray = this.Type.IsArray;
            this.ArrayDimension = this.Type.GetArrayDimension().ToImmutableArray();
            this.IsDelegate = typeof(Delegate).IsAssignableFrom(this.Type);
            this.IsEnum = this.Type.IsEnum;
            this.IsAbstract = this.Type.IsAbstract;
            this.IsNested = this.Type.IsNested;
            this.IsSealed = this.Type.IsSealed;
            this.IsStatic = this.Type.IsAbstract && this.Type.IsSealed;
            this.IsGeneric = this.Type.ContainsGenericParameters;
            this.IsTypeParameter = this.Type.IsGenericParameter;

            this.ImplementedInterfaceNames =
                type.GetTypeInfo().ImplementedInterfaces.Select(t => t.ToTypeFullName().FullName).ToImmutableArray();

            this.GenericRuntimeTypeArguments = type.GetGenericArguments()
                .Select((value, index) => new { index, value })
                .ToImmutableDictionary(arg => arg.index, arg => arg.value);

            this.GenericTypeArguments = this.GenericRuntimeTypeArguments
                .ToImmutableDictionary(
                    pair =>
                        pair.Key,
                    pair =>
                        new GenericParameterType(
                            pair.Value.ToTypeFullName(),
                            pair.Value.Name,
                            pair.Key,
                            pair.Value.IsGenericParameter
                                ? pair.Value.GetGenericParameterConstraints()
                                    .Select(t => t.ToTypeFullName())
                                    .ToImmutableArray<ITypeFullName>()
                                : ImmutableArray.Create<ITypeFullName>()));

            var customDefaultCtor = Enumerable.Empty<IMethodDescription>();
            if (this.IsStruct || this.IsArray)
            {
                customDefaultCtor = customDefaultCtor.Append(
                    ReflectionMethodDescription.CreateCustomDefaultConstructorDescription(type, this.BaseTypeService, this));
            }

            this._declaredConstructors = new Lazy<ImmutableArray<IMethodDescription>>(() =>
                type.GetConstructors(DefaultBindingFlags)
                    .Select(info => (IMethodDescription)new ReflectionMethodDescription(info, type, baseTypeService))
                    .Concat(customDefaultCtor)
                    .ToImmutableArray());

            this._declaredFields = new Lazy<ImmutableArray<IFieldDescription>>(() =>
                GetMemberInfo(type, t => t.GetFields(DefaultBindingFlags))
                    .Select(info => (IFieldDescription)new ReflectionFieldDescription(info, baseTypeService))
                    .ToImmutableArray());

            this._declaredMethods = new Lazy<ImmutableArray<IMethodDescription>>(() =>
                GetMemberInfo(type, t => t.GetMethods(DefaultBindingFlags))
                    .Select(info =>
                        (IMethodDescription)new ReflectionMethodDescription(info, info.ReturnType, baseTypeService))
                    .Distinct(new HashEqualityComparer<IMethodDescription>(description => description.UniqueName.GetHashCode()))
                    .ToImmutableArray());

            this._declaredProperties = new Lazy<ImmutableArray<IPropertyDescription>>(() =>
                GetMemberInfo(type, t => t.GetProperties(DefaultBindingFlags))
                    .Select(info => (IPropertyDescription)new ReflectionPropertyDescription(info, baseTypeService))
                    .ToImmutableArray());
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public Type Type { get; }

        /// <inheritdoc />
        public IImmutableDictionary<int, Type> GenericRuntimeTypeArguments { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() => this;

        /// <inheritdoc />
        public override ImmutableArray<IMethodDescription> GetDeclaredConstructors() =>
            this._declaredConstructors.Value;

        /// <inheritdoc />
        public override ImmutableArray<IFieldDescription> GetDeclaredFields() => this._declaredFields.Value;

        /// <inheritdoc />
        public override ImmutableArray<IMethodDescription> GetDeclaredMethods() => this._declaredMethods.Value;

        /// <inheritdoc />
        public override ImmutableArray<IPropertyDescription> GetDeclaredProperties() => this._declaredProperties.Value;

        /// <inheritdoc />
        public override ITypeDescription GetGenericTypeArgumentDescription(int position) =>
            new ReflectionTypeDescription(this.Type.GenericTypeArguments[position], this.BaseTypeService);

        private static IEnumerable<T> GetMemberInfo<T>(Type type, Func<Type, IEnumerable<T>> getFunc)
            where T : MemberInfo
        {
            try
            {
                // make the given function safe by wrapping it with a try catch and return an empty sequence on failure.
                Func<Type, IEnumerable<T>> MakeSave(Func<Type, IEnumerable<T>> func) =>
                    t =>
                    {
                        try
                        {
                            return func(t);
                        }
                        catch (Exception e)
                        {
                            ViLog.Log<ReflectionTypeDescription>(e);
                            return Enumerable.Empty<T>();
                        }
                    };

                var getFuncSave = MakeSave(getFunc);

                IImmutableDictionary<string, T> members = getFuncSave(type).ToImmutableDictionary(GetUniqueName, x => x);

                if (type.BaseType != null)
                {
                    members = members
                        .Merge(
                            getFuncSave(type.BaseType)
                                .ToImmutableDictionary(GetUniqueName, x => x));
                }

                members = type.GetInterfaces()
                    .Aggregate(
                        members,
                        (current, @interface) => current.Merge(getFuncSave(@interface)
                            .ToImmutableDictionary(GetUniqueName, x => x)));

                return members.Values;
            }
            catch (Exception e)
            {
                ViLog.Log<ReflectionTypeDescription>(e);
                return Enumerable.Empty<T>();
            }
        }

        private static string GetUniqueName(MemberInfo memberInfo) =>
            memberInfo switch
            {
                MethodInfo x =>
                    $"{x.ReturnType.ToTypeFullName()} {GetUniqueNameForMethodBase(x)}",

                MethodBase x =>
                    GetUniqueNameForMethodBase(x),
                _ => memberInfo.Name,
            };

        private static string GetUniqueNameForMethodBase(MethodBase methodBase) =>
            methodBase.Name +
            methodBase.GetGenericArguments()
                .ToDisplayString(", ", "<", ">") +
            methodBase.GetParameters()
                .Select(info => $"{info.Name}{info.ParameterType.ToTypeFullName()}")
                .ToDisplayString(", ", "(", ")");

        #endregion
    }
}