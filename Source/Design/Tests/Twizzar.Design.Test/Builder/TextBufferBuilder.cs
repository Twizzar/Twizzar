using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Moq;

namespace Twizzar.Design.Test.Builder;

public class TextBufferBuilder
{
    private readonly Mock<ITextBuffer> _mock;

    public TextBufferBuilder()
    {
        this._mock = new Mock<ITextBuffer>();

        this._mock.Setup(buffer => buffer.EditInProgress)
            .Returns(false);

        this._mock.Setup(buffer => buffer.Properties)
            .Returns(new Mock<PropertyCollection>().Object);
    }

    public TextBufferBuilder WithCurrentSnapshot(ITextSnapshot snapshot)
    {
        this._mock.Setup(buffer => buffer.CurrentSnapshot)
            .Returns(snapshot);

        return this;
    }

    public TextBufferBuilder WithCurrentSnapshot(TextSnapshotBuilder textSnapshotBuilder)
    {
        textSnapshotBuilder.WithTextBuffer(this._mock.Object);

        this._mock.Setup(buffer => buffer.CurrentSnapshot)
            .Returns(textSnapshotBuilder.Build());

        return this;
    }

    public ITextBuffer Build() =>
        this._mock.Object;
}