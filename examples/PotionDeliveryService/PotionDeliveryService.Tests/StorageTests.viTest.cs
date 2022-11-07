using ViTest.Fixture;
using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests
{
    partial class StorageTests
    {
        private class UniqueIngredienBuilder : ItemBuilder<PotionDeliveryService.Interfaces.IIngredient, IIngredient8febBuilderPaths>
        {
            public UniqueIngredienBuilder()
            {
                this.With(p => p.Name.Unique());
            }
        }
    }
}