using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Services;
using Twizzar.Runtime.Infrastructure.DomainService;
using Twizzar.Runtime.Test.IntegrationTest;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Runtime.Test.UnitTest;

[TestClass]
[TestCategory("Autofac")]
public class FixtureContainerTest
{
    [TestMethod]
    public void AutofacFixtureContainer_ctor_ArgumentNullException()
    {
        // assert
        Verify.Ctor<FixtureItemContainer>()
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void AutofacFixtureContainer_ctor_NotNull()
    {
        // arrange
        var resolver = new Mock<IResolver>().Object;

        // act
        var sut = new FixtureItemContainer(resolver, Build.New<IRegisteredCodeInstanceContainer>());

        // assert
        Assert.IsNotNull(sut);
    }

    [TestMethod]
    [DataRow(typeof(int), 1)]
    [DataRow(typeof(string), "test")]
    [DataRow(typeof(bool), true)]
    public void AutofacFixtureContainer_GetInstance_Successfully(Type type, object expectedValue)
    {
        // arrange
        var sut = new FixtureContainerBuilder()
            .AddTypeToResolve(type, expectedValue)
            .Build();

        // act
        var result = typeof(FixtureItemContainer)
            .GetMethod(nameof(sut.GetInstance), Array.Empty<Type>())
            ?.MakeGenericMethod(type)
            .Invoke(sut, Array.Empty<object>());

        // assert
        Assert.AreEqual(expectedValue, result);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(5)]
    public void AutofacFixtureContainer_GetInstancesCorrectCount(int count)
    {
        // arrange
        var sut = new FixtureContainerBuilder()
            .AddTypeToResolve(1)
            .Build();

        //act
        var sequence = sut.GetInstances<int>(count);

        //asserts
        Assert.AreEqual(count, sequence.Count());
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(-7)]
    [DataRow(int.MinValue)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void AutofacFixtureContainer_GetInstancesNegativeCount(int negativeCount)
    {
        // arrange
        var sut = new FixtureContainerBuilder()
            .AddTypeToResolve(1)
            .Build();

        // act
        sut.GetInstances<ITestInterface>(negativeCount);
    }

    [TestMethod]
    public void AutofacFixtureContainer_GetInstancesReturnTypeIsIEnumerable()
    {
        // arrange
        var sut = new FixtureContainerBuilder()
            .AddTypeToResolve(1)
            .Build();

        // act
        var sequence = sut.GetInstances<int>(3);

        // assert
        Assert.IsInstanceOfType(sequence, typeof(IEnumerable<int>));
    }

    [TestMethod]
    public void AutofacFixtureContainer_GetInstancesReturnsCorrectContent()
    {
        static void TestGeneric<T>(T expectedValue)
        {
            // arrange
            const int count = 3;
            var sut = new FixtureContainerBuilder()
                .AddTypeToResolve<T>(expectedValue)
                .Build();

            // act
            var result = sut.GetInstances<T>(count);

            // assert
            Assert.IsTrue(result.All(v => v.Equals(expectedValue)));
        }

        var random = new Random();
        TestGeneric<int>(random.Next());
        TestGeneric<string>(Guid.NewGuid().ToString());
        TestGeneric<double>(random.NextDouble());
        TestGeneric<int?>(null);
    }

    private class FixtureContainerBuilder
    {
        private readonly Mock<IResolver> _resolver;

        public FixtureContainerBuilder()
        {
            this._resolver = new Mock<IResolver>();
        }

        /// <summary>
        /// Add a type which should get resolved to a certain type.
        /// When the container.Resolve&lt;T &gt;() is called this type will be returned.
        /// </summary>
        /// <typeparam name="T">Type to add</typeparam>
        /// <param name="value">The value it gets resolved to.</param>
        /// <returns></returns>
        public FixtureContainerBuilder AddTypeToResolve<T>(T value)
        {
            this._resolver.Setup(resolver => resolver.Resolve<T>()).Returns(value);
            return this;
        }

        /// <summary>
        /// Add a named type which should get resolved to a certain type.
        /// When the container.Resolve&lt;T &gt;() is called this type will be returned.
        /// </summary>
        /// <typeparam name="T">Type to add</typeparam>
        /// <param name="value">The value it gets resolved to.</param>
        /// <param name="definitionId">The definition id.</param>
        /// <returns></returns>
        public FixtureContainerBuilder AddTypeToResolve<T>(T value, string definitionId)
        {
            this._resolver.Setup(resolver => resolver.ResolveNamed<T>(definitionId)).Returns(value);
            return this;
        }

        public FixtureContainerBuilder AddTypeToResolve(Type type, object value)
        {
            typeof(FixtureContainerBuilder)
                .GetMethods()
                .First(info => info.IsGenericMethod && info.Name == nameof(this.AddTypeToResolve))
                ?.MakeGenericMethod(type)
                .Invoke(this, new[] {value});

            return this;
        }

        public IFixtureItemContainer Build() =>
            new FixtureItemContainer(
                this._resolver.Object,
                new ItemBuilder<IRegisteredCodeInstanceContainer>().Build());
    }
}