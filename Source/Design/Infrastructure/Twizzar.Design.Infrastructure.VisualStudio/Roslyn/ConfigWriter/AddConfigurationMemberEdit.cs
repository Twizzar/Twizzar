using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn.ConfigWriter
{
    /// <summary>
    /// Holds information about a Configuration statement like <c>this.Property(MyMember).Value(5);</c>.
    /// </summary>
    public readonly struct AddConfigurationMemberEdit : IConfigurationMemberEdit
    {
        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddConfigurationMemberEdit"/> struct.
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="memberPath"></param>
        public AddConfigurationMemberEdit(StatementSyntax syntax, string memberPath)
        {
            this.Syntax = syntax;
            this.MemberPath = memberPath;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the syntax statement.
        /// </summary>
        public StatementSyntax Syntax { get; }

        /// <summary>
        /// Gets the member path.
        /// </summary>
        public string MemberPath { get; }

        #endregion
    }
}