using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Design.Ui.View.Converter;

namespace Twizzar.Design.Ui.Tests.View.Converter;

[TestFixture]
public class InverseBooleanToVisibilityConverterTests
{
    private IValueConverter _converter;

    [SetUp]
    public void Setup()
    {
        this._converter = new InverseBooleanToVisibilityConverter();
    }

    [Test]
    public void Convert_true_returns_collapsed()
    {
        var result = this._converter.Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture);

        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_false_returns_visible()
    {
        var result = this._converter.Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture);

        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_null_returns_hidden()
    {
        var result = this._converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void ConvertBack_visible_returns_false()
    {
        var result = this._converter.ConvertBack(Visibility.Visible, typeof(bool?), null, CultureInfo.InvariantCulture);

        result.Should().Be(false);
    }

    [Test]
    public void ConvertBack_collapsed_returns_true()
    {
        var result = this._converter.ConvertBack(Visibility.Collapsed, typeof(bool?), null, CultureInfo.InvariantCulture);

        result.Should().Be(true);
    }

    [Test]
    public void ConvertBack_hidden_returns_true()
    {
        var result = this._converter.ConvertBack(Visibility.Hidden, typeof(bool?), null, CultureInfo.InvariantCulture);

        result.Should().Be(true);
    }

    [Test]
    public void ConvertBack_null_returns_true()
    {
        var result = this._converter.ConvertBack(null, typeof(bool?), null, CultureInfo.InvariantCulture);

        result.Should().Be(true);
    }
}