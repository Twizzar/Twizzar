using System;

using DemoCode.Interfaces;

namespace DemoCode
{
    internal class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public string LoggerName { get; set; }
        public IValidationService ValidationService { get; set; }
    }
}