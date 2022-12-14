using Twizzar.Fixture;
using PotionDeliveryService;
using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests
{
    partial class BaseFeaturesTests
    {
        private class BluePotionBuilder : ItemBuilder<PotionDeliveryService.Potion, Potion40f6BuilderPaths>
        {
            public BluePotionBuilder()
            {
                this.With(p => p.Ctor.color.Value(PotionColor.Blue));
                this.With(p => p.Price.Value(42));
                this.With(p => p.Ctor.ingredient1.Stub<IIngredient>());
            }
        }
    }
}