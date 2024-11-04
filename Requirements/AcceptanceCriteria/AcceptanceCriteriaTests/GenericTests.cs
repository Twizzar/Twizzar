using DemoCode.ExampleCode;
using DemoCode.Interfaces.ExampleCode;

using FluentAssertions;

using NUnit.Framework;

using System.Collections.Generic;
using Twizzar.Fixture;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace AcceptanceCriteriaTests
{
    public partial class GenericTests
    {
        [Test]
        public void Simple_gerneric_method_works_correctly()
        {
            var sutInt = new ItemBuilder<IGenericMethods>()
                .With(p => p.SimpleGenericMethodT.Value(5))
                .Build();

            sutInt.SimpleGenericMethod<int>().Should().Be(5);
            sutInt.SimpleGenericMethod<float>().Should().Be(0);

            var sutFloat = new ItemBuilder<IGenericMethods>()
                .With(p => p.SimpleGenericMethodT.Value(5f))
                .Build();

            sutFloat.SimpleGenericMethod<float>().Should().Be(5);
            sutFloat.SimpleGenericMethod<int>().Should().Be(0);
        }

        [Test]
        public void Delegate_configuration_works_with_generics()
        {
            var sut = new ItemBuilder<IGenericMethods>()
                .With(p => p.SimpleGenericMethodT__T.Value<object>(x => x))
                .Build();

            sut.SimpleGenericMethod(4).Should().Be(4);
            sut.SimpleGenericMethod("Test").Should().Be("Test");

            var car = new Car();
            sut.SimpleGenericMethod(car).Should().Be(car);

            var sutInt = new ItemBuilder<IGenericMethods>()
                .With(p => p.SimpleGenericMethodT__T.Value<int>(x => x + 1))
                .Build();

            sutInt.SimpleGenericMethod(3).Should().Be(4);
            sutInt.SimpleGenericMethod(2.0f).Should().Be(0);
            sutInt.SimpleGenericMethod(new Car()).Should().Be(null);
        }

        [Test]
        public void Callback_can_be_configured_for_generics()
        {
            var callbacks =  new List<object>();

            var sut = new ItemBuilder<IGenericMethods>()
                .With(p => p.SimpleGenericMethodT__T.Callback<object>(x => callbacks.Add(x)))
                .Build();

            callbacks.Should().HaveCount(0);

            sut.SimpleGenericMethod(3);

            callbacks.Should().HaveCount(1);
            callbacks.Should().Contain(3);

            var car = new Car();
            sut.SimpleGenericMethod(car);

            callbacks.Should().HaveCount(2);
            callbacks.Should().Contain(car);
        }

        [Test]
        public void Single_nested_generic_can_be_configured()
        {
            var sut = new ItemBuilder<IGenericMethods>()
                .With(p => p.CreateNewListT.Value(new List<int>()))
                .Build();

            sut.CreateNewList(3, 4).Should().BeAssignableTo<List<int>>();

            var sut2 = new ItemBuilder<IGenericMethods>()
                .With(p => p.CreateNewListT.Value<int>(x => x.ToList()))
                .Build();

            var bla = sut2.CreateNewList(1, 2, 3);
            sut2.CreateNewList(1, 2, 3).Should().BeEquivalentTo(1, 2, 3);
        }

        [Test]
        public void Generic_method_with_struct_constrain_can_be_configured()
        {
            var sut = new ItemBuilder<IGenericMethods>()
                .With(p => p.StructGenericT.Value(5))
                .Build();

            sut.StructGeneric(3).Should().Be(5);

            var sutDelegate = new ItemBuilder<IGenericMethods>()
                .With(p => p.StructGenericT.Value<int>(x => x + 1))
                .Build();

            sutDelegate.StructGeneric(3).Should().Be(4);
        }

        [Test]
        public void Generic_method_with_one_type_constrain_can_be_configured()
        {
            var car = new Car();

            var sut = new ItemBuilder<IGenericMethods>()
                .With(p => p.ConstrainGenericT.Value(car))
                .Build();

            sut.ConstrainGeneric<Car>(car).Should().Be(car);
            sut.ConstrainGeneric<IVehicle>(car).Should().Be(car);
        }

        [Test]
        public void Generic_method_with_one_nested_type_constrain_can_be_configured()
        {
            var l = new List<IList<Car>>() {  new List<Car> { } };

            var sut = new ItemBuilder<IGenericMethods>()
                .With(p => p.ConstrainNestedGenericT.Value(l))
                .Build();

            sut.ConstrainNestedGeneric<Car>(new List<Car>()).Should().BeEquivalentTo(l);
        }

        [Test]
        public void Verification_with_generics_is_working()
        {
            var sut = new EmptyIGenericMethodsBuilder()
                .Build(out var context);

            sut.CreateNewList(1, 2, 3);
            sut.CreateNewList('a', 'b', 'c');

            context.Verify(p => p.CreateNewListT)
                .WhereItemsIs<int>(items => items.SequenceEqual(new [] {1, 2, 3}))
                .Called(1);

            context.Verify(p => p.CreateNewListT)
                .WhereItemsIs<char>(items => items.SequenceEqual(new[] {'a', 'b', 'c'}))
                .Called(1);

            context.Verify(p => p.CreateNewListT)
                .Called(2);
        }

        [Test]
        public void Generic_methods_with_mutliple_nested_type_arguments_can_be_setuped()
        {
            var l = new List<Tuple<string, int>>() { new Tuple<string, int>("a", 5) };
            var sut = new ItemBuilder<IGenericMethods>()
                .With(p => p.ComplicatedGenericTK.Value(l))
                .Build();

            var t = sut.ComplicatedGeneric<string, int>(Task.FromResult<IList<int>>(new List<int>()));
            t.Should().BeEquivalentTo(l);
        }

        [Test]
        public void Generic_methods_with_mutliple_nested_type_arguments_can_be_verified()
        {
            var sut = new EmptyIGenericMethodsBuilder()
                .Build(out var context);

            sut.ComplicatedGeneric<string, int>(Task.FromResult<IList<int>>(new List<int>()));

            context.Verify(p => p.ComplicatedGenericTK)
                .WhereTaskIs<int>(t => t.Result.Count == 0)
                .Called(1);
        }

        [Test]
        public void Bla()
        {
            var sut = new EmptyIGenericMethodsBuilder()
                .Build(out var context);

            sut.MoreThanOneParam<string, float>(("a", 3f), null, 3);

            context.Verify(p => p.MoreThanOneParamTK)
                .WhereAIs(("a", 3f))
                .WhereBIs<string>(s => s == null)
                .WhereCIs(3)
                .Called(1);
        }
    }
}
