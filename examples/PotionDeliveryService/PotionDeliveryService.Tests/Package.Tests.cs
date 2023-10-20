namespace PotionDeliveryService.Tests;

[TestFixture]
public class PackageTests
{
    [Test]
    [TestSource(nameof(Package<Potion>.UnWrap))]
    public void Unwrap_package_returns_all_added_items()
    {
        // arrange
        var package = new Package<Potion>();

        // add 3 potions to the packages.
        // here we can use the BuildMany method from Twizzar to generate 3 unique positions
        foreach (var potion in new ItemBuilder<Potion>().BuildMany(3))
        {
            package.Add(potion);
        }

        // act

        // first wrap the package
        package.Wrap();

        // then unwrap the package to get the content
        var content = package.UnWrap().ToList();

        // assert
        Assert.That(content, Has.Count.EqualTo(3));
        Assert.That(content, Has.All.InstanceOf<Potion>());
    }

    [Test]
    [TestSource(nameof(Package<Potion>.Add))]
    public void Content_cannot_be_added_to_a_wrapped_packages()
    {
        // arrange

        // create a package with the state Wrapped
        var wrappedPackages = new ItemBuilder<Package<Potion>>()
            .With(p => p._state.Value(PackageState.Wrapped))
            .Build();

        var package = new ItemBuilder<Potion>().Build();

        // act & assert
        Assert.Throws<InvalidOperationException>(() =>
            wrappedPackages.Add(package));
    }

    [TestCase(PackageState.Open)]
    [TestCase(PackageState.UnWrapped)]
    [TestSource(nameof(Package<Potion>.Add))]
    public void Not_wrapped_package_cannot_be_unwrapped(PackageState packageState)
    {
        // arrange

        // set the private field _state to the test case packageState.
        var package = new ItemBuilder<Package<Potion>>()
            .With(p => p._state.Value(packageState))
            .Build();

        // act & assert
        Assert.Throws<InvalidOperationException>(() =>
            package.UnWrap());
    }
}