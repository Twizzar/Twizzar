using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.FixtureItem.Adornment;

[TestClass]
public class ViAdornmentCreatorTests
{
    #region members

    [TestMethod]
    public void Ctor_throws_ArgumentNullException_when_Arguments_are_null()
    {
        ViAdornmentCreator.ViAdornmentFactory factory = (_, _, _, _) => default;

        // assert
        Verify.Ctor<ViAdornmentCreator>()
            .SetupParameter("viAdornmentViAdornmentFactory", factory)
            .ShouldThrowArgumentNullException();
    }

    #endregion
}