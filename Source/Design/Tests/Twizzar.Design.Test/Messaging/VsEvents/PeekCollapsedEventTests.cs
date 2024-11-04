using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.Messaging.VsEvents;

[TestClass]
public class PeekCollapsedEventTests
{
    [TestMethod]
    public void Ctor_test()
    {
        // assert
        Verify.Ctor<PeekCollapsedEvent>()
            .SetupParameter("adornmentId", AdornmentId.CreateNew(TestHelper.RandomString()))
            .ShouldThrowArgumentNullException();
    }
}