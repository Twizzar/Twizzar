using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.VisualStudio.Services;
using Twizzar.TestCommon;

namespace Twizzar.Design.Test.UnitTest;

[TestClass()]
public class VsEventCacheTests
{
    #region members

    [TestMethod()]
    public void RegisterAllReferencesLoaded_test()
    {
        // arrange 
        var projectName = TestHelper.RandomString();
        var viProject = new Mock<IViProject>();
        viProject.Setup(project => project.Name)
            .Returns(projectName);

        var sut = new VsEventCache();

        // act
        sut.RegisterAllReferencesLoaded(viProject.Object);
            
        // assert
        sut.AllReferencesAreLoaded(projectName).Should().BeTrue();
    }

    [TestMethod()]
    public void RegisterProjectUnloaded_test()
    {
        // arrange 
        var projectName = TestHelper.RandomString();
        var sut = new VsEventCache();
        var viProject = new Mock<IViProject>();
        viProject.Setup(project => project.Name)
            .Returns(projectName);

        // act
        sut.RegisterAllReferencesLoaded(viProject.Object);
        sut.RegisterProjectUnloaded(viProject.Object);

        // assert
        sut.AllReferencesAreLoaded(projectName).Should().BeFalse();
    }

    #endregion
}