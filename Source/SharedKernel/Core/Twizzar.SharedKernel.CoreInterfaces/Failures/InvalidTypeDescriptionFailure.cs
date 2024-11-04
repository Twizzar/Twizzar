using System.Diagnostics.CodeAnalysis;

using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Interfaces;

using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.Failures
{
    /// <summary>
    /// Exception type when the given type description is not valid for the given context.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InvalidTypeDescriptionFailure : Failure, IHasLogger, IHasEnsureHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeDescriptionFailure"/> class.
        /// </summary>
        /// <param name="typeDescription">The type description.</param>
        /// <param name="message">Failure message.</param>
        public InvalidTypeDescriptionFailure(
            ITypeDescription typeDescription,
            string message)
            : base(message)
        {
            this.EnsureParameter(typeDescription, nameof(typeDescription)).ThrowWhenNull();

            this.TypeDescription = typeDescription;
        }

        /// <summary>
        /// Gets the type description.
        /// </summary>
        public ITypeDescription TypeDescription { get; }

        #region Implementation of IHasLogger

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        #endregion

        #region Implementation of IHasEnsureHelper

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region Overrides of Failure

        /// <inheritdoc />
        public override string ToString() =>
            $"InvalidTypeDescriptionFailure: {this.TypeDescription.TypeFullName.GetTypeName()} {this.Message}";

        #endregion
    }
}