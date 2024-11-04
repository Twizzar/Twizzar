using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace Twizzar.Design.Ui.MultiKey
{
    /// <summary>
    /// Multi key shortcut binding.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [TypeConverter(typeof(MultiKeyGestureConverter))]
    public class MultiKeyBinding : InputBinding
    {
        #region static fields and constants

        /// <summary>
        /// Binding property for gesture.
        /// </summary>
        public static readonly DependencyProperty GestureProperty = DependencyProperty.Register(
            "Gesture",
            typeof(MultiKeyGesture),
            typeof(MultiKeyBinding),
            new PropertyMetadata(default(MultiKeyGesture)));

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the gesture binding.
        /// </summary>
        public override InputGesture Gesture
        {
            get => (MultiKeyGesture)this.GetValue(GestureProperty);
            set
            {
                this.SetValue(GestureProperty, value);
                base.Gesture = value;
            }
        }

        #endregion
    }
}