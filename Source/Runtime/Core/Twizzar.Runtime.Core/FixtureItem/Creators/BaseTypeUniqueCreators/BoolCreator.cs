using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators
{
    /// <summary>
    /// Implements the <see cref="IUniqueCreator{T}"/> for <see cref="bool"/>.
    /// </summary>
    public class BoolCreator : IUniqueCreator<bool>
    {
        private const bool DefaultValue = true;

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Implementation of IUniqueCreator

        /// <inheritdoc />
        public bool GetNextValue()
        {
            return DefaultValue;
        }

        #endregion

    }
}