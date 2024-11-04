using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;

namespace Twizzar.Design.Shared.CoreInterfaces.Name
{
    /// <summary>
    /// Extension method for <see cref="ITypeFullName"/>.
    /// </summary>
    public static class TypeFullNameExtensions
    {
        /// <summary>
        /// Cast the <see cref="ITypeFullName"/> to <see cref="TypeFullName"/>.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>The casted <see cref="TypeFullName"/>.</returns>
        public static TypeFullName Cast(this ITypeFullName self) =>
            (TypeFullName)self;

        /// <summary>
        /// Tries to parse the type fullname to an friendly csharp name.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>On parse success the name; else a parse failure.</returns>
        public static string GetFriendlyCSharpFullName(this ITypeFullName self) =>
            self.Cast().GetFriendlyCSharpFullName();

        /// <summary>
        /// Gets the type full name token.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>The <see cref="ITypeFullNameToken"/>.</returns>
        public static ITypeFullNameToken GetTypeFullNameToken(this ITypeFullName self) =>
            self.Cast().TypeFullNameToken;

        /// <summary>
        /// Get the namespace of the full type.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>The namespace as a string.</returns>
        public static string GetNameSpace(this ITypeFullName self) =>
            self.Cast().GetNameSpace();

        /// <summary>
        /// Gets the namespace and the Declaring type.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>The namespace and the declaring type separated with a .</returns>
        public static string GetNameSpaceWithDeclaringType(this ITypeFullName self) =>
            self.Cast().GetNameSpaceWithDeclaringType();

        /// <summary>
        /// Get the type fill name with the generic postfix.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string GetTypeFullNameWithGenericPostfix(this ITypeFullName self) =>
            self.GetTypeFullNameToken().Namespace + self.GetTypeFullNameToken().ToNameWithGenericPostfix();

        /// <summary>
        /// Gets the friendly csharp type name without the generic part.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string GetFriendlyTypeFullNameWithoutGenerics(this ITypeFullName self)
        {
            var token = self.GetTypeFullNameToken();
            return $"{token.ToNamespaceWithDeclaringType()}{token.Typename}";
        }
    }
}
