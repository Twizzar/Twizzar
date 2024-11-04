using System;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Description.Services;
using Twizzar.Runtime.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Runtime.Infrastructure.ApplicationService
{
    /// <summary>
    /// It returns self generated TypeDescriptions for given types.
    /// </summary>
    public class ReflectionTypeDescriptionProvider : ITypeDescriptionQuery
    {
        #region fields

        private readonly IReflectionDescriptionFactory _descriptionFactory;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionTypeDescriptionProvider"/> class.
        /// </summary>
        /// <param name="descriptionFactory">The description factory.</param>
        public ReflectionTypeDescriptionProvider(IReflectionDescriptionFactory descriptionFactory)
        {
            this.EnsureMany()
                .Parameter(descriptionFactory, nameof(descriptionFactory))
                .ThrowWhenNull();

            this._descriptionFactory = descriptionFactory;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the Logger instance.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public IRuntimeTypeDescription GetTypeDescription(Type type) =>
            this.GetTypeDescriptionFromType(type);

        /// <inheritdoc />
        public IRuntimeTypeDescription GetTypeDescription(ITypeFullName typeFullName) =>
            this.GetTypeDescription(((TypeFullName)typeFullName).Type);

        private IRuntimeTypeDescription GetTypeDescriptionFromType(Type type) =>
            this._descriptionFactory.Create(type);

        #endregion
    }
}