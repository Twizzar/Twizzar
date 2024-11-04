using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.ViewModels.FixtureItem.Nodes.Status;

[TestClass]
public class ErrorStatusIconViewModelTests
{
    [TestMethod]
    public void All_ctor_parameter_throw_argumentNullException_when_null()
    {
        // assert
        Verify.Ctor<ErrorStatusIconViewModel>()
            .SetupParameter("message", TestHelper.RandomString())
            .ShouldThrowArgumentNullException();
    }
}