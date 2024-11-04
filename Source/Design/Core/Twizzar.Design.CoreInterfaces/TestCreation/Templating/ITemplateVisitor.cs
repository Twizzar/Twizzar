namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating
{
    /// <summary>
    /// Service for visiting a snipped and all its children.
    /// </summary>
    public interface ITemplateVisitor
    {
        /// <summary>
        /// Visit a snipped and all its children and return the produced code.
        /// </summary>
        /// <param name="snippet"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        string Visit(ITemplateSnippet snippet, ITemplateContext context);
    }
}