using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Creators;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;

namespace Twizzar.Runtime.Core.FixtureItem.Creators.BaseTypeUniqueCreators
{
    /// <summary>
    /// Implements the <see cref="IBaseTypeCreator"/> for <see cref="string"/>.
    /// </summary>
    public class StringUniqueCreator : IUniqueCreator<string>
    {
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
        public string GetNextValue()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion
    }
}