using Twizzar.Design.Ui.Interfaces.ViewModels.FixtureItem.Nodes.Enums;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Design.Ui.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="AccessModifier"/>.
    /// </summary>
    public static class AccessModifierExtensions
    {
        /// <summary>
        /// Maps the <see cref="AccessModifier"/> to a <see cref="MemberModifier"/>.
        /// </summary>
        /// <param name="accessModifier">The access modifier.</param>
        /// <returns>A member modifier.</returns>
        public static MemberModifier MapToMemberModifier(this AccessModifier accessModifier)
        {
            if (accessModifier.IsPublic)
            {
                return MemberModifier.Public;
            }
            else if (accessModifier.IsProtected)
            {
                return MemberModifier.Protected;
            }
            else if (accessModifier.IsInternal)
            {
                return MemberModifier.Internal;
            }
            else if (accessModifier.IsPrivate)
            {
                return MemberModifier.Private;
            }

            return MemberModifier.NotDefined;
        }
    }
}
