using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Runtime.Core.FixtureItem.Definition.MemberDefinitions;
using Twizzar.Runtime.Core.FixtureItem.Definition.ValueDefinitions;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Definition.ValueDefinitions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.MemberConfigurations;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Runtime.Test.UnitTest.FixtureItem.Definition
{
    [TestClass]
    public class MemberDefinitionTest
    {
        private static readonly IConfigurationSource Source = Mock.Of<IConfigurationSource>();

        public class DummyMemberDefinition : MemberDefinition
        {
            #region Overrides of ValueObject

            public static IValueDefinition CreateValueDefinitionProxy(
                IMemberConfiguration memberConfiguration,
                Type type) =>
                CreateValueDefinition(memberConfiguration, type);

            /// <inheritdoc />
            protected override IEnumerable<object> GetEqualityComponents() =>
                throw new NotImplementedException();

            #endregion

            #region Overrides of MemberDefinition

            /// <inheritdoc />
            public override IValueDefinition ValueDefinition { get; }

            #endregion
        }

        [TestMethod]
        public void CreateValueDefinition_LinkMemberConfiguration_maps_to_LinkDefinition()
        {
            // arrange
            var memberName = RandomString();
            var link = RandomNamedFixtureItemId();

            // act
            var valueDefinition = DummyMemberDefinition.CreateValueDefinitionProxy(
                new LinkMemberConfiguration(memberName, link, Source),
                typeof(MemberDefinitionTest));

            // assert
            var definition = valueDefinition.Should().BeOfType<LinkDefinition>().Subject;
            definition.Link.Should().Be(link);
        }

        [TestMethod]
        public void CreateValueDefinition_UndefinedMemberConfiguration_maps_to_UndefinedDefinition()
        {
            // arrange
            var memberName = RandomString();
            var typeFullName = RandomTypeFullName();

            // act
            var valueDefinition = DummyMemberDefinition.CreateValueDefinitionProxy(
                new UndefinedMemberConfiguration(memberName, typeFullName, Source), typeof(MemberDefinitionTest));

            // assert
            valueDefinition.Should().BeOfType<UndefinedDefinition>();
        }

        [TestMethod]
        public void CreateValueDefinition_ValueMemberConfiguration_maps_to_RawValueDefinition()
        {
            // arrange
            var memberName = RandomString();
            var value = RandomString();

            // act
            var valueDefinition = DummyMemberDefinition.CreateValueDefinitionProxy(
                new ValueMemberConfiguration(memberName, value, Source), typeof(MemberDefinitionTest));

            // assert
            var definition = valueDefinition.Should().BeOfType<RawValueDefinition>().Subject;
            definition.Value.Should().Be(value);
        }

        [TestMethod]
        public void CreateValueDefinition_UniqueValueMemberConfiguration__maps_to_UniqueDefinition()
        {
            // arrange
            var memberName = RandomString();

            // act
            var valueDefinition = DummyMemberDefinition.CreateValueDefinitionProxy(
                new UniqueValueMemberConfiguration(memberName, Source), typeof(MemberDefinitionTest));

            // assert
            valueDefinition.Should().BeOfType<UniqueDefinition>();
        }

        [TestMethod]
        public void CreateValueDefinition_NullValueMemberConfiguration__maps_to_NullValueDefinition()
        {
            // arrange
            var memberName = RandomString();

            // act
            var valueDefinition = DummyMemberDefinition.CreateValueDefinitionProxy(
                new NullValueMemberConfiguration(memberName, Source), typeof(MemberDefinitionTest));

            // assert
            valueDefinition.Should().BeOfType<NullValueDefinition>();
        }
    }
}
