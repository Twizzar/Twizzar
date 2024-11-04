using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.SharedKernel.Core.FixtureItem.Configuration.Services;
using Twizzar.SharedKernel.CoreInterfaces.Failures;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon.TypeDescription.Builders;
using static Twizzar.TestCommon.TestHelper;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Twizzar.SharedKernel.Core.Tests.FixtureItem
{
    [TestFixture]
    public class CtorSelectorTests
    {
        private readonly CtorSelector _sut = new CtorSelector();
        private IMethodDescription _minCtorDescription;
        private IMethodDescription _maxCtorDescription;

        [Test]
        public void CtorSelector_MaxBehavior_GetCtorDescription_Successful()
        {
            // arrange
            var typeDescription = this.SetupTypeDescription();

            // act
            var res = this._sut.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);
            var res2 = this._sut.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

            // assert
            Assert.IsNotNull(res);
            Assert.AreEqual(this._maxCtorDescription.DeclaredParameters.Length, AssertAndUnwrapSuccess(res).DeclaredParameters.Length);

            AssertAndUnwrapSuccess(res).DeclaredParameters.Should()
                .BeEquivalentTo(AssertAndUnwrapSuccess(res2).DeclaredParameters);
        }

        [Test]
        public void CtorSelector_MinBehavior_GetCtorDescription_Successful()
        {
            // arrange
            var typeDescription = this.SetupTypeDescription();

            // act
            var res = this._sut.GetCtorDescription(typeDescription, CtorSelectionBehavior.Min);
            var res2 = this._sut.GetCtorDescription(typeDescription, CtorSelectionBehavior.Min);

            // assert
            Assert.IsNotNull(res);
            Assert.AreEqual(this._minCtorDescription.DeclaredParameters.Length, AssertAndUnwrapSuccess(res).DeclaredParameters.Length);

            AssertAndUnwrapSuccess(res).DeclaredParameters.Should()
                .BeEquivalentTo(AssertAndUnwrapSuccess(res2).DeclaredParameters);
        }

        [Test]
        public void CtorSelector_GetCtorDescription_ArgumentNullException()
        {
            // arrange
            Action act = () => this._sut.GetCtorDescription(null, CtorSelectionBehavior.Max);
            
            // act
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CtorSelector_GetCtorDescription_InvalidTypeDescriptionException()
        {
            // arrange
            var typeDescription = new TypeDescriptionBuilder().AsInterface().Build();

            // act
            var result = this._sut.GetCtorDescription(typeDescription, CtorSelectionBehavior.Max);

            //assert
            result.IsFailure.Should().BeTrue();
            result.GetFailureUnsafe().Should().BeOfType<InvalidTypeDescriptionFailure>();
        }

        [TestCase(CtorSelectionBehavior.Custom)]
        public void CtorSelector_GetCtorDescription_NotImplementedException(CtorSelectionBehavior behavior)
        {
            // arrange
            var typeDescription = new TypeDescriptionBuilder().WithIsClass(true).Build();

            // act
            this._sut.GetCtorDescription(typeDescription, behavior).IsFailure.Should().BeTrue();
        }

        private ITypeDescription SetupTypeDescription()
        {
            this._minCtorDescription = new MethodDescriptionBuilder()
                .WithDeclaredParameter(new List<IParameterDescription>()
                {
                    new ParameterDescriptionBuilder().WithType(Mock.Of<ITypeFullName>(name => name.FullName == "System.String")).Build(),
                })
                .WithAccessModifier(AccessModifier.CreatePublic())
                .Build();

            var ctor2 = new MethodDescriptionBuilder()
                .WithDeclaredParameter(new List<IParameterDescription>()
                {
                    new ParameterDescriptionBuilder().WithType(Mock.Of<ITypeFullName>(name => name.FullName == "System.Int32")).Build(),
                    new ParameterDescriptionBuilder().WithType(Mock.Of<ITypeFullName>(name => name.FullName == "System.String")).Build(),
                })
                .WithAccessModifier(AccessModifier.CreatePublic())
                .Build();

            this._maxCtorDescription = new MethodDescriptionBuilder()
                .WithDeclaredParameter(new List<IParameterDescription>()
                {
                    new ParameterDescriptionBuilder().WithType(Mock.Of<ITypeFullName>(name => name.FullName == "System.Int32")).Build(),
                    new ParameterDescriptionBuilder().WithType(Mock.Of<ITypeFullName>(name => name.FullName == "System.String")).Build(),
                    new ParameterDescriptionBuilder().WithType(Mock.Of<ITypeFullName>(name => name.FullName == "System.String")).Build(),
                })
                .WithAccessModifier(AccessModifier.CreatePublic())
                .Build();

            var typeDescription = new TypeDescriptionBuilder()
                .WithIsClass(true)
                .WithDeclaredConstructors(this._minCtorDescription, ctor2, this._maxCtorDescription)
                .Build();

            return typeDescription;
        }
    }
}
