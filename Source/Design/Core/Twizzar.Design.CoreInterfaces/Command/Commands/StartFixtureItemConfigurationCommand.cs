using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Command.Services;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;

namespace Twizzar.Design.CoreInterfaces.Command.Commands
{
    /// <summary>
    /// Command setting the project name for the current adornment.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class StartFixtureItemConfigurationCommand : ValueObject, ICommand<StartFixtureItemConfigurationCommand>
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="StartFixtureItemConfigurationCommand"/> class.
        /// </summary>
        /// <param name="fixtureItemId">The id of the fixture.</param>
        /// <param name="projectName">The project name.</param>
        /// <param name="documentFilePath">The document file path.</param>
        /// <param name="invocationSpan">The the span of a type invocation. e.g. Build.New(..).</param>
        public StartFixtureItemConfigurationCommand(
            FixtureItemId fixtureItemId,
            string projectName,
            string documentFilePath,
            IViSpan invocationSpan)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(fixtureItemId, nameof(fixtureItemId))
                .Parameter(invocationSpan, nameof(invocationSpan))
                .ThrowWhenNull();

            EnsureHelper.GetDefault.Many<string>()
                .Parameter(projectName, nameof(projectName))
                .Parameter(documentFilePath, nameof(documentFilePath))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            this.RootFixtureItemId = fixtureItemId;
            this.ProjectName = projectName;
            this.DocumentFilePath = documentFilePath;
            this.InvocationSpan = invocationSpan;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the RootFixtureItemId.
        /// </summary>
        public FixtureItemId RootFixtureItemId { get; }

        /// <summary>
        /// Gets the ProjectName.
        /// </summary>
        public string ProjectName { get; }

        /// <summary>
        /// Gets the document file path.
        /// </summary>
        public string DocumentFilePath { get; }

        /// <summary>
        /// Gets the the span of a type invocation. e.g. Build.New(..).
        /// </summary>
        public IViSpan InvocationSpan { get; }

        #endregion

        #region members

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.RootFixtureItemId;
            yield return this.ProjectName;
            yield return this.InvocationSpan;
        }

        #endregion
    }
}