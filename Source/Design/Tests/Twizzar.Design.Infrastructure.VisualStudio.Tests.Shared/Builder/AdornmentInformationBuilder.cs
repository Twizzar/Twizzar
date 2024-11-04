using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.Common.FixtureItem.Adornment;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem;
using static Twizzar.TestCommon.TestHelper;

namespace Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;

public class AdornmentInformationBuilder
{
    private IViSpan _objectCreationSpan;
    private IViSpan _objectCreationTypeSpan;
    private readonly FixtureItemId _fixtureItemId;
    private IViSpanVersion _version = ViSpanVersion.UnknownVersion;

    public AdornmentInformationBuilder()
    {
        this._objectCreationSpan = CreateRandomViSpan();

        this._objectCreationTypeSpan = new ViSpan(
            this._objectCreationSpan.Start,
            this._objectCreationSpan.Length - 2);
        this._fixtureItemId = RandomNamedFixtureItemId();
    }

    public AdornmentInformationBuilder WithObjectCreationSpan(int start, int length)
    {
        this._objectCreationSpan = new ViSpan(start, length);
        return this;
    }

    public AdornmentInformationBuilder WithVersion(IViSpanVersion version)
    {
        this._version = version;
        return this;
    }

    private static ViSpan CreateRandomViSpan() =>
        new(RandomInt(0, 512), RandomInt(2, 512));

    public IAdornmentInformation Build() =>
        new AdornmentInformation(
                this._objectCreationSpan,
                this._objectCreationTypeSpan,
                false,
                this._fixtureItemId,
                string.Empty,
                string.Empty)
            .UpdateVersion(this._version);
}