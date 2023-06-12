namespace PotionDeliveryService.Interfaces;

public interface IDestination
{
    string Recipient { get; }

    string EMail { get; }

    string Address { get; }

    CountryCode Country { get; }
}