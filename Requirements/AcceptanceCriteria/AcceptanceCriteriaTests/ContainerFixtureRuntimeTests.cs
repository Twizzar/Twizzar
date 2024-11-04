using System;
using System.Collections.Generic;
using System.Linq;

using DemoCode.Car;
using DemoCode.Interfaces;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Twizzar.Fixture;

namespace AcceptanceCriteriaTests
{
    [TestFixture]
    [Category("Container")]
    [Category("AcceptanceTest")]
    public class ContainerFixtureRuntimeTests
    {
        [Test]
        public void Container_returns_a_mock_of_an_interface_when_requesting_an_interface()
        {
            // Act
            var emailGateway = new ItemBuilder<IEmailGateway>().Build();

            // Assert
            var mock = Mock.Get(emailGateway);
            mock.Should().BeOfType<Mock<IEmailGateway>>()
                .And.NotBeNull();
        }

        [Test]
        [Category("PBI 663")]
        public void Container_AcceptanceCriteria663_CtorParametersOfBaseTypeWithDefault()
        {
            // act
            var email = new ItemBuilder<EmailMessage>().Build();

            // assert
            email.IsImportant.Should().BeTrue();
            email.ToAddress.Should().NotBeNullOrEmpty();
            email.CcAddress.Should().NotBeNullOrEmpty();
            email.BccAddress.Should().NotBeNullOrEmpty();
            email.MessageBody.Should().NotBeNullOrEmpty();
            email.Format.Should().NotBeNull();
        }

        [Test]
        [Category("PBI 716")]
        public void Container_AcceptanceCriteria716_GetInstancesReturnsNotNull()
        {
            // act
            var passengers = new ItemBuilder<IPassenger>().BuildMany(3).ToList();

            // assert
            Assert.IsNotNull(passengers);
        }

        [Test]
        public void Container_GetInstances_ReturnsDefaultType()
        {
            // act
            var passengers = new ItemBuilder<IPassenger>().BuildMany(2).ToList(); ;

            // assert
            passengers.Select(p => 
                p.Should().BeAssignableTo(typeof(IPassenger)));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public void Container_AcceptanceCriteria716_GetInstancesCorrectCount(int count)
        {
            // act
            var passengers = new ItemBuilder<IPassenger>().BuildMany(count).ToList();

            // assert
            Assert.AreEqual(count, passengers.Count());
        }

        [Test]
        public void Container_AcceptanceCriteria716_GetInstancesNegativeCount()
        {
            // act 
            Action act = () => new ItemBuilder<IPassenger>().BuildMany(-1).ToList();
            
            // assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Container_AcceptanceCriteria716_GetInstancesReturnTypeIsIEnumerable()
        {
            // act
            var passengers = new ItemBuilder<IPassenger>().BuildMany(3).ToList();

            // assert
            passengers.Should().BeAssignableTo(typeof(IEnumerable<IPassenger>));
        }

        [Test]
        public void Container_AcceptanceCriteria1564_GetInstanceOfArray()
        {
            // act
            var emptyIntArray = new ItemBuilder<int[]>().Build();
            var emptyStringArray = new ItemBuilder<string[]>().Build();
            var emptyInterfaceArray = new ItemBuilder<IPassenger[]>().Build();
            var emptyClassArray = new ItemBuilder<Car[]>().Build();

            // assert
            emptyIntArray.Should().HaveCount(0);
            emptyStringArray.Should().HaveCount(0);
            emptyInterfaceArray.Should().HaveCount(0);
            emptyClassArray.Should().HaveCount(0);
        }
    }
}
