using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media.Imaging;
using Twizzar.Design.Infrastructure.VisualStudio.Ui;
using Twizzar.Design.Ui.Interfaces.Messaging.VsEvents;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Enums;

namespace Twizzar.Design.Ui.View
{
    /// <summary>
    /// Interaction logic for FixtureMemberImage.xaml.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class FixtureMemberImage
    {
        #region static fields and constants

        /// <summary>
        /// Using a DependencyProperty as the backing store for MemberModifier.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MemberModifierProperty =
            DependencyProperty.Register(
                "MemberModifier",
                typeof(MemberModifier),
                typeof(FixtureMemberImage),
                new PropertyMetadata(MemberModifier.NotDefined, UpdateImageSource));

        /// <summary>
        /// Using a DependencyProperty as the backing store for MemberModifier.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MemberKindProperty =
            DependencyProperty.Register(
                "MemberKind",
                typeof(MemberKind),
                typeof(FixtureMemberImage),
                new PropertyMetadata(MemberKind.NotDefined, UpdateImageSource));

        #endregion

        #region fields

        private readonly IconThemer _iconThemer = new();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureMemberImage"/> class.
        /// </summary>
        public FixtureMemberImage()
        {
            this.InitializeComponent();
            this.DataContextChanged += this.OnDataContextChanged;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the MemberModifier.
        /// </summary>
        public MemberModifier MemberModifier
        {
            get => (MemberModifier)this.GetValue(MemberModifierProperty);
            set => this.SetValue(MemberModifierProperty, value);
        }

        /// <summary>
        /// Gets or sets the MemberKind.
        /// </summary>
        public MemberKind MemberKind
        {
            get => (MemberKind)this.GetValue(MemberKindProperty);
            set => this.SetValue(MemberKindProperty, value);
        }

        private IFixtureItemNodeViewModel NodeViewModel => this.DataContext as IFixtureItemNodeViewModel;

        #endregion

        #region members

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.NodeViewModel?.UiEventHub?.Unsubscribe<VsThemeChangedEvent>(this, this.UpdateImageSource);
            this.NodeViewModel?.UiEventHub?.Subscribe<VsThemeChangedEvent>(this, this.UpdateImageSource);
        }

        private void UpdateImageSource(VsThemeChangedEvent obj)
        {
            this.UpdateImageSource();
        }

        private static void UpdateImageSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FixtureMemberImage control)
            {
                control.UpdateImageSource();
            }
        }

        private void UpdateImageSource()
        {
            switch (this.MemberKind)
            {
                case MemberKind.NotDefined: break;
                case MemberKind.Property:
                    this.SetImageSourceForProperty();
                    break;
                case MemberKind.Field:
                    this.SetImageSourceForField();
                    break;
                case MemberKind.Method:
                    this.SetImageSourceForMethod();
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void SetImageSourceForMethod()
        {
            switch (this.MemberModifier)
            {
                case MemberModifier.NotDefined:
                case MemberModifier.Internal:
                case MemberModifier.Public:
                    this.SetImageSource(@"Method/Method_16x.png");
                    break;
                case MemberModifier.Protected:
                    this.SetImageSource(@"MethodProtect/MethodProtect_16x.png");
                    break;
                case MemberModifier.Private:
                    this.SetImageSource(@"MethodPrivate/MethodPrivate_16x.png");
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void SetImageSourceForField()
        {
            switch (this.MemberModifier)
            {
                case MemberModifier.Internal:
                    this.SetImageSource(@"FieldInternal/FieldInternal_16x.png");
                    break;
                case MemberModifier.NotDefined:
                case MemberModifier.Public:
                    this.SetImageSource(@"Field/Field_16x.png");
                    break;
                case MemberModifier.Protected:
                    this.SetImageSource(@"FieldProtected/FieldProtected_16x.png");
                    break;
                case MemberModifier.Private:
                    this.SetImageSource(@"FieldPrivate/FieldPrivate_16x.png");
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void SetImageSourceForProperty()
        {
            switch (this.MemberModifier)
            {
                case MemberModifier.NotDefined:
                case MemberModifier.Internal:
                case MemberModifier.Public:
                    this.SetImageSource(@"Property/Property_16x.png");
                    break;
                case MemberModifier.Protected:
                    this.SetImageSource(@"PropertyProtect/PropertyProtect_16x.png");
                    break;
                case MemberModifier.Private:
                    this.SetImageSource(@"PropertyPrivate/PropertyPrivate_16x.png");
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void SetImageSource(string filePath)
        {
            var uri = GetUri(filePath);
            var bitmap = new BitmapImage(uri);
            var wBitmap = new WriteableBitmap(bitmap);
            this._iconThemer.ThemeIcon(wBitmap);

            this.Image.Source = wBitmap;
        }

        private static Uri GetUri(string filePath) =>
            new($"pack://application:,,,/Twizzar.Design.Ui;component/View/Images/{filePath}");

        #endregion
    }
}