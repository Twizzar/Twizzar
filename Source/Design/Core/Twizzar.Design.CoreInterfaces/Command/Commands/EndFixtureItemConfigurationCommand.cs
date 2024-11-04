using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Design.CoreInterfaces.Command.Commands
{
    /// <summary>
    /// Command when the fixture item configuration ends. This most likely happens when the FixtureItemPanel is closed.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EndFixtureItemConfigurationCommand : ValueObject, ICommand<EndFixtureItemConfigurationCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndFixtureItemConfigurationCommand"/> class.
        /// </summary>
        /// <param name="fixtureItemId">The id of the fixture to clear.</param>
        public EndFixtureItemConfigurationCommand(FixtureItemId fixtureItemId)
        {
            this.RootFixtureItemId = fixtureItemId ?? throw new ArgumentNullException(nameof(fixtureItemId));
        }

        /// <summary>
        /// Gets the FixtureItemId of the root fixture item.
        /// </summary>
        public FixtureItemId RootFixtureItemId { get; }

        #region Overrides of ValueObject

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.RootFixtureItemId;
        }

        #endregion
    }
}
