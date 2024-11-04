using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Moq;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;

public class TextSnapshotBuilder : IHasEnsureHelper
{
    #region fields

    private readonly Mock<ITextSnapshot> _mock;

    #endregion

    #region ctors

    public TextSnapshotBuilder()
    {
        this._mock = new Mock<ITextSnapshot>();

        this._mock.Setup(snapshot => snapshot.CreateTrackingSpan(It.IsAny<Span>(), It.IsAny<SpanTrackingMode>()))
            .Returns(new Mock<ITrackingSpan>().Object);

        this._mock.Setup(snapshot => snapshot.ContentType)
            .Returns(new Mock<IContentType>().Object);

        this._mock.Setup(snapshot => snapshot.Lines)
            .Returns(Enumerable.Empty<ITextSnapshotLine>());

    }

    #endregion

    #region properties

    /// <inheritdoc />
    public IEnsureHelper EnsureHelper { get; set; }

    #endregion

    #region members

    public TextSnapshotBuilder WithLength(int length)
    {
        this.EnsureParameter(length, nameof(length))
            .IsGreaterEqualThan(0)
            .ThrowOnFailure();

        this._mock.Setup(snapshot => snapshot.Length)
            .Returns(length);

        //this._mock.Setup(snapshot => snapshot.)

        return this;
    }

    public TextSnapshotBuilder WithTextBuffer(ITextBuffer textBuffer)
    {
        this._mock.Setup(snapshot => snapshot.TextBuffer)
            .Returns(textBuffer);

        return this;
    }

    public TextSnapshotBuilder WithVersion(ITextVersion version)
    {
        this._mock.Setup(snapshot => snapshot.Version)
            .Returns(version);

        return this;
    }

    public ITextSnapshot Build() =>
        this._mock.Object;

    #endregion
}