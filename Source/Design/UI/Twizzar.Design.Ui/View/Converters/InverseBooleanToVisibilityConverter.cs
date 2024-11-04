using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Twizzar.Design.Ui.View.Converter
{
    /// <summary>
    /// Represents the converter that inverts the boolean value.
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool boolean && !boolean ? Visibility.Visible : Visibility.Collapsed;

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => !(value is Visibility visibility) || visibility != Visibility.Visible;

        #endregion
    }
}
