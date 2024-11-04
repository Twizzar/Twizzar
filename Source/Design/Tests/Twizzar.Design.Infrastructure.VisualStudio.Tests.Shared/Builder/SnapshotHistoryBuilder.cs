using Microsoft.VisualStudio.Text;
using Moq;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;

public class SnapshotHistoryBuilder
{
    private readonly Mock<ISnapshotHistory> _mock;

    public SnapshotHistoryBuilder()
    {
        this._mock = new Mock<ISnapshotHistory>();
    }

    public SnapshotHistoryBuilder AddVersion(IViSpanVersion version, ITextSnapshot textSnapshot = null)
    {
        var snapshot = textSnapshot ?? new TextSnapshotBuilder().WithLength(1024).Build();

        this._mock.Setup(history => history.Get(version))
            .Returns(Maybe.Some(snapshot));

        return this;
    }

    public ISnapshotHistory Build() =>
        this._mock.Object;
}