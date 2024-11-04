using System;
using System.Collections.Generic;
using System.Linq;

using DemoCode.Interfaces;

namespace DemoCode
{
    public class EmailMessageBuffer
    {
        #region private fields

        private readonly IEmailGateway _emailGateway;
        private readonly IValidationService _validationService;
        private readonly ILogger _logger;
        private readonly List<EmailMessage> _emails = new List<EmailMessage>();

        #endregion


        #region ctor
        
        public EmailMessageBuffer(IEmailGateway emailGateway, IValidationService validationService, ILogger logger)
        {
            this._emailGateway = emailGateway ?? throw new ArgumentNullException(nameof(emailGateway));
            this._validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            this._logger = logger;
        }

        #endregion

        #region properties
        
        public int UnsentMessagesCount => this._emails.Count;

        #endregion

        #region public methods

        public void Add(EmailMessage message)
        {
            _emails.Add(message);
        }

        public void SendAll()
        {
            var totalMails = _emails.Count;
            for (int i = totalMails -1; i >= 0; i--)
            {
                var email = _emails[i];
                email.FormatMail();
                Send(email);
            }
        }

        public void SendLimited(int maximumMessagesToSend)
        {
            var limitedBatchOfMessages = _emails.Take(maximumMessagesToSend).ToArray();
            var totalMails = limitedBatchOfMessages.Length;
            for (int i = totalMails - 1; i >= 0; i--)
            {
                var email = _emails[i];
                email.FormatMail();
                Send(email);
            }
        }

        #endregion

        private void Send(EmailMessage email)
        {
            if (!_validationService.Validate(email))
            {
                throw new ApplicationException("email not valid.");
            }

            _emailGateway.Send(email);

            _emails.Remove(email);

            _logger.Log("email sent.");
        }
    }
}