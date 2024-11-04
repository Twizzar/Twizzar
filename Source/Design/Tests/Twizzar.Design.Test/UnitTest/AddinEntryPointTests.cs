using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Infrastructure.VisualStudio;

using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.UnitTest;

[TestClass]
public partial class AddinEntryPointTests
{
    [TestMethod]
    public void Ctor_parameters_throw_ArgumentNullException_when_null()
    {
        // assert
        Verify.Ctor<AddinEntryPoint>()
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void AddinEntryPointTest()
    {
        // arrange
        var solutionEventPublisher = new ISolutionEventsPublishefBuilder()
            .Build(out var eventPublisherContext);

        var sut = new AddinEntryPointBuilder()
            .With(p => p.Ctor.solutionEventsPublishers.Value(new[] { solutionEventPublisher }))
            .Build(out var context);

        // act
        sut.Start();

        // assert

        eventPublisherContext.Verify(p => p.Initialize)
            .Called(1);
        context.Verify(p => p.Ctor.vsColorThemeEventWrapper.Initialize)
            .Called(1);
        context.Verify(p => p.Ctor.unhandledExceptionsLogger.Initialize)
            .Called(1);
    }
}