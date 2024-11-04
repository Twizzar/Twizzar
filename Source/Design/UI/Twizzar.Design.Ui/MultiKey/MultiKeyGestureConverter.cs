using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.Functional.Monads.ResultMonad;

using Key = System.Windows.Input.Key;

namespace Twizzar.Design.Ui.MultiKey
{
    /// <summary>
    /// Multi key gesture converter.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MultiKeyGestureConverter : IValueConverter
    {
        #region fields

        private readonly KeyGestureConverter _keyGestureConverter = new();
        private readonly string _ctrlDe = "strg";
        private readonly string _shiftDe = "umschalt";
        private readonly string _ctrlEn = "ctrl";
        private readonly string _shiftEn = "shift";

        #endregion

        #region members

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(MultiKeyGesture) ||
                (value is not string valueAsString) ||
                string.IsNullOrEmpty(valueAsString))
            {
                return null;
            }

            // two key strokes are supported

            var keyStrokes = valueAsString
                .ToLower()
                .Replace(this._ctrlDe, this._ctrlEn)
                .Replace(this._shiftDe, this._shiftEn)
                .Split(',');

            switch (keyStrokes.Length)
            {
                case > 2:
                    this.Log($"Invalid shortcut string: {valueAsString}");
                    return null;
                case 2:
                    {
                        var firstGesture = this.GetKeyGesture(keyStrokes[0]);
                        var secondGesture = this.GetKeyGesture(keyStrokes[1]);

                        if (firstGesture is null || secondGesture is null)
                        {
                            return null;
                        }

                        return Result.Match(
                            from first in firstGesture
                            from second in secondGesture
                            select new MultiKeyGesture(new List<KeyGesture> { first, second }),
                            gesture => gesture,
                            failure =>
                            {
                                this.Log(failure.Message);
                                return null;
                            });
                    }
                default:
                    {
                        var shortcut = this.GetKeyGesture(valueAsString);

                        return shortcut
                            .Match(
                                gesture => new MultiKeyGesture(new List<KeyGesture> { gesture }),
                                failure =>
                                {
                                    this.Log(failure.Message);
                                    return null;
                                });
                    }
            }
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is MultiKeyGesture multiKeyGesture
                ? multiKeyGesture.ToString()
                : string.Empty;

        private IResult<KeyGesture, Failure> GetKeyGesture(string keyStrokes)
        {
            try
            {
                return Result.Success<KeyGesture, Failure>(
                    (KeyGesture)this._keyGestureConverter.ConvertFromInvariantString(keyStrokes));
            }
            catch (Exception)
            {
                var keys = keyStrokes.Split('+');

                if (keys.Length == 2)
                {
                    var modifier = ParseModifierKey(keys[0]);

                    return ParseKey(keys[1])
                        .MapSuccess(k => new KeyGesture(k, modifier));
                }
                else if (keys.Length == 1)
                {
                    return ParseKey(keys[0])
                        .MapSuccess(k => new KeyGesture(k));
                }
                else
                {
                    return Result.Failure<KeyGesture, Failure>(
                        new Failure($"Cannot convert {keyStrokes} to a KeyGesture"));
                }
            }
        }

        private static IResult<Key, Failure> ParseKey(string keyString)
        {
            var k = new KeyConverter();

            try
            {
                var key = (Key)k.ConvertFromString(keyString);
                return Result.Success<Key, Failure>(key);
            }
            catch (Exception)
            {
                var key = ConvertStringToKey(keyString);
                return key == Key.None
                    ? Result.Failure<Key, Failure>(new Failure($"Cannot parse {keyString}"))
                    : Result.Success<Key, Failure>(key);
            }
        }

        private static Key ConvertStringToKey(string keyName) =>
            CultureInfo.CurrentCulture.Name.ToLowerInvariant() switch
            {
                "de-ch" =>
                    keyName switch
                    {
                        "<" => Key.Oem102,
                        ">" => Key.Oem102,
                        @"\" => Key.Oem102,
                        _ => Key.None,
                    },
                _ => Key.None,
            };

        private static ModifierKeys ParseModifierKey(string modifier) =>
            modifier.Trim().ToLowerInvariant() switch
            {
                "ctrl" => ModifierKeys.Control,
                "alt" => ModifierKeys.Alt,
                "shift" => ModifierKeys.Shift,
                _ => ModifierKeys.None,
            };

        #endregion
    }
}