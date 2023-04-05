﻿using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests
{
    partial class CauldronTests
    {
        private class TestCauldronBuilder : ItemBuilder<PotionDeliveryService.Cauldron, Caldron3020BuilderPaths>
        {
            public TestCauldronBuilder()
            {
                this.With(p => p.Ctor.recipes.Stub<IPotionRecipes>());
                this.With(p => p.Ctor.recipes.GetPotionColor.Value(PotionColor.Purple));
                this.With(p => p.Ctor.recipes.GetPotionName.Value("Test Name"));
                this.With(p => p.Ctor.recipes.GetPotionEffect.Stub<IEffect>());
                this.With(p => p.Ctor.recipes.GetPotionEffect.Name.Value("Test Effect"));
            }
        }
    }
}