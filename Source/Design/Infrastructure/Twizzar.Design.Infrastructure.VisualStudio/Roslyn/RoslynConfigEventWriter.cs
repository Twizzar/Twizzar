using System;
using System.Collections.Generic;
using System.Threading;

using Twizzar.Design.CoreInterfaces.Command.Events;
using Twizzar.Design.CoreInterfaces.Command.FixtureItem.Config;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

#pragma warning disable VSTHRD100 // Avoid async void methods

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Writes configuration changes in to the designated config class.
    /// </summary>
    public class RoslynConfigEventWriter : IEventWriter
    {
        #region fields

        private readonly IRoslynConfigWriter _roslynConfigWriter;
        private readonly ISet<string> _openConfigurations = new HashSet<string>();

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynConfigEventWriter"/> class.
        /// </summary>
        /// <param name="roslynConfigWriter"></param>
        public RoslynConfigEventWriter(
            IRoslynConfigWriter roslynConfigWriter)
        {
            this.EnsureMany()
                .Parameter(roslynConfigWriter, nameof(roslynConfigWriter))
                .ThrowWhenNull();

            this._roslynConfigWriter = roslynConfigWriter;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public bool IsListening => true;

        /// <inheritdoc />
        public Maybe<SynchronizationContext> SynchronizationContext { get; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public async void Handle(FixtureItemMemberChangedEvent e)
        {
            try
            {
                if (!this._openConfigurations.Contains(e.FixtureItemId.RootItemPath.SomeOrProvided(string.Empty)) || e.IsFromInitialization)
                {
                    return;
                }

                await this._roslynConfigWriter.UpdateConfigAsync(e.FixtureItemId, e.MemberConfiguration);
            }
            catch (Exception ex)
            {
                this.Log(ex);
            }
        }

        /// <inheritdoc />
        public void Handle(FixtureItemConfigurationStartedEvent e)
        {
            e.FixtureItemId.RootItemPath.IfSome(rootPath => this._openConfigurations.AddIfNotExists(rootPath));
        }

        /// <inheritdoc />
        public void Handle(FixtureItemConfigurationEndedEvent e)
        {
            if (this._openConfigurations.Contains(e.RootFixtureItemPath))
            {
                this._openConfigurations.Remove(e.RootFixtureItemPath);
            }
        }

        #endregion
    }
}