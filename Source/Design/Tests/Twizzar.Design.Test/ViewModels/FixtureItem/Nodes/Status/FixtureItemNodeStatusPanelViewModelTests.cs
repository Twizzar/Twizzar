using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.TestCommon;

namespace Twizzar.Design.Test.ViewModels.FixtureItem.Nodes.Status;

[TestClass()]
public class FixtureItemNodeStatusPanelViewModelTests
{
    [TestMethod]
    public void Add_adds_the_icon_to_the_collection()
    {
        // arrange
        var sut = new StatusPanelViewModel();
        var item = Build.New<IStatusIconViewModel>();

        // act
        sut.Add(item);

        // assert
        sut.Icons.Should().HaveCount(1);
        sut.Icons.Should().Contain(item);
    }

    [TestMethod]
    public void Remove_removes_the_icon_from_the_collection()
    {
        // arrange
        var sut = new StatusPanelViewModel();
        var item1 = Build.New<IStatusIconViewModel>();
        var item2 = Build.New<IStatusIconViewModel>();
        sut.Add(item1);
        sut.Add(item2);

        // act
        sut.Remove(item1);

        // assert
        sut.Icons.Should().HaveCount(1);
        sut.Icons.Should().Contain(item2);
        sut.Icons.Should().NotContain(item1);
    }
}