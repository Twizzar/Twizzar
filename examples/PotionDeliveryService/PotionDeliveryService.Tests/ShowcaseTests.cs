using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests;

/// <summary>
/// Test to showcase some key features of Twizzar:
///
/// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
/// ┃ Automatically detects and resolves dependencies of a class or interface.┃
/// ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
/// Twizzar automatically detects dependencies and can pull up classes, structs and interfaces.Instead of writing tests in the traditional way,
/// Twizzar also resolves dependencies automatically.However, if desired, they can be configured manually.
/// 
/// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
/// ┃ Intuitive UI for dependency configuration ┃
/// ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
/// If needed, dependencies can be easily configured via a user-friendly UI or a provided API.
/// 
/// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
/// ┃ Access to non-public members ┃
/// ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
/// Easily access and modify all members of a class in your project, regardless of their access modifier.
///
/// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
/// ┃ Cleaner automated testing thanks to lean arrange section ┃
/// ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
/// Write clear and maintainable unit tests by automatically outsourcing the generation of the SUT and DoCs to a builder.
///
/// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
/// ┃ Works with every testing framework ┃
/// ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
/// 
/// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
/// Twizzar runs on all popular testing frameworks such as NUnit, xUnit, MS Test or Resharper Test Runner.
/// Simultaneous use of other testing power tools such as NCrunch also works smoothly.
///
/// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
/// ┃ Reusable test configurations ┃
/// ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
/// Save time by reusing your own test configurations.

/// </summary>
[TestFixture]
public partial class ShowcaseTests
{
    #region members

    [Test]
    public void Showcase_automatically_detects_and_resolves_dependencies_of_a_class_or_interface()
    {
        var myPotion = new ItemBuilder<Potion>()
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(myPotion.Name, Is.Not.Null);
            Assert.That(myPotion.Ingredient1, Is.InstanceOf<IIngredient>());
            Assert.That(myPotion.Ingredient2, Is.InstanceOf<IIngredient>());
            Assert.That(myPotion.Effect, Is.InstanceOf<IEffect>());
        });
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

        Assert.Pass();
    }

    [Test]
    public void Showcase_Access_to_non_public_members()
    {
        var package = new WrappedPackageBuilder().Build();

        Assert.Throws<InvalidOperationException>(() => package.Add(new ItemBuilder<IPotion>().Build()));
    }

    #endregion
}