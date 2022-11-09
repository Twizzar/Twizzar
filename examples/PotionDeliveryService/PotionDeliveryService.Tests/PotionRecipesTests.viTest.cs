﻿using System.Collections.Immutable;

using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests
{
    partial class PotionRecipesTests
    {
        private class EmptyPotionRecipesBuilder : ItemBuilder<PotionDeliveryService.PotionRecipes, PotionRecipesdaf2BuilderPaths>
        {
            public EmptyPotionRecipesBuilder()
            {
                this.With(p => p.Ctor.potions.Value(ImmutableDictionary<string, IPotion>.Empty));

                this.With(p =>
                    p.Ctor.ingredientsToPotions.Value(ImmutableDictionary<(IIngredient, IIngredient), IPotion>.Empty));
            }
        }
    }
}