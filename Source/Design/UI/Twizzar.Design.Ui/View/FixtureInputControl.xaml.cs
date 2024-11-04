using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Twizzar.Design.Ui.ViewModels.FixtureItem.Nodes;

namespace Twizzar.Design.Ui.View
{
    /// <summary>
    /// Interaction logic for FixtureInputControl.xaml.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class FixtureInputControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureInputControl"/> class.
        /// </summary>
        public FixtureInputControl()
        {
            this.InitializeComponent();

            var mediator = new Mediator.InputValueMediator(this.ValueRichTextBox, this.AutocompleteControl);
            this.ValueRichTextBox.InitializeMediator(mediator);
            this.AutocompleteControl.InitializeMediator(mediator);

            this.AutocompleteControl.Placement = PlacementMode.Relative;
            this.AutocompleteControl.PlacementTarget = this.ValueRichTextBox;
            this.AutocompleteControl.VerticalOffset = this.ValueRichTextBox.ActualHeight;

            this.Loaded += this.UserControlLoaded;
            this.IsVisibleChanged += this.FixtureInputControlIsVisibleChanged;
            this.Unloaded += this.UserControlUnloaded;
            this.DataContextChanged += this.OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ClearAndAddAdorner();
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            this.ClearAndAddAdorner();
        }

        private void ClearAndAddAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this.ValueRichTextBox);

            if (adornerLayer == null)
            {
                return;
            }

            this.ClearAdorners(adornerLayer);
            this.AddAdorner(adornerLayer);
        }

        private void UserControlUnloaded(object sender, RoutedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this.ValueRichTextBox);
            this.ClearAdorners(adornerLayer);
        }

        private void FixtureInputControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this.ValueRichTextBox);
            var adorners = adornerLayer?.GetAdorners(this.ValueRichTextBox);

            if (adorners != null)
            {
                foreach (var adorner in adorners)
                {
                    adorner.Visibility = this.IsVisible ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }

        private void AddAdorner(AdornerLayer adornerLayer)
        {
            var vm = this.DataContext as FixtureItemNodeViewModel;
            var adorner = new FixtureItemValueRichTextBoxAdorner(this.ValueRichTextBox, vm?.Value);
            adornerLayer.Add(adorner);
        }

        private void ClearAdorners(AdornerLayer adornerLayer)
        {
            var adorners = adornerLayer?.GetAdorners(this.ValueRichTextBox);

            if (adorners != null)
            {
                foreach (var adorner in adorners)
                {
                    adornerLayer.Remove(adorner);
                }
            }
        }
    }
}