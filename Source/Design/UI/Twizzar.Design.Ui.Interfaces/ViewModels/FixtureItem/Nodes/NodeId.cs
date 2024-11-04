using System;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes
{
    /// <summary>
    /// Node id for identifying a node in the UI Tree.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class NodeId
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeId"/> class.
        /// </summary>
        public NodeId()
        {
            this.Guid = Guid.NewGuid();
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the guid.
        /// </summary>
        public Guid Guid { get; }

        #endregion
    }
}