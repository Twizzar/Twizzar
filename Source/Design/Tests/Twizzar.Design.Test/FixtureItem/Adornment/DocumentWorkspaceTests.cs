using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.FixtureItem.Adornment;

[TestClass]
public class DocumentWorkspaceTests
{
    #region members

    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        // assert
        Verify.Ctor<DocumentWorkspace>()
            .ShouldThrowArgumentNullException();
    }

    #endregion
}