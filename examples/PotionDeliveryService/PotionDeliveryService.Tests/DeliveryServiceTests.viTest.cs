using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests
{
    partial class DeliveryServiceTests
    {
        private class DeliveryServiceaa96Builder : ItemBuilder<PotionDeliveryService.DeliveryService, DeliveryServiceaa96BuilderPaths>
        {
            public DeliveryServiceaa96Builder()
            {
                this.With(p => p.Ctor.potionRecipes.Stub<IPotionRecipes>());
                this.With(p => p.Ctor.potionRecipes.GetPotionRecipe_ValueTupleIIngredientIIngredient.InstanceOf<ValueTuple<IIngredient, IIngredient>>());
                this.With(p => p.Ctor.potionRecipes.GetPotionRecipe_ValueTupleIIngredientIIngredient.Ctor.item1.Name.Value("Test Ingredient 1"));
                this.With(p => p.Ctor.potionRecipes.GetPotionRecipe_ValueTupleIIngredientIIngredient.Ctor.item2.Name.Value("Test Ingredient 2"));
                this.With(p => p.Ctor.potionRecipes.GetPotionColor_PotionColor.Value(PotionColor.Blue));
                this.With(p => p.Ctor.potionRecipes.GetPotionEffect_IEffect.Stub<IEffect>());
                this.With(p => p.Ctor.potionRecipes.GetPotionEffect_IEffect.Name.Value("MyPotionEffect"));
                this.With(p => p.Ctor.potionRecipes.GetPotionName_String.Value("MyPotion"));
                this.With(p => p.Ctor.storage.Stub<IStorage>());
                this.With(p => p.Ctor.storage.CheckAvailable_Boolean.Value(true));
                this.With(p => p.Ctor.storage.Take_IIngredient.InstanceOf<Potion>());
                this.With(p => p.Ctor.storage.Take_IIngredient.Ctor.Name.Value("MyPotion"));
            }
        }
    }
}