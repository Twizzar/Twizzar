using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.SharedKernel.CoreInterfaces;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using VSLangProj;

namespace Twizzar.Design.Infrastructure.VisualStudio.Models
{
    /// <inheritdoc cref="IViReference" />
    [ExcludeFromCodeCoverage] // dependent on VsSDK
    public class ViReference : Entity<ViReference, string>, IViReference
    {
        #region fields

        private readonly Reference _reference;

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViReference"/> class.
        /// </summary>
        /// <param name="reference">The reference.</param>
        public ViReference(Reference reference)
            : base(reference.Identity)
        {
            this.EnsureParameter(reference, nameof(reference)).ThrowWhenNull();

            this._reference = reference;
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string Name => this._reference.Name;

        /// <inheritdoc />
        public string Path => this._reference.Path;

        /// <inheritdoc />
        public string Description => this._reference.Description;

        /// <inheritdoc />
        public string Culture => this._reference.Culture;

        /// <inheritdoc />
        public int MajorVersion => this._reference.MajorVersion;

        /// <inheritdoc />
        public int MinorVersion => this._reference.MinorVersion;

        /// <inheritdoc />
        public int RevisionNumber => this._reference.RevisionNumber;

        /// <inheritdoc />
        public int BuildNumber => this._reference.BuildNumber;

        /// <inheritdoc />
        public bool StrongName => this._reference.StrongName;

        /// <inheritdoc />
        public bool CopyLocal
        {
            get => this._reference.CopyLocal;
            set => this._reference.CopyLocal = value;
        }

        /// <inheritdoc />
        public string Version => this._reference.Version;

        /// <inheritdoc />
        public IViProject ContainingProject => new VsProject(this._reference.ContainingProject);

        /// <inheritdoc />
        public Maybe<IViProject> SourceProject =>
            Maybe.ToMaybe(this._reference.SourceProject)
                .Map(project => (IViProject)new VsProject(project));

        #endregion

        #region members

        /// <inheritdoc />
        public void Remove()
        {
            this._reference.Remove();
        }

        /// <inheritdoc />
        protected override bool Equals(string a, string b) =>
            a == b;

        #endregion
    }
}