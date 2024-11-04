using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

using Twizzar.Fixture;
using Twizzar.Fixture.Member;
using static Twizzar.Fixture.Member.ItemMemberConfigFactory;

namespace Twizzar.Api.Tests.Fixture
{

    public class CarPathDummyProvider : PathProvider<Car>
    {
        public MemberPath<Car, int> NumberOfSeats => new PropertyBasetypeMemberPath<Car, int>(nameof(Car.NumberOfSeats), RootPath);

        public EngineMemberPath Engine => new(RootPath);

        public ConstructorMemberPath Ctor => new ConstructorMemberPath(RootPath);

        public class ConstructorMemberPath
        {
            private MemberPath<Car> TzParent;
            public ConstructorMemberPath(MemberPath<Car> parent)
            {
                this.TzParent = parent;
            }

            public MemberPath<Car, int> numberOfSeats =>
                new CtorParamMemberPath<Car, int>("numberOfSeats", this.TzParent);

            public MemberPath<Car, IEngine> engine =>
                new CtorParamMemberPath<Car, IEngine>("engine", this.TzParent);
        }

        public class EngineMemberPath : PropertyMemberPath<Car, Engine>
        {
            /// <inheritdoc />
            public EngineMemberPath(MemberPath<Car> parent)
                : base("Engine", parent)
            {
            }

            public PropertyBasetypeMemberPath<Car, string> EngineName => new(nameof(Fixture.Engine.EngineName), this);
        }
    }

    public class IMethodTestPathDummyProvider : PathProvider<IMethodTest>
    {
        public PropertyBasetypeMemberPath<IMethodTest, int> MyNumber = new(nameof(IMethodTest.MyNumber), RootPath);

        public MethodBasetypeMemberPath<IMethodTest, IList<int>, IList<int>> GetNumber_System_Int32 = new("GetNumber", RootPath);
        public GetEngineMemberPath GetEngine_IEngine => new(RootPath);
        public GetEngineMemberPath_ICylinder GetEngine__Int32 => new(RootPath, TzParameter.Create("count", typeof(int)));

        public MethodBasetypeMemberPath<IMethodTest, TzVoid, TzVoid> VoidMethod = new(nameof(IMethodTest.VoidMethod),
            RootPath);

        public class GetEngineMemberPath : MethodMemberPath<IMethodTest, IEngine, IEngine>
        {
            public GetEngineMemberPath(MemberPath<IMethodTest> parent, params TzParameter[] parameters)
                : base("GetEngine", parent, parameters)
            { }

            public PropertyBasetypeMemberPath<IMethodTest, string> EngineName => new(nameof(IEngine.EngineName), this);
        }

        public class GetEngineMemberPath_ICylinder : MethodMemberPath<IMethodTest, ICylinder, ICylinder>
        {
            public GetEngineMemberPath_ICylinder(MemberPath<IMethodTest> parent, params TzParameter[] parameters)
                : base("GetEngine", parent, parameters)
            { }
        }

        public GetEngine_ICylinderMemberPath GetEngine_ICylinder => new(RootPath);

        public class GetEngine_ICylinderMemberPath : MethodMemberPath<IMethodTest, ICylinder, ICylinder>
        {
            public GetEngine_ICylinderMemberPath(MemberPath<IMethodTest> parent)
                : base("GetEngine", parent, TzParameter.Create("count", typeof(int)))
            {
            }
        }

        public GenericMethodTMemberPath GenericMethodT => new(RootPath);

        public class GenericMethodTMemberPath : MethodMemberPath<IMethodTest, object, object>
        {
            public GenericMethodTMemberPath(MemberPath<IMethodTest> parent)
                : base(nameof(IMethodTest.GenericMethod), parent, new[] { "T" })
            {
            }


        }

        public SimpleGenericMethodTMemberPath SimpleGenericMethodT => new(RootPath);

        public class SimpleGenericMethodTMemberPath : MethodMemberPath<IMethodTest, ICylinder, ICylinder>
        {
            public SimpleGenericMethodTMemberPath(MemberPath<IMethodTest> parent)
                : base(nameof(IMethodTest.SimpleGenericMethod), parent, new[] { "T" })
            {
            }

            public PropertyBasetypeMemberPath<IMethodTest, int> CylinderCount =>
                new(nameof(ICylinder.CylinderCount), this);
        }

        public ComplexGenericMethodTMemberPath ComplexGenericMethodT => new(RootPath);

