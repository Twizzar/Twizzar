using Moq;
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

        Assert.That(myPotion.Name, Is.Not.Null);
        Assert.That(myPotion.Ingredient1, Is.InstanceOf<IIngredient>());
        Assert.That(myPotion.Ingredient2, Is.InstanceOf<IIngredient>());
        Assert.That(myPotion.Effect, Is.InstanceOf<IEffect>());
    }

    [Test]
    public void Showcase_Intuitive_UI_for_dependency_configuration()
    {
        var vitalityPotion = new VitalityPotionBuilder()
            .Build();

        Assert.That(vitalityPotion.Ingredient1.Name, Is.EqualTo("Mana Potion"));
        Assert.That(vitalityPotion.Name, Is.EqualTo("Vitality Potion"));
        Assert.That(vitalityPotion.Color, Is.EqualTo(PotionColor.Purple));
        Assert.That(vitalityPotion.Ingredient1, Is.InstanceOf<Potion>());

        var potion1 = (Potion)vitalityPotion.Ingredient1;

        Assert.That(potion1.Name, Is.EqualTo("Mana Potion"));
        Assert.That(potion1.Color, Is.EqualTo(PotionColor.Blue));
        Assert.That(potion1.Ingredient1.Name, Is.EqualTo("Water"));
    }

    [Test]
    public void Showcase_Cleaner_automated_testing_thanks_to_lean_arrange_section()
    {
        // using Moq
        var water = new Mock<IIngredient>();
        water
            .Setup(ingredient => ingredient.Name)
            .Returns("Water");

        var manaPotion = new Potion(
            "Mana Potion",
            water.Object,
            Mock.Of<IIngredient>(),
            Mock.Of<IEffect>(),
            PotionColor.Blue);

        var moqVitalityPotion = new Potion(
            "Vitality Potion",
            manaPotion,
            Mock.Of<IPotion>(),
            Mock.Of<IEffect>(),
            PotionColor.Purple);

        // using twizzar
        var twizzarVitalityPotion = new VitalityPotionBuilder()
            .Build();


        Assert.That(moqVitalityPotion.Color, Is.EqualTo(twizzarVitalityPotion.Color));
        Assert.That(moqVitalityPotion.Name, Is.EqualTo(twizzarVitalityPotion.Name));
        Assert.That(moqVitalityPotion.Ingredient1.Name, Is.EqualTo(twizzarVitalityPotion.Ingredient1.Name));
    }

    [Test]
    public void Resolved_VitalityPotions_are_not_equal()
    {
        var potion1 = new VitalityPotionBuilder().Build();
        var potion2 = new VitalityPotionBuilder().Build();

        Assert.That(potion1, Is.Not.EqualTo(potion2));
    }

    [Test]
    public void VitalityPotion_gets_delivered()
    {
        var potion = new VitalityPotionBuilder().Build();

        var destination = new ItemBuilder<IDestination>().Build();
        var package = new Package<IPotion>(potion);

        destination.Receive(package);
    }

    [Test]
    [Ignore("Fails on build due to a bug, see https://github.com/Twizzar/Twizzar/issues/21.")]
    public void Showcase_Access_to_non_public_members()
    {
        var package = new WrappedPackageBuilder().Build();

        Assert.Throws<InvalidOperationException>(() => package.Add(new ItemBuilder<IPotion>().Build()));
    }

    #endregion
}