namespace DemoCode.Interfaces
{
    public interface IEmailGateway
    {
        void Send(EmailMessage message);
    }
}