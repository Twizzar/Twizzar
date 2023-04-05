﻿using PotionDeliveryService.Interfaces;
using Twizzar.Fixture;
using PotionDeliveryService;

namespace PotionDeliveryService.Tests
{
    partial class DeliveryServiceTests
    {
        private class DeliveryServiceaa96Builder : ItemBuilder<PotionDeliveryService.DeliveryService, DeliveryServiceaa96BuilderPaths>
        {
            public DeliveryServiceaa96Builder()
            {
                this.With(p => p.Ctor.potionRecipes.Stub<IPotionRecipes>());
                this.With(p => p.Ctor.potionRecipes.GetPotionRecipe.InstanceOf<ValueTuple<IIngredient, IIngredient>>());
                this.With(p => p.Ctor.potionRecipes.GetPotionRecipe.Ctor.item1.Name.Unique());
                this.With(p => p.Ctor.potionRecipes.GetPotionRecipe.Ctor.item2.Name.Unique());
                this.With(p => p.Ctor.potionRecipes.GetPotionColor.Unique());
                this.With(p => p.Ctor.potionRecipes.GetPotionEffect.Stub<IEffect>());
                this.With(p => p.Ctor.potionRecipes.GetPotionEffect.Name.Unique());
                this.With(p => p.Ctor.potionRecipes.GetPotionName.Value("MyPotion"));
                this.With(p => p.Ctor.storage.Stub<IStorage>());
                this.With(p => p.Ctor.storage.CheckAvailable.Value(true));
                this.With(p => p.Ctor.storage.Take.InstanceOf<Potion>());
                this.With(p => p.Ctor.storage.Take.Ctor.name.Value("MyPotion"));
            }
        }

        private class DeliveryServiceBuilder : ItemBuilder<PotionDeliveryService.DeliveryService, DeliveryServiceb88bBuilderPaths>
        {
        }
    }
}