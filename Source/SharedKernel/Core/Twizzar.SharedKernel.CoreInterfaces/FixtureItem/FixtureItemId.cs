using System.Collections.Generic;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem
{
    /// <summary>
    /// Value object containing all information to identify a FixtureItem.
    /// </summary>
    public class FixtureItemId : ValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureItemId"/> class.
        /// </summary>
        /// <param name="rootItemPath">The id of the root item.</param>
        /// <param name="name">The configuration name of the fixture item.</param>
        /// <param name="typeFullName">The type of the fixture item.</param>
        private FixtureItemId(Maybe<string> rootItemPath, Maybe<string> name, ITypeFullName typeFullName)
        {
            EnsureHelper.GetDefault.Many()
                .Parameter(name, nameof(name))
                .Parameter(typeFullName, nameof(typeFullName))
                .ThrowWhenNull();

            this.Name = name;
            this.TypeFullName = typeFullName;
            this.RootItemPath = rootItemPath;
        }

        /// <summary>
        /// Gets the name of the id.
        /// </summary>
        public Maybe<string> Name { get; }

        /// <summary>
        /// Gets the type of the id.
        /// </summary>
        public ITypeFullName TypeFullName { get; }

        /// <summary>
        /// Gets the id of the root element.
        /// </summary>
        public Maybe<string> RootItemPath { get; }

        /// <summary>
        /// Create a new instance copy the old values but set a new type.
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <returns>A new fixtureItemId.</returns>
        public FixtureItemId WithType(ITypeFullName typeFullName) =>
            new(this.RootItemPath, this.Name, typeFullName);

        /// <summary>
        /// Create a new instance copy the old values but set a new name.
        /// </summary>
        /// <param name="newName">The new name.</param>
        /// <returns>A new fixtureItemId.</returns>
        public FixtureItemId WithName(Maybe<string> newName) =>
            new(this.RootItemPath, newName, this.TypeFullName);

        /// <summary>
        /// Create a new instance copy the old values but set a rootItemPath.
        /// </summary>
        /// <param name="rootItemPath">The rootItemPath.</param>
        /// <returns>A new fixtureItemId.</returns>
        public FixtureItemId WithRootItemPath(Maybe<string> rootItemPath) =>
            new(rootItemPath, this.Name, this.TypeFullName);

        /// <summary>
        /// Create a named FixtureItemId.
        /// </summary>
        /// <param name="name">The configuration name of the fixture item.</param>
        /// <param name="type">The type of the fixture item.</param>
        /// <returns>A new instance of a FixtureItemId.</returns>
        public static FixtureItemId Create(Maybe<string> name, ITypeFullName type) =>
            new(Maybe.None(), name, type);

        /// <summary>
        /// Create a named FixtureItemId.
        /// </summary>
        /// <param name="name">The configuration name of the fixture item.</param>
        /// <param name="type">The type of the fixture item.</param>
        /// <returns>A new instance of a FixtureItemId.</returns>
        public static FixtureItemId CreateNamed(string name, ITypeFullName type) =>
            new(Maybe.None(), name, type);

        /// <summary>
        /// Create a nameless FixtureItemId.
        /// </summary>
        /// <param name="type">The type of the fixture item.</param>
        /// <returns>A new instance of a FixtureItemId.</returns>
        public static FixtureItemId CreateNameless(ITypeFullName type) =>
            new(Maybe.None(), Maybe.None<string>(), type);

        /// <summary>
        /// Gets e friendly to read representation of the id. Useful for debugging.
        /// </summary>
        /// <returns>The name without the rootItemPath + the type name.</returns>
        public override string ToString() =>
            $"{this.Name.Map(s => this.RootItemPath.Match(r => s.Replace(r, string.Empty), () => s))}: {this.TypeFullName.GetFriendlyCSharpTypeName()}";

        /// <summary>
        /// Gets the path of the id.
        /// </summary>
        /// <returns></returns>
        public string ToPathString() => $"{this.Name.SomeOrProvided("SystemDefault")}";

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Name;
            yield return this.RootItemPath;
            yield return this.TypeFullName;
        }
    }
}
