namespace PotionDeliveryService.Tests;

[TestFixture]
public class PackageTests
{
    [Test]
    public void Unwrap_package_returns_all_added_items()
    {
        var package = new Package<Potion>();

        foreach (var potion in new ItemBuilder<Potion>().BuildMany(3))
        {
            package.Add(potion);
        }

        package.Wrap();
        var content = package.UnWrap().ToList();

        Assert.That(content, Has.Count.EqualTo(3));
        Assert.That(content, Has.All.InstanceOf<Potion>());
    }

    [Test]
    public void Content_cannot_be_added_to_a_wrapped_packages()
    {
        var wrappedPackages = new ItemBuilder<Package<Potion>>()
            .With(p => p._state.Value(PackageState.Wrapped))
            .Build();

        var package = new ItemBuilder<Potion>().Build();

        Assert.Throws<InvalidOperationException>(() => 
            wrappedPackages.Add(package));
    }

    [Test]
    public void Not_wrapped_package_cannot_be_unwrapped()
    {
        foreach (var packageState in new[] {PackageState.Open, PackageState.UnWrapped})
        {
            var package = new ItemBuilder<Package<Potion>>()
                .With(p => p._state.Value(packageState))
                .Build();

            Assert.Throws<InvalidOperationException>(() =>
                package.UnWrap());
        }
    }
}