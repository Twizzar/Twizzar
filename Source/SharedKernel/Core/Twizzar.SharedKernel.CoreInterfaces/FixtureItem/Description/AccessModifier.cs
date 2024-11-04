using System.Collections.Generic;

#pragma warning disable CS1591, S107, SA1600

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// The access modifier for types and members.
    /// </summary>
    public sealed class AccessModifier : ValueObject
    {
        #region ctors

        public AccessModifier(bool isPrivate, bool isPublic, bool isProtected, bool isInternal)
        {
            this.IsPrivate = isPrivate;
            this.IsPublic = isPublic;
            this.IsProtected = isProtected;
            this.IsInternal = isInternal;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets a value indicating whether the field is private.
        /// </summary>
        public bool IsPrivate { get; }

        /// <summary>
        /// Gets a value indicating whether the is a public field.
        /// </summary>
        public bool IsPublic { get; }

        /// <summary>
        /// Gets a value indicating whether the is a protected field.
        /// </summary>
        public bool IsProtected { get; }

        /// <summary>
        /// Gets a value indicating whether the is a public field.
        /// </summary>
        public bool IsInternal { get; }

        #endregion

        #region members

        public static AccessModifier CreatePublic() =>
            new(false, true, false, false);

        public static AccessModifier CreatePrivate() =>
            new(true, false, false, false);

        public static AccessModifier CreateProtected() =>
            new(false, false, true, false);

        public static AccessModifier CreateInternal() =>
            new(false, false, false, true);

        /// <inheritdoc />
        public override string ToString()
        {
            var modifiers = new List<string>();

            if (this.IsPublic)
            {
                modifiers.Add("public");
            }

            if (this.IsPrivate)
            {
                modifiers.Add("private");
            }

            if (this.IsProtected)
            {
                modifiers.Add("protected");
            }

            if (this.IsInternal)
            {
                modifiers.Add("internal");
            }

            return string.Join(" ", modifiers);
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.IsPrivate;
            yield return this.IsPublic;
            yield return this.IsProtected;
            yield return this.IsInternal;
        }

        #endregion
    }
}