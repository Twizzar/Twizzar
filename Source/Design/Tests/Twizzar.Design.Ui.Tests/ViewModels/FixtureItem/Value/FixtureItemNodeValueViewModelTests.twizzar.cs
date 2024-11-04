using Moq;
using Twizzar.Design.CoreInterfaces.Common.Util;
using Twizzar.Design.Ui.Interfaces.Controller;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Value;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Ui.Tests.ViewModels.FixtureItem.Value;

partial class FixtureItemNodeValueViewModelTests
{
    private class FixtureItemNodeValueViewModel28c7Builder : ItemBuilder<FixtureItemNodeValueViewModel, FixtureItemNodeValueViewModel28c7BulderPaths>
    {
        public FixtureItemNodeValueViewModel28c7Builder()
        {
            this.With(p => p.Ctor.uiEventHub.Stub<IUiEventHub>());
            this.With(p => p.FixtureNodeVM.Stub<IFixtureItemNodeViewModel>());
            this.With(p => p.FixtureNodeVM.IsDisposed.Value(false));
        }
    }

    private class FixtureItemNodeValueViewModelf18cBuilder : ItemBuilder<FixtureItemNodeValueViewModel, FixtureItemNodeValueViewModelf18cBuilderPaths>
    {
        public FixtureItemNodeValueViewModelf18cBuilder()
        {
            this.With(p => p.Ctor.uiEventHub.Stub<IUiEventHub>());
            this.With(p => p.FixtureNodeVM.Stub<IFixtureItemNodeViewModel>());
            this.With(p => p.FixtureNodeVM.IsDisposed.Value(true));
        }
    }

    private class FixtureItemNodeValueViewModel414eBuilder : ItemBuilder<FixtureItemNodeValueViewModel, FixtureItemNodeValueViewModel414eBuilderPaths>
    {
        public FixtureItemNodeValueViewModel414eBuilder()
        {
            var nodeMock = new Mock<IFixtureItemNode>();
            nodeMock.Setup(x => x.NodeValueController)
                .Returns(Mock.Of<IFixtureItemNodeValueController>());
            nodeMock.Setup(node => node.Parent)
                .Returns(
                    Maybe.Some(
                        Mock.Of<IFixtureItemNode>(node => node.NodeValueController == Mock.Of<IFixtureItemNodeValueController>())));
                
            var viewModelNode = nodeMock.As<IFixtureItemNodeViewModel>();
            viewModelNode.Setup(node => node.IsDisposed).Returns(true);

            this.With(p => p.FixtureNodeVM.Value(viewModelNode.Object));
        }
    }

    private class IFixtureItemNode33a3Builder : ItemBuilder<IFixtureItemNode, IFixtureItemNode33a3BuilderPaths>
    {
        public IFixtureItemNode33a3Builder()
        {
            this.With(p=> p.NodeValueController.Stub<IFixtureItemNodeValueController>());
        }
    }
}