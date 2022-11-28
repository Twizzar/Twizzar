﻿namespace PotionDeliveryService.Tests
{
    partial class StorageTests
    {
        private class UniqueIngredientBuilder : ItemBuilder<PotionDeliveryService.Interfaces.IIngredient, IIngredient8febBuilderPaths>
        {
            public UniqueIngredientBuilder()
            {
                this.With(p => p.Name.Unique());
            }
        }
    }
}