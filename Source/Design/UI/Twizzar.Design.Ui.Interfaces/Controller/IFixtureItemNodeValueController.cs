using System;
using System.Threading.Tasks;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes;

namespace Twizzar.Design.Ui.Interfaces.Controller
{
    /// <summary>
    /// Controller for the <see cref="IFixtureItemNodeValueViewModel"/>.
    /// </summary>
    public interface IFixtureItemNodeValueController : IDisposable
    {
        #region events

        /// <summary>
        /// Event triggered when the value is changed.
        /// </summary>
        event Action<IViToken> ValueChanged;

        #endregion

        #region properties

        /// <summary>
        /// Gets a value indicating whether commit is dirty or not.
        /// </summary>
        bool IsCommitDirty { get; }

        #endregion

        #region members

        /// <summary>
        /// Set the value.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="isDefault">True when this is a default value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetValueAsync(string value, bool isDefault);

        /// <summary>
        /// Try to focus this, when this is disposed focus parent.
        /// </summary>
        void Focus();

        #endregion
    }
}