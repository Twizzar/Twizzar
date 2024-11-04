using DemoCode.Interfaces;

using NUnit.Framework;

using Twizzar.Fixture;

namespace DemoCode.Tests
{
    public partial class ValidationServiceTests
    {
        [Test]
        public void Ctor_throws_argument_null_exception()
        {
            Verify.Ctor<ValidationService>().ShouldThrowArgumentNullException();
        }

        [Test]
        public void Validate_invalid_Email_returns_false()
        {
            // arrange
            var sut = new ItemBuilder<ValidationService>().Build();

            var spamMail = new ValidEmailMessageBuilder()
                .With(p => p.MessageBody.Value("spam"))
                .Build();

            var buyMail = new ValidEmailMessageBuilder()
                .With(p => p.MessageBody.Value("buy"))
                .Build();

            var invalidHeader = new ValidEmailMessageBuilder()
                .With(p => p.Header.Value("Invalid Header"))
                .Build();

            var emptyAddress = new ValidEmailMessageBuilder()
                .With(p => p.ToAddress.Value(string.Empty))
                .Build();

            var emptySubject = new ValidEmailMessageBuilder()
                .With(p => p.Subject.Value(string.Empty))
                .Build();

            var allAddress = new ValidEmailMessageBuilder()
                .With(p => p.CcAddress.Value("all@company.com"))
                .Build();

            // act
            var spamResult = sut.Validate(spamMail);
            var buyResult = sut.Validate(buyMail);
            var invalidHeaderResult = sut.Validate(invalidHeader);
            var emptyAddressResult = sut.Validate(emptyAddress);
            var emptySubjectResult = sut.Validate(emptySubject);
            var allAddressResult = sut.Validate(allAddress);

            // assert
            Assert.IsFalse(spamResult);
            Assert.IsFalse(buyResult);
            Assert.IsFalse(invalidHeaderResult);
            Assert.IsFalse(emptyAddressResult);
            Assert.IsFalse(emptySubjectResult);
            Assert.IsFalse(allAddressResult);
        }


        [TestCaseSource(nameof(_invalidMailConfigs))]
        public void Validate_invalid_Email_returns_false_test_cases(ItemBuilderBase<EmailMessage> builder)
        {
            // arrange
            var sut = new ItemBuilder<ValidationService>().Build();
            var spamMail = builder.Build();
            
            // act
            var spamResult = sut.Validate(spamMail);
            
            // assert
            Assert.IsFalse(spamResult);
        }

        public static readonly object[] _invalidMailConfigs =
        {
            new object[] { new ValidEmailMessageBuilder().With(p => p.MessageBody.Value("spam")) },
            new object[] { new ValidEmailMessageBuilder().With(p => p.MessageBody.Value("buy")) },
            new object[] { new ValidEmailMessageBuilder().With(p => p.Header.Value("Invalid Header")) },
            new object[] { new ValidEmailMessageBuilder().With(p => p.ToAddress.Value(string.Empty)) },
            new object[] { new ValidEmailMessageBuilder().With(p => p.Subject.Value(string.Empty)) },
            new object[] { new ValidEmailMessageBuilder().With(p => p.CcAddress.Value("all@company.com")) },
        };
    }
}