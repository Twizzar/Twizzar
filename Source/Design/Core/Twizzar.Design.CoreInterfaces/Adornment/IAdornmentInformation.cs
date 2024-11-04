using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;

namespace Twizzar.Design.CoreInterfaces.Adornment
{
    /// <summary>
    /// Interface contains the adornment information.
    /// </summary>
    public interface IAdornmentInformation : IValueObject
    {
        /// <summary>
        /// Gets the the span of the builder object creation. e.g. new ItemBuilder&lt;T&gt;() or new MyCustomBuilder().
        /// </summary>
        IViSpan ObjectCreationSpan { get; }

        /// <summary>
        /// Gets the the span of the builder object creation type. e.g. new ItemBuilder&lt;T&gt; or new MyCustomBuilder.
        /// <remarks>This is <see cref="ObjectCreationSpan"/> without the Arguments.</remarks>
        /// </summary>
        IViSpan ObjectCreationTypeSpan { get; }

        /// <summary>
        /// Gets the id of the fixture item.
        /// </summary>
        FixtureItemId FixtureItemId { get; }

        /// <summary>
        /// Gets a value indicating whether multiple adornment are on the current adornment invocation line.
        /// </summary>
        public bool MultipleAdornmentsOnLine { get; }

        /// <summary>
        /// Gets the project name.
        /// </summary>
        public string ProjectName { get; }

        /// <summary>
        /// Gets the document file path.
        /// </summary>
        public string DocumentFilePath { get; }

        /// <summary>
        /// Update the versions of all viSpans. And returns a new Instance of <see cref="IAdornmentInformation"/>.
        /// </summary>
        /// <param name="viSpanVersion">The new version.</param>
        /// <returns>A new Adornment Information.</returns>
        IAdornmentInformation UpdateVersion(IViSpanVersion viSpanVersion);

        /// <summary>
        /// Update the spans.
        /// </summary>
        /// <param name="objectCreationSpan"></param>
        /// <param name="objectTypeCreationSpan"></param>
        /// <returns></returns>
        IAdornmentInformation UpdateSpans(IViSpan objectCreationSpan, IViSpan objectTypeCreationSpan);
    }
}