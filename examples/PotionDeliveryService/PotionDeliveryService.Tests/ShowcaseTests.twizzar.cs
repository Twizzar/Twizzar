﻿namespace PotionDeliveryService.Tests
{
    partial class ShowcaseTests
    {
        private class VitalityPotionBuilder : ItemBuilder<PotionDeliveryService.Potion, Potionee40BuilderPaths>
        {
            public VitalityPotionBuilder()
            {
                this.With(p => p.Ctor.name.Value("Vitality Potion"));
                this.With(p => p.Ctor.color.Value(PotionColor.Purple));
                this.With(p => p.Ctor.ingredient1.InstanceOf<Potion>());
                this.With(p => p.Ctor.ingredient1.Ctor.name.Value("Mana Potion"));
                this.With(p => p.Ctor.ingredient1.Ctor.ingredient1.Name.Value("Water"));
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