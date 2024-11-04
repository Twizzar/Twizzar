using System;

namespace DemoCode.Interfaces
{
    public class EmailMessage
    {
        public EmailMessage(
            string toAddress, 
            string messageBody, 
            bool isImportant, string ccAddress, string bccAddress, string header, string subject, string footer, EmailFormat format)
        {
            ToAddress = toAddress ?? throw new ArgumentNullException(nameof(toAddress));
            MessageBody = messageBody ?? throw new ArgumentNullException(nameof(messageBody));
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            IsImportant = isImportant;
            CcAddress = ccAddress;
            BccAddress = bccAddress;
            Footer = footer;
            Format = format;
        }


        public string ToAddress { get; private set; }
        public string CcAddress { get; private set; }
        public string BccAddress { get; private set; }
        public string Header { get; private set; }
        public string Subject { get; set; }
        public string MessageBody { get; private set; }
        public string Footer { get; private set; }
        public bool IsImportant { get; private set; }
        public EmailFormat Format { get; private set; }

        public void FormatMail()
        {
            switch (Format)
            {
                case EmailFormat.Html:
                    this.MessageBody = MessageBody.ToUpper();
                    break;
                case EmailFormat.RichText:
                    this.MessageBody = MessageBody.ToLower();
                    break;
                case EmailFormat.Plain:
                default:
                    break;
            }
        }
    }
}