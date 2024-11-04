using System;

using DemoCode.Interfaces;

namespace DemoCode
{
    public class ValidationService : IValidationService
    {
        private ILogger _logger;

        public ValidationService(ILogger logger)
        {            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool Validate(EmailMessage emailMessage)
        {
            this._logger.Log("Validate email.");

            // validate body
            if (string.IsNullOrEmpty(emailMessage.MessageBody)
                || emailMessage.MessageBody.ToLower().StartsWith("spam")
                || emailMessage.MessageBody.ToLower().StartsWith("buy"))
            {
                return false;
            }

            // validate header
            if (!emailMessage.Header.Contains("This is a valid email header"))
            {
                return false;
            }

            // validate address
            if (string.IsNullOrEmpty(emailMessage.ToAddress))
            {
                return false;
            }

            // validate subject
            if (string.IsNullOrEmpty(emailMessage.Subject))
            {
                return false;
            }

            // validate all mail address not used
            if (emailMessage.CcAddress != "all@company.com" || 
                emailMessage.BccAddress != "all@company.com")
            {
                return false;
            }

            return true;
        }
    }
}