        public class ComplexGenericMethodTMemberPath : MethodMemberPath<IMethodTest, object, object>
        {
            public ComplexGenericMethodTMemberPath(MemberPath<IMethodTest> parent)
                : base(nameof(IMethodTest.ComplexGenericMethod), parent, new[] { "T" }, TzParameter.Create("param", typeof(IList<>).GetGenericArguments()[0].MakeArrayType()))
            {
            }
        }
    }

    //public static class ItemBuilderExtensions
    //{
    //    public static ItemBuilder<Car> With(
    //        this ItemBuilder<Car> builder,
    //        Func<CarPathDummyProvider, MemberConfig<Car>> selector) =>
    //            ItemBuilderHelperMethods.With(builder, selector);

    //    public static (Car Car, IItemScope<Car, CarPathDummyProvider> ItemScope) BuildWithScope(
    //        this ItemBuilder<Car> builder) =>
    //        ItemBuilderHelperMethods.BuildWithScope<Car, CarPathDummyProvider>(builder);

    //    public static IEnumerable<(Car Car, IItemScope<Car, CarPathDummyProvider> ItemScope)> BuildManyWithScope(
    //        this ItemBuilder<Car> builder,
    //        int count) =>
    //        ItemBuilderHelperMethods.BuildManyWithScope<Car, CarPathDummyProvider>(builder, count);
    //}

    public class MyCarBuilder : ItemBuilder<Car, CarPathDummyProvider>
    {
        public MyCarBuilder()
        {
            //this.With(p => p.Engine.EngineName.Value("test"));
        }
    }

    public class IMethodTestBuildeer : ItemBuilder<IMethodTest, IMethodTestPathDummyProvider>
    {
        public IMethodTestBuildeer()
        {
            //this.With(p => p.GetEngine_IEngine.EngineName.Value("Test"));
        }
    }

    public class ItemBuilder_T1Tests
    {
        [Test]
        public void TestMethodSetup()
        {
            var path = new IMethodTestPathDummyProvider().GetEngine__Int32;
            var cylinder = new Cylinder();

            var sut = new ItemBuilder<IMethodTest>();
            sut.AddMemberConfig(Value(path, cylinder));
            var i = sut.Build();

            i.GetEngine(1).Should().Be(cylinder);
        }

        [Test]
        public void TestBuildMany()
        {
            // arrange
            var path = new CarPathDummyProvider().NumberOfSeats;

            // act
            var suts = new ItemBuilder<Car>()
                .AddMemberConfig(Value(path, 3))
                .BuildMany(3);

            // assert
            suts.All(car => car.NumberOfSeats == 3).Should().BeTrue();
        }

        [Test]
        public void TestContextGet()
        {
            // arrange
            var path = new CarPathDummyProvider().NumberOfSeats;

            // act
            var sut = new ItemBuilder<Car, CarPathDummyProvider>()
                .Build(out var context);

            // assert
            context.Get(p => p.Ctor.numberOfSeats)
                .Should().NotBe(0);

            context.Get(p => p.Ctor.engine).Should().NotBeNull();
        }

        [Test]
        public void Delegate_setup_is_returned_correctly()
        {
            // arrange
            var path = new IMethodTestPathDummyProvider().GetEngine_ICylinder;
            var cylinder = new Cylinder();

            // act
            var sut = new ItemBuilder<IMethodTest, IMethodTestPathDummyProvider>()
                .AddMemberConfig(ViDelegate(path, new Func<int, ICylinder>(i => cylinder)))
                .Build();

            // assert
            var result = sut.GetEngine(5);
            result.Should().Be(cylinder);
        }

        [Test]
        public void Callbacks_get_called_correctly_and_at_the_correct_time()
        {
            // arrange
            var path = new IMethodTestPathDummyProvider().GetEngine_ICylinder;
            var receivedCallbacks = new List<int>();

            // act
            var sut = new ItemBuilder<IMethodTest, IMethodTestPathDummyProvider>()
                .AddMemberConfig(ViCallback(path, new Action<int>(i => { receivedCallbacks.Add(i);})))
                .Build();

            // assert
            receivedCallbacks.Should().HaveCount(0);
            sut.GetEngine(5);
            receivedCallbacks.Should().HaveCount(1);
            receivedCallbacks.Should().Contain(5);
        }

        [Test]
        public void Configuration_of_generic_methods_works()
        {
            // arrange
            var path = new IMethodTestPathDummyProvider().SimpleGenericMethodT;
            var path2 = new IMethodTestPathDummyProvider().SimpleGenericMethodT.CylinderCount;
            
            // act
            var sut = new ItemBuilder<IMethodTest, IMethodTestPathDummyProvider>()
                .AddMemberConfig(ViType<IMethodTest, ICylinder>(path))
                .AddMemberConfig(Value(path2, 5))
                .Build();

            sut.SimpleGenericMethod<ICylinder>().CylinderCount.Should().Be(5);
        }

        //[Test]
        //public void Configured_CustomBuilder_returns_configured_instant_on_build()
        //{
        //    var car = new MyCarBuilder().Build();

        //    car.Engine.Should().BeOfType<Engine>();
        //    car.Engine.EngineName.Should().Be("test");
        //}

        //[Test]
        //public void May_returns_different_instance_with_same_content()
        //{
        //    var cars = new MyCarBuilder().BuildMany(5);

        //    cars.Should().OnlyHaveUniqueItems();
        //    cars.Select(car => car.Engine.EngineName).All(s => s == "test").Should().BeTrue();
        //}

        //[Test]
        //public void ItemBuilder_with_configurations_works()
        //{
        //    var car = new ItemBuilder<Car>()
        //        .With(provider => provider.NumberOfSeats.Value(3))
        //        .With(provider => provider.Engine.EngineName.Value("Test"))
        //        .Build();

        //    car.NumberOfSeats.Should().Be(3);
        //}

        //[Test]
        //public void Scope_get_returns_correct_dependency()
        //{
        //    new ItemBuilder<Car, CarPathProvider>()
        //        .With(p => p.NumberOfSeats.Value(3));

        //    var (_, scope) = new ItemBuilder<Car>()
        //        .With(provider => provider.NumberOfSeats.Value(5))
        //        .BuildWithScope();

        //    scope.Get(provider => provider.NumberOfSeats).Should().Be(5);
        //}

        //[Test]
        //public void MethodReturn_Type_is_correctly_set()
        //{
        //    var methodTest = new IMethodTestBuildeer().Build();

        //    methodTest.GetEngine().EngineName.Should().Be("Test");
        //}

        //[Test]
        //public void Scope_get_method_return_value_is_resolved_correctly()
        //{
        //    var (methodTest, scope) = new IMethodTestBuildeer().BuildWithScope();

        //    scope.Get(p => p.GetEngine_IEngine)
        //        .Should()
        //        .BeAssignableTo<IEngine>();
        //}

        //[Test]
        //public void MethodReturn_Type_scope_retrieves_the_correct_value()
        //{
        //    var (_, scope) = new IMethodTestBuildeer()
        //        .With(p => p.GetNumber_System_Int32.Value(new List<int>() { 5 }))
        //        .BuildWithScope();

        //    scope.Get(p => p.GetNumber_System_Int32)
        //        .Should()
        //        .OnlyContain(i => i == 5);
        //}
    }

    public class Car
    {
        #region ctors

        public Car(int numberOfSeats, IEngine engine)
        {
            this.NumberOfSeats = numberOfSeats;
            this.Engine = engine;
        }

        #endregion

        #region properties

        public int NumberOfSeats { get; set; }

        public IEngine Engine { get; set; }

        #endregion

        #region members

        public float Accelerate(float velocity) =>
            throw new NotImplementedException();

        #endregion
    }

    public interface IEngine
    {
        #region properties

        ICylinder Cylinder { get; }
        string EngineName { get; set; }

        #endregion

        #region members

        float Start(int securityNumber);

        string Start(string securityNumber);

        #endregion
    }

    public class Engine : IEngine
    {
        #region properties

        /// <inheritdoc />
        public ICylinder Cylinder { get; }

        /// <inheritdoc />
        public string EngineName { get; set; }

        public int Number { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public float Start(int securityNumber) =>
            throw new NotImplementedException();

        /// <inheritdoc />
        public string Start(string securityNumber) =>
            throw new NotImplementedException();

        #endregion
    }

    public interface ICylinder
    {
        #region properties

        int CylinderCount { get; }

        #endregion
    }

    public class Cylinder : ICylinder
    {
        #region properties

        /// <inheritdoc />
        public int CylinderCount { get; }

        #endregion
    }

    public interface IMethodTest
    {
        void VoidMethod();
        int MyNumber { get; }
        IList<int> GetNumber();
        IEngine GetEngine();
        ICylinder GetEngine(int count);

        T SimpleGenericMethod<T>();

        List<T> GenericMethod<T>();

        List<T> ComplexGenericMethod<T>(T[] param);
    }
}