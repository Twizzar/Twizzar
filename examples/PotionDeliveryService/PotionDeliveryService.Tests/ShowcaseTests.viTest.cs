using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests
{
    partial class ShowcaseTests
    {
        private class VitalityPotionBuilder : ItemBuilder<PotionDeliveryService.Potion, Potionee40BuilderPaths>
        {
            public VitalityPotionBuilder()
            {
                this.With(p => p.Ctor.Name.Value("Vitality Potion"));
                this.With(p => p.Ctor.Color.Value(PotionColor.Purple));
                this.With(p => p.Ctor.Ingredient1.InstanceOf<Potion>());
                this.With(p => p.Ctor.Ingredient1.Ctor.Name.Value("Mana Potion"));
                this.With(p => p.Ctor.Ingredient1.Ctor.Color.Unique());
                this.With(p => p.Ctor.Ingredient1.Ctor.Ingredient1.Name.Value("Water"));
                this.With(p => p.Ctor.Ingredient1.Ctor.Ingredient1.Stub<IIngredient>());
                this.With(p => p.Ctor.Ingredient1.Ctor.Ingredient2.Stub<IIngredient>());
                this.With(p => p.Ctor.Ingredient1.Ctor.Effect.Stub<IEffect>());
            }
        }
        private class WrappedPackageBuilder : ItemBuilder<PotionDeliveryService.Package<PotionDeliveryService.Interfaces.IPotion>, MyPackageBuilderPaths>
        {
            public WrappedPackageBuilder()
            {
                this.With(p => p._state.Value(PackageState.Wrapped));
            }
        }
    }
}