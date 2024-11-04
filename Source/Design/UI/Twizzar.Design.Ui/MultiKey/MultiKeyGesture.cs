using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace Twizzar.Design.Ui.MultiKey
{
    /// <summary>
    /// MultiKey gesture for binding multiple key shortcuts.
    /// </summary>
    [TypeConverter(typeof(MultiKeyGestureConverter))]
    [ExcludeFromCodeCoverage]
    public class MultiKeyGesture : KeyGesture
    {
        #region static fields and constants

        private static readonly TimeSpan MaximumDelayBetweenKeyPresses = TimeSpan.FromSeconds(1);

        #endregion

        #region fields

        private readonly IList<KeyGesture> _shortCuts;

        private int _currentKeyIndex;
        private DateTime _lastKeyPress;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyGesture"/> class.
        /// </summary>
        /// <param name="shortcuts">The defined shortcuts.</param>
        public MultiKeyGesture(IList<KeyGesture> shortcuts)
            : base(Key.None, ModifierKeys.None, string.Empty)
        {
            this._shortCuts = new List<KeyGesture>(shortcuts);

            if (this._shortCuts.Count == 0)
            {
                throw new ArgumentException("At least one key must be specified.");
            }
        }

        #endregion

        #region members

        /// <summary>
        /// Determines if the given input matches the defined multi key gesture.
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="inputEventArgs"></param>
        /// <returns><c>True</c> if input matches the defined multi-key binding (sequence).</returns>
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if ((inputEventArgs is not KeyEventArgs args) || !IsDefinedKey(args.Key))
            {
                return false;
            }

            if (this._currentKeyIndex != 0 && ((DateTime.Now - this._lastKeyPress) > MaximumDelayBetweenKeyPresses))
            {
                // took too long to press next key so reset
                this._currentKeyIndex = 0;
                return false;
            }

            if (this._shortCuts[this._currentKeyIndex].Modifiers != Keyboard.Modifiers
                || this._shortCuts[this._currentKeyIndex].Key != args.Key)
            {
                // wrong modifiers or key
                this._currentKeyIndex = 0;
                return false;
            }

            this._currentKeyIndex++;

            if (this._currentKeyIndex != this._shortCuts.Count)
            {
                // still matching
                this._lastKeyPress = DateTime.Now;
                inputEventArgs.Handled = true;
                return false;
            }

            // match complete
            this._currentKeyIndex = 0;
            return true;
        }

        private static bool IsDefinedKey(Key key) => (key >= Key.None) && (key <= Key.OemClear);

        #endregion
    }
}