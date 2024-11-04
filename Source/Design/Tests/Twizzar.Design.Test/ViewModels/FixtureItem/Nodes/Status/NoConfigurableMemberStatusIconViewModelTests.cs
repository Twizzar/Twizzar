using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twizzar.Design.Ui.Interfaces.Services;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes.Status;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;

namespace Twizzar.Design.Test.ViewModels.FixtureItem.Nodes.Status;

[TestClass]
public class NoConfigurableMemberStatusIconViewModelTests
{
    [TestMethod]
    public void All_ctor_parameter_throw_argumentNullException_when_null()
    {
        Verify.Ctor<NoConfigurableMemberStatusIconViewModel>()
            .ShouldThrowArgumentNullException();
    }

    [TestMethod]
    public void Ctor_removes_other_items_of_same_type()
    {
        // arrange
        var panel = new Mock<IStatusPanelViewModel>();
        var icons = new ObservableCollection<IStatusIconViewModel>();

        panel.Setup(model => model.Icons)
            .Returns(icons);

        var item1 = new NoConfigurableMemberStatusIconViewModel(
            panel.Object, 
            Build.New<IImageProvider>());
        icons.Add(item1);

        //act
        new NoConfigurableMemberStatusIconViewModel(panel.Object, Build.New<IImageProvider>());

        // assert
        panel.Verify(model => model.Remove(item1), Times.Once);
    }
}