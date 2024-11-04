using Microsoft.VisualStudio.Text;
using Moq;

namespace Twizzar.Design.Test.Builder;

public class TextVersionBuilder
{
    private readonly Mock<ITextVersion> _mock;

    public TextVersionBuilder()
    {
        this._mock = new Mock<ITextVersion>();

        this._mock .Setup(textVersion => textVersion.Length)
            .Returns(1028);

        this._mock.Setup(textVersion => textVersion.Changes)
            .Returns(new Mock<INormalizedTextChangeCollection>().Object);

        this._mock.Setup(textVersion => textVersion.TextBuffer)
            .Returns(this._mock.Object.TextBuffer);
    }

    public TextVersionBuilder WithVersion(int version)
    {
        this._mock.Setup(textVersion => textVersion.VersionNumber)
            .Returns(version);

        return this;
    }

    public TextVersionBuilder WithNext(ITextVersion version)
    {
        this._mock.Setup(textVersion => textVersion.Next)
            .Returns(version);

        return this;
    }

    public ITextVersion Build() =>
        this._mock.Object;

}