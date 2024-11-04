using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Twizzar.Design.Ui.View.Converters
{
    /// <summary>
    /// Reverse boolean to visibility converter: false is Visible.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ReversedBooleanToVisibilityConverter : IValueConverter
    {
        private readonly BooleanToVisibilityConverter _innerConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReversedBooleanToVisibilityConverter"/> class.
        /// </summary>
        public ReversedBooleanToVisibilityConverter()
        {
            this._innerConverter = new BooleanToVisibilityConverter();
        }

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool b
                ? this._innerConverter.Convert(!b, targetType, parameter, culture)
                : Visibility.Collapsed;

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object Func(object x) =>
                this._innerConverter.ConvertBack(x, targetType, parameter, culture);

            return value switch
            {
                Visibility.Collapsed => Func(Visibility.Visible),
                Visibility.Visible => Func(Visibility.Collapsed),
                Visibility.Hidden => Func(Visibility.Visible),
                _ => Func(Visibility.Collapsed),
            };
        }
    }
}