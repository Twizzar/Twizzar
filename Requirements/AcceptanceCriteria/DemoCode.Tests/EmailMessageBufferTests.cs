using System;

using DemoCode.Interfaces;

using Moq;

using NUnit.Framework;
using Twizzar.Fixture;

namespace DemoCode.Tests
{
    public partial class EmailMessageBufferTests
    {
        [Test]
        public void Ctor_throws_argument_null_exception()
        {
            Verify.Ctor<EmailMessageBuffer>()
                .IgnoreParameter("logger")
                .ShouldThrowArgumentNullException();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(15)]
        public void Add_mail_to_message_buffer(int n)
        {
            // arrange
            var sut = new ItemBuilder<EmailMessageBuffer>().Build();

            // act
            foreach (var mail in new ItemBuilder<EmailMessage>().BuildMany(n))
            {
                sut.Add(mail);
            }

            // assert
            Assert.AreEqual(n, sut.UnsentMessagesCount);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(15)]
        [TestCase(500)]
        public void Send_mail_from_message_buffer(int n)
        {
            // arrange
            var sut = new DefaultEmailMessageBufferBuilder()
                .Build(out var scope);

            foreach (var mail in new ItemBuilder<EmailMessage>().BuildMany(n))
            {
                sut.Add(mail);
            }

            // act
            sut.SendAll();

            // assert
            Assert.AreEqual(0, sut.UnsentMessagesCount);
            scope.GetAsMoq(p => p.Ctor.validationService)
                .Verify(vs => vs.Validate(It.IsAny<EmailMessage>()), Times.Exactly(n));
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(15, 10)]
        public void Send_mail_from_message_buffer(int n, int sendN)
        {
            // arrange
            var sut = new DefaultEmailMessageBufferBuilder()
                .With(p => p.Ctor.validationService.Validate.Value(true))
                .Build(out var scope);

            foreach (var mail in new ItemBuilder<EmailMessage>().BuildMany(n))
            {
                sut.Add(mail);
            }

            // act
            sut.SendLimited(sendN);

            // assert
            Assert.AreEqual(n - sendN, sut.UnsentMessagesCount);

            scope.GetAsMoq(p => p.Ctor.validationService)
                .Verify(vs => vs.Validate(It.IsAny<EmailMessage>()), Times.Exactly(sendN));
        }

        [Test]
        public void Send_invalid_mail_throws_exception()
        {
            // arrange
            var validationService = new ItemBuilder<IValidationService>()
                .With(p => p.Validate.Value(false))
                .Build();

            var emailMessageBuffer = new ItemBuilder<EmailMessageBuffer>()
                .With(p => p.Ctor.validationService.Validate.Value(true))
                .With(p => p.Ctor.validationService.Value(validationService))
                .Build();

            var sut = new ItemBuilder<EmailMessageBuffer>()
                .With(p => p.Ctor.validationService.Value(validationService))
                .Build();

            var invalidMail = new ItemBuilder<EmailMessage>().Build();
            sut.Add(invalidMail);

            // act
            void Act() => sut.SendAll();

            // assert
            Assert.Throws<ApplicationException>(Act);
        }
    }
}