using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Core.FixtureItem.Description
{
    /// <summary>
    /// The method description build with reflection. For more infos see <see cref="IMethodDescription"/>.
    /// </summary>
    public sealed class ReflectionMethodDescription : MethodDescription, IRuntimeMethodDescription
    {
        #region fields

        private readonly Lazy<ImmutableArray<IParameterDescription>> _declaredParameters;
        private readonly Type _returnType;
        private readonly Maybe<MethodBase> _methodBase;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionMethodDescription"/> class.
        /// </summary>
        /// <param name="methodBase"></param>
        /// <param name="returnType"></param>
        /// <param name="baseTypeService"></param>
        public ReflectionMethodDescription(MethodBase methodBase, Type returnType, IBaseTypeService baseTypeService)
            : base(baseTypeService)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(methodBase, nameof(methodBase))
                .Parameter(returnType, nameof(returnType))
                .ThrowWhenNull();

            this._methodBase = methodBase;
            this._returnType = returnType;
            this.TypeFullName = returnType.ToTypeFullName();
            this.Name = methodBase.Name;
            this.Type = returnType;
            this.OverrideKind = OverrideKind.Create(methodBase.IsVirtual, methodBase.IsFinal);
            this.IsGeneric = methodBase.ContainsGenericParameters;
            this.IsAbstract = methodBase.IsAbstract;
            this.IsConstructor = methodBase.IsConstructor;
            this.IsStatic = methodBase.IsStatic;
            this.GenericTypeArguments = GetGenericTypeArguments(methodBase);

            this.Type = returnType;
            this.IsClass = this.Type.IsClass;
            this.IsInterface = this.Type.IsInterface;
            this.IsEnum = this.Type.IsEnum;
            this.IsStruct = this.Type.IsStruct();
            this.IsArray = this.Type.IsArray;
            this.ArrayDimension = this.Type.GetArrayDimension().ToImmutableArray();
            this.DeclaredDescription = new ReflectionTypeDescription(methodBase.DeclaringType, this.BaseTypeService);
            this.IsTypeParameter = this.Type.IsGenericParameter;

            this.MethodKind = methodBase.MemberType switch
            {
                MemberTypes.Constructor => MethodKind.Constructor,
                MemberTypes.Method => MethodKind.Ordinary,
                MemberTypes.Property => MethodKind.Property,
                _ => MethodKind.Other,
            };

            this.AccessModifier = new AccessModifier(
                methodBase.IsPrivate,
                methodBase.IsPublic,
                methodBase.IsFamily,
                methodBase.IsAssembly);

            this._declaredParameters = new Lazy<ImmutableArray<IParameterDescription>>(
                () =>
                    methodBase.GetParameters()
                        .Select(
                            info => (IParameterDescription)new ReflectionParameterDescription(info, baseTypeService))
                        .ToImmutableArray());
        }

        private ReflectionMethodDescription(Type returnType, IBaseTypeService baseTypeService, ITypeDescription declaringType)
            : base(baseTypeService)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(returnType, nameof(returnType))
                .ThrowWhenNull();

            this._returnType = returnType;
            this.TypeFullName = returnType.ToTypeFullName();
            this.Name = ".ctor";
            this.Type = returnType;
            this.OverrideKind = OverrideKind.Create();
            this.IsGeneric = false;
            this.IsAbstract = false;
            this.IsConstructor = true;
            this.IsStatic = false;
            this.GenericTypeArguments = ImmutableDictionary<int, GenericParameterType>.Empty;
            this.AccessModifier = AccessModifier.CreatePublic();
            this.DeclaredDescription = declaringType;

            this._declaredParameters = new Lazy<ImmutableArray<IParameterDescription>>(
                () =>
                    ImmutableArray<IParameterDescription>.Empty);
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public override ImmutableArray<IParameterDescription> DeclaredParameters => this._declaredParameters.Value;

        /// <inheritdoc />
        public override ITypeDescription DeclaredDescription { get; }

        /// <inheritdoc />
        public Type Type { get; }

        /// <inheritdoc />
        public Maybe<MethodInfo> GetMethodInfo() =>
            this._methodBase.Bind(@base =>
                @base.CastTo<MethodInfo>()
                    .Match(Maybe.Some, _ => Maybe.None()));

        #endregion

        #region members

        /// <summary>
        /// Create a default constructor for a structs.
        /// </summary>
        /// <param name="type">The struct type.</param>
        /// <param name="baseTypeService"></param>
        /// <param name="declaringDescription"></param>
        /// <returns>A new <see cref="ReflectionMethodDescription"/>.</returns>
        public static ReflectionMethodDescription CreateCustomDefaultConstructorDescription(
            Type type,
            IBaseTypeService baseTypeService,
            ITypeDescription declaringDescription) =>
            new(type, baseTypeService, declaringDescription);

        /// <inheritdoc />
        public override ITypeDescription GetReturnTypeDescription() =>
            new ReflectionTypeDescription(this._returnType, this.BaseTypeService);

        private static ImmutableDictionary<int, GenericParameterType> GetGenericTypeArguments(MethodBase methodBase)
        {
            // This is a generic method
            if (methodBase.IsGenericMethod && methodBase is MethodInfo methodInfo)
            {
                // This has generic parameters values (like T).
                if (methodInfo.ContainsGenericParameters)
                {
                    return methodBase.GetGenericArguments()
                        .Select((type, index) => new GenericParameterType(
                            Maybe.None(),
                            type.Name,
                            index,
                            type.GetGenericParameterConstraints()
                                .Select(t => t.ToTypeFullName()).ToImmutableArray<ITypeFullName>()))
                        .ToImmutableDictionary(x => x.Position, x => x);
                }

                var genericMethod = methodInfo.GetGenericMethodDefinition();

                return methodBase.GetGenericArguments()
                    .Zip(
                        genericMethod.GetGenericArguments(),
                        (type, genericType) => (type, genericType))
                    .Select(
                        (tuple, index) =>
                            new GenericParameterType(
                                tuple.type.ToTypeFullName(),
                                tuple.genericType.Name,
                                index,
                                tuple.genericType.GetGenericParameterConstraints()
                                    .Select(t => t.ToTypeFullName())
                                    .ToImmutableArray<ITypeFullName>()))
                    .ToImmutableDictionary(x => x.Position, x => x);
            }
            else
            {
                return ImmutableDictionary<int, GenericParameterType>.Empty;
            }
        }

        #endregion
    }
}