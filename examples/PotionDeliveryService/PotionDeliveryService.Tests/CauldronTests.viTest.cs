using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests
{
    partial class CauldronTests
    {
        private class TestCaldronBuilder : ItemBuilder<PotionDeliveryService.Cauldron, Caldron3020BuilderPaths>
        {
            public TestCaldronBuilder()
            {
                this.With(p => p.Ctor.recipes.Stub<IPotionRecipes>());
                this.With(p => p.Ctor.recipes.GetPotionColor_PotionColor.Value(PotionColor.Purple));
                this.With(p => p.Ctor.recipes.GetPotionName_String.Value("Test Name"));
                this.With(p => p.Ctor.recipes.GetPotionEffect_IEffect.Stub<IEffect>());
                this.With(p => p.Ctor.recipes.GetPotionEffect_IEffect.Name.Value("Test Effect"));
            }
        }
    }
}