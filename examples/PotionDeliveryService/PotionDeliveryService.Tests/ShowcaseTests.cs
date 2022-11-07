using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests;

[TestFixture]
public partial class ShowcaseTests
{
    [Test]
    public void Showcase_automatically_detects_and_resolves_dependencies_of_a_class_or_interface()
    {
        var myPotion = new ItemBuilder<Potion>()
            .Build();
    }

    [Test]
    public void Showcase_Intuitive_UI_for_dependency_configuration()
    {
        var vitalityPotion = new VitalityPotionBuilder()
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(vitalityPotion.Name, Is.EqualTo("Vitality Potion"));
            Assert.That(vitalityPotion.Color, Is.EqualTo(PotionColor.Purple));
            Assert.That(vitalityPotion.Ingredient1, Is.InstanceOf<Potion>());
            Assert.That(vitalityPotion.Ingredient2, Is.InstanceOf<Potion>());
        });

        var potion1 = (Potion)vitalityPotion.Ingredient1;
        Assert.Multiple(() =>
        {
            Assert.That(potion1.Name, Is.EqualTo("Mana Potion"));
            Assert.That(potion1.Color, Is.EqualTo(PotionColor.Blue));
            Assert.That(potion1.Ingredient1.Name, Is.EqualTo("Water"));
            Assert.That(potion1.Ingredient2.Name, Is.EqualTo("RedBerry"));
        });

        var potion2 = (Potion)vitalityPotion.Ingredient2;
        Assert.Multiple(() =>
        {
            Assert.That(potion2.Name, Is.EqualTo("Health Potion"));
            Assert.That(potion2.Color, Is.EqualTo(PotionColor.Red));
            Assert.That(potion2.Ingredient1.Name, Is.EqualTo("Water"));
            Assert.That(potion2.Ingredient2.Name, Is.EqualTo("Glowing Mushroom"));
        });
    }

    [Test]
    public void Showcase_Reusable_test_configurations()
    {
        var destination = new ItemBuilder<IDestination>().Build();

        var potion = new VitalityPotionBuilder().Build();
        var package = new Package<IPotion>();
        package.Add(potion);
        package.Wrap();

        destination.Receive(package);
    }

    [Test]
    public void Showcase_Access_to_non_public_members()
    {
        var package = new WrappedPackageBuilder().Build();

        Assert.Throws<InvalidOperationException>(() => package.Add(new ItemBuilder<IPotion>().Build()));
    }
}