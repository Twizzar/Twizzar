﻿namespace PotionDeliveryService.Tests;

[TestFixture]
public partial class StorageTests
{
    [Test]
    public void Stored_ingredient_is_available()
    {
        // arrange
        var storage = new Storage();
        var ingredient = new UniqueIngredienBuilder().Build();

        // act
        storage.Store(ingredient);
        var isAvailable = storage.CheckAvailable(ingredient.Name);

        // assert
        Assert.That(isAvailable, Is.True);
    }

    [Test]
    public void When_stored_one_and_taken_twice_throw_InvalidOperationException()
    {
        // arrange
        var storage = new Storage();
        var ingredient = new UniqueIngredienBuilder().Build();

        // act
        storage.Store(ingredient);
        storage.Take(ingredient.Name);

        // assert
        Assert.Throws<InvalidOperationException>(() => storage.Take(ingredient.Name));
    }

    [Test]
    public void When_all_ingredient_are_taken_out_then_their_are_no_longer_available()
    {
        // arrange
        var storage = new Storage();
        var ingredient = new UniqueIngredienBuilder().Build();

        // act
        for (int i = 0; i < 5; i++)
        {
            storage.Store(ingredient);
        }

        // assert
        Assert.That(storage.CheckAvailable(ingredient.Name), Is.True);

        for (int i = 0; i < 5; i++)
        {
            storage.Take(ingredient.Name);
        }

        Assert.That(storage.CheckAvailable(ingredient.Name), Is.False);
    }
}