namespace DemoCode.Interfaces
{
    public interface ILogger
    {
        void Log(string validateEmail);

        string LoggerName { get; }

        IValidationService ValidationService { get; set; }
    }
}