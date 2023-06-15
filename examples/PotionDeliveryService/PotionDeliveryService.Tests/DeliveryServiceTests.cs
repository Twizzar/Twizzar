using Moq;
using NUnit.Framework;
using Twizzar.Fixture;
using PotionDeliveryService;
using PotionDeliveryService.Interfaces;

namespace PotionDeliveryService.Tests;

[TestFixture]
public partial class DeliveryServiceTests
{
    [Test]
    public void Deliver_sends_package_to_destination_over_parcelService()
    {
        // arrange

        // act
        // deliveryService.Deliver("Mana Potion", destination);

        // assert
    }
}