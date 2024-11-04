using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Configuration.Location;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Document location provided by roslyn.
    /// </summary>
    /// <param name="Column"></param>
    /// <param name="Row"></param>
    public record RoslynDocumentLocation(int Column, int Row) : IDocumentLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynDocumentLocation"/> class.
        /// </summary>
        /// <param name="location"></param>
        public RoslynDocumentLocation(Location location)
            : this(location.GetLineSpan().Span.Start)
        {
        }

        private RoslynDocumentLocation(LinePosition position)
            : this(position.Character, position.Line)
        {
        }
    }
}