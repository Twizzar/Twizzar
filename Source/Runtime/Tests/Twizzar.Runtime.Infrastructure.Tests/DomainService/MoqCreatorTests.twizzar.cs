using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Moq;

using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

using TwizzarInternal.Fixture;
using TwizzarInternal.Fixture.Member;

using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Runtime.Infrastructure.Tests.DomainService
{
    public static class ItemBuilderExtensions
    {
        public static void SetupImmutableArray<TFixtureItem, TPathType, TInnerType>(this ItemBuilder<TFixtureItem, TPathType> self, Func<TPathType, MethodMemberPath<TFixtureItem, ImmutableArray<TInnerType>, ImmutableArray<TInnerType>>> memberPathFunc)
            where TPathType : new() => self.With(p => memberPathFunc(p).Value(ImmutableArray.Create<TInnerType>()));
    }

    partial class MoqCreatorTests
    {
        private class IsGenericValueDefinitionBuilder : ItemBuilder<Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions.IPropertyDefinition, IPropertyDefinition7cc0BuilderPaths>
        {
            public IsGenericValueDefinitionBuilder()
            {
                const string propName = nameof(IRuntimeTypeDescription.IsGeneric);

                this.With(p => p.Name.Value(propName));
                this.With(p => p.ValueDefinition.Stub<IRawValueDefinition>());
                this.With(p => p.ValueDefinition.Value_.Value(true));
                this.With(p => p.PropertyDescription.Value(new DefaultPropertyDescriptionBuilder()
                    .With(x => x.Type.Value(typeof(bool)))
                    .With(x => x.Name.Value(propName))
                    .Build()));
            }
        }

        private class DefaultPropertyDescriptionBuilder : ItemBuilder<Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.IRuntimePropertyDescription, IRuntimePropertyDescription9ddbBuilderPaths>
        {
            public DefaultPropertyDescriptionBuilder()
            {
                this.With(p => p.Name.Unique());
                this.With(p => p.Type.Value(typeof(string)));
            }
        }

        private class EmptyIRuntimeTypeDescriptionBuilder : ItemBuilder<IRuntimeTypeDescription, EmptyIRuntimeTypeDescriptionPaths>
        {
            public EmptyIRuntimeTypeDescriptionBuilder()
            {
                this.SetupImmutableArray(p => p.GetDeclaredConstructors);
                this.SetupImmutableArray(p => p.GetDeclaredFields);
                this.SetupImmutableArray(p => p.GetDeclaredConstructors);
                this.SetupImmutableArray(p => p.GetDeclaredProperties);
            }
        }

        private class TrueIBaseTypeUniqueCreatorBuilder : ItemBuilder<IBaseTypeCreator, IBaseTypeUniqueCreatorbf07BuilderPaths>
        {
            public TrueIBaseTypeUniqueCreatorBuilder()
            {
                this.With(p => p.CreateInstance__IValueDefinition_IBaseDescription.Value(true));
            }
        }

        private class Method1DefinitionWithValue53Builder : ItemBuilder<Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions.IMethodDefinition, Method1DefinitionWithValue53BuilderPaths>
        {
            public Method1DefinitionWithValue53Builder()
            {
                this.With(p => p.MethodDescription.Stub<IRuntimeMethodDescription>());
                this.With(p => p.MethodDescription.Name.Value("Method1"));
                this.With(p => p.MethodDescription.Type.Value(typeof(int)));
                this.With(p => p.MethodDescription.TypeFullName.Value(TypeFullName.Create(typeof(int))));
                this.With(p => p.MethodDescription.DeclaredParameters.Value(ImmutableArray<IParameterDescription>.Empty));
                this.With(p => p.ValueDefinition.Stub<IRawValueDefinition>());
                this.With(p => p.ValueDefinition.Value_.Value(5));
                this.With(p => p.MethodDescription.GetMethodInfo.Value(
                    Maybe.Some(typeof(ITestInterface).GetMethod(nameof(ITestInterface.Method1)))));
            }
        }

        private class Method3DefinitionBuilder : ItemBuilder<CoreInterfaces.FixtureItem.Definition.MemberDefinitions.IMethodDefinition, IMethodDefinition3e59BuilderPaths>
        {
            public Method3DefinitionBuilder()
            {
                this.With(p => p.MethodDescription.Stub<IRuntimeMethodDescription>());
                this.With(p => p.MethodDescription.Name.Value("Method"));
                this.With(p => p.MethodDescription.Type.Value(typeof(int)));
                this.With(p => p.MethodDescription.TypeFullName.Value(TypeFullName.Create(typeof(int))));
                this.With(p => p.MethodDescription.GetMethodInfo.Value(
                    Maybe.Some(typeof(ITestInterface3).GetMethod(nameof(ITestInterface3.Method)))));


                this.With(p => p.MethodDescription.DeclaredParameters.Value(ImmutableArray<IParameterDescription>.Empty
                    .Add(Mock.Of<IRuntimeParameterDescription>(description => description.Type == typeof(int)))
                    .Add(Mock.Of<IRuntimeParameterDescription>(description => description.Type == typeof(int)))));
            }
        }

        private class Method32DefinitionBuilder : ItemBuilder<CoreInterfaces.FixtureItem.Definition.MemberDefinitions.IMethodDefinition, IMethodDefinition32BuilderPaths>
        {
            public Method32DefinitionBuilder()
            {
                this.With(p => p.MethodDescription.Stub<IRuntimeMethodDescription>());
                this.With(p => p.MethodDescription.Name.Value(nameof(ITestInterface3.Method2)));
                this.With(p => p.MethodDescription.Type.Value(typeof(List<>)));
                this.With(p => p.MethodDescription.TypeFullName.Value(TypeFullName.Create(typeof(List<>))));
                this.With(p => p.MethodDescription.GetMethodInfo.Value(
                    Maybe.Some(typeof(ITestInterface3).GetMethod(nameof(ITestInterface3.Method2)))));

                this.With(p => p.MethodDescription.DeclaredParameters.Value(ImmutableArray<IParameterDescription>.Empty));
            }
        }

        private class MoqCreatorWithBaseType5Builder : ItemBuilder<Twizzar.Runtime.Infrastructure.DomainService.MoqCreator, MoqCreatorafc3BuilderPaths>
        {
            public MoqCreatorWithBaseType5Builder()
            {
                this.With(p => p.Ctor.baseTypeCreator.Stub<IBaseTypeCreator>());
                this.With(p => p.Ctor.baseTypeCreator.CreateInstance__IValueDefinition_IBaseDescription.Value(5));
            }
        }

        private class Method2DefinitionBuilder : ItemBuilder<Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.MemberDefinitions.IMethodDefinition, IMethodDefinitiond17dBuilderPaths>
        {
            public Method2DefinitionBuilder()
            {
                this.With(p => p.MethodDescription.Stub<IRuntimeMethodDescription>());
                this.With(p => p.MethodDescription.Name.Value("Method2"));
                this.With(p => p.MethodDescription.Type.Value(typeof(int)));
                this.With(p => p.MethodDescription.TypeFullName.Value(TypeFullName.Create(typeof(int))));
                this.With(p => p.MethodDescription.DeclaredParameters.Value(ImmutableArray<IParameterDescription>.Empty));
                this.With(p => p.ValueDefinition.Stub<IRawValueDefinition>());
                this.With(p => p.ValueDefinition.Value_.Value(-12));
                this.With(p => p.MethodDescription.GetMethodInfo.Value(
                    Maybe.Some(typeof(ITestInterface2).GetMethod(nameof(ITestInterface2.Method2)))));
            }
        }

        private class MoqCreatorBuilder : ItemBuilder<Twizzar.Runtime.Infrastructure.DomainService.MoqCreator, MoqCreator0c5eBuilderPaths>
        {
        }
    }
}