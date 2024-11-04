using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.Runtime.Test.UnitTest.FixtureItem.Builders;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.TypeDescription.Builders;

namespace Twizzar.Runtime.Test.AcceptanceTest
{
    [TestClass]
    public class CtorSelectorTest
    {
        private IMethodDescription CreateCtorDescription(params (Type Type, string Name)[] parameters) =>
            new MethodDescriptionBuilder()
                .AsConstructor()
                .WithDeclaredParameter(
                    parameters.Select(
                        tuple =>
                            new ParameterDescriptionBuilder()
                                .WithType(TypeFullName.Create(tuple.Type))
                                .WithName(tuple.Name)
                                .Build()))
                .Build();

        [TestMethod]
        public void CtorSelector_AcceptanceCriteria720_SameCtorAllTheTime()
        {
            // arrange
            var typeDescription = new RuntimeTypeDescriptionBuilder()
                .AsClass()
                .WithDeclaredConstructors(
                    this.CreateCtorDescription((typeof(int), "p1"), (typeof(string), "p2"), (typeof(TestClass), "p3"), (typeof(float), "p4")),
                    this.CreateCtorDescription((typeof(TestClass), "p3"), (typeof(float), "p4"), (typeof(string), "p5"))
                )
                .Build();
            var numberParams = typeof(TestClass).GetConstructors().Max(ctor => ctor.GetParameters().Length);

            var sut = new CtorSelector();

            // act
            var res = sut.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

            // assert
            Assert.IsNotNull(res);
            var methodDescription = res.GetSuccessUnsafe();

            Assert.AreEqual(numberParams,
                methodDescription.DeclaredParameters.Count());

            for (var i = 0; i < 100; i++)
            {
                var res2 = sut.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);
                var methodDescription2 = res2.GetSuccessUnsafe();
                methodDescription.DeclaredParameters.Should().BeEquivalentTo(methodDescription2.DeclaredParameters);
            }
        }
    }
#pragma warning disable IDE0060 // Remove unused parameter

    public class TestClass
    {
        public TestClass(int p1, string p2, TestClass p3, float p4)
        {
            
        }

        public TestClass(TestClass p3, float p4, int p1, string p2)
        {

        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
