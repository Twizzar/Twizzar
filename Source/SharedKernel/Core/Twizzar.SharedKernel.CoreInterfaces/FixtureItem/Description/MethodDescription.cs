using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description.Services;

#pragma warning disable CS1591, S107, SA1600

namespace Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description
{
    /// <summary>
    /// Provides information about methods and constructors.
    /// </summary>
    public abstract class MethodDescription : BaseTypeDescription, IMethodDescription
    {
        #region ctors

        protected MethodDescription(IBaseTypeService baseTypeService)
            : base(baseTypeService)
        {
        }

        #endregion

        #region properties

        /// <inheritdoc />
        public string UniqueName => this.Name + this.GetMethodPostfix();

        public string Parameters =>
            this.DeclaredParameters
                .Select(description => description.Name)
                .ToCommaSeparated();

        /// <inheritdoc />
        public string FriendlyParameterFullTypes =>
            this.DeclaredParameters
                .Select(description => description.TypeFullName.GetFriendlyCSharpTypeFullName())
                .ToCommaSeparated();

        /// <inheritdoc />
        public abstract ImmutableArray<IParameterDescription> DeclaredParameters { get; }

        /// <inheritdoc />
        public bool IsConstructor { get; protected set; }

        /// <inheritdoc />
        public bool IsAbstract { get; protected set; }

        /// <inheritdoc />
        public bool IsStatic { get; protected set; }

        /// <inheritdoc />
        public bool IsGeneric { get; protected set; }

        /// <inheritdoc />
        public ImmutableDictionary<int, GenericParameterType> GenericTypeArguments { get; protected set; }

        /// <inheritdoc />
        public OverrideKind OverrideKind { get; protected set; }

        /// <inheritdoc />
        public MethodKind MethodKind { get; protected set; }

        /// <inheritdoc />
        public string Name { get; protected set; }

        /// <inheritdoc />
        public abstract ITypeDescription DeclaredDescription { get; }

        #endregion

        #region members

        /// <inheritdoc />
        public override string ToString()
        {
            var staticStr = this.IsStatic ? " static" : string.Empty;
            var abstractStr = this.IsAbstract ? " abstract" : string.Empty;

            var genericStr = this.IsGeneric
                ? "<" +
                  string.Join(
                      ",",
                      this.GenericTypeArguments.Values
                          .Select(name =>
                              name.TypeFullName
                                  .Map(fullName => fullName.GetTypeName())
                                  .SomeOrProvided(name.Name))) +
                  ">"
                : string.Empty;

            return
                $"{this.AccessModifier}{staticStr}{abstractStr} {this.TypeFullName?.GetFriendlyCSharpTypeName()} {this.Name}{genericStr}({this.Parameters})";
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.AccessModifier;
            yield return this.TypeFullName;
            yield return this.Name;
            yield return this.DeclaredParameters;
            yield return this.IsConstructor;
            yield return this.IsAbstract;
            yield return this.IsStatic;
            yield return this.IsGeneric;
            yield return this.GenericTypeArguments;
            yield return this.OverrideKind;
        }

        private string GetMethodPostfix()
        {
            var genericTypeParameters = this.GetTypeParameterString();
            var parameterString = this.GetParameterString();

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(genericTypeParameters))
            {
                sb.Append(genericTypeParameters);
            }

            if (!string.IsNullOrEmpty(parameterString))
            {
                sb.Append($"__{parameterString}");
            }

            return sb.ToString();
        }

        private string GetParameterString() =>
            this.DeclaredParameters
                .Select(description => description.TypeFullName.GetTypeName())
                .ToDisplayString("_")
                .ToSourceVariableCodeFriendly();

        private string GetTypeParameterString() =>
            this.GenericTypeArguments
                .Select(pair => pair.Value.Name)
                .ToDisplayString(string.Empty)
                .ToSourceVariableCodeFriendly();

        #endregion
    }
}