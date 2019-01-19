using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//We're using a template, so we need IO
using System.IO;

//We've got emails, so we need to use mailkit
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MailKit.Search;

//And since we need MimeMessages, we need MimeKit too.
using MimeKit;

//And because we're storing login details in memory, we want secure strings.
using System.Security;

//We're using an XML document to store our login details, so we need an XML interface.
using System.Xml.Linq;

//For fancy formatting, it's gotta be FormatWith
namespace SmartMailbox.Outputs
{
    class EmailOutput : IOutputComponent
    {
        private EmailSystem emailSystem = new EmailSystem();

        void SendSpam(string subject, string summary, string image)
        {
            emailSystem.SendSmartboxEmail(image, @"<p>" + summary + @"</p><img src=""cid:{0}"">", "SPAM from " + subject);
        }

        void SendReal(string subject, string summary, string image)
        {
            emailSystem.SendSmartboxEmail(image, @"<p>" + summary + @"</p><img src=""cid:{0}"">", subject);
        }

        public void HandleClassification(Classification c)
        {
            //TODO: Write handling code
        }

        static void Main(string[] args)
        {
            EmailOutput output = new EmailOutput();

            output.SendReal("Lidl", "Very cheap banans: 1kg for 3 pounds", "lidl.jpg");
            output.SendSpam("Lidl", "Very cheap banans: 1kg for 3 pounds", "lidl.jpg");
        }
    }

    class EmailSystem : IDisposable
    {
        //private const string TestValue = @"Z:\TiVo button better dimensions.stl";
        //private const string TestValue = @"D:\Fate system analysis 2.txt";
        //private const string LoginFile = "./Data/EmailLogin.xml";
        //private const string MalformedRequestTemplate = "./Data/MalformedRequestTemplate.txt";
        //private const string ConfirmationEmailTemplate = "./Data/ConfirmationEmailTemplate.txt";

        private string ImapHostname;
        private int ImapPort;
        private string ImapUsername;
        private string ImapPassword;
        private MailboxAddress InAddress;

        private string SmtpHostname;
        private int SmtpPort;
        private string SmtpUsername;
        private string SmtpPassword;
        private MailboxAddress OutAddress;

        private SmtpClient _smtp;
        private ImapClient _imap;

        /// <summary>
        /// Attribute used to make smtp persistent, and make sure it autoconnects as required if disconnected.
        /// </summary>
        private SmtpClient smtp
        {
            get
            {
                if (_smtp.IsConnected && !_smtp.IsAuthenticated)
                {
                    _smtp.Disconnect(true);
                }
                if (!_smtp.IsConnected)
                {
                    _smtp.Connect(SmtpHostname, SmtpPort, SecureSocketOptions.StartTls);
                    _smtp.Authenticate(SmtpUsername, SmtpPassword);
                }
                return _smtp;
            }
        }

        private ImapClient imap
        {
            get
            {
                if (_imap.IsConnected && !_imap.IsAuthenticated)
                {
                    _imap.Disconnect(true);
                }
                if (!_imap.IsConnected)
                {
                    _imap.Connect(ImapHostname, ImapPort, SecureSocketOptions.SslOnConnect);
                    _imap.Authenticate(ImapUsername, ImapPassword);
                }
                return _imap;
            }
        }

        public EmailSystem()
        {
            //Load logins
            //XDocument logindoc = XDocument.Load(LoginFile);

            //XElement smtpelement = logindoc.Root.Descendants("Login").Single(x => x.Attribute("Type").Value == "SMTP");
            SmtpHostname = "Smtp.gmail.com";//smtpelement.Descendants("Hostname").Single().Value;
            SmtpPort = 587;//int.Parse(smtpelement.Descendants("Port").Single().Value);
            SmtpUsername = "smartmailbox2019";//smtpelement.Descendants("Username").Single().Value;
            SmtpPassword = "hackcambridge2019";//smtpelement.Descendants("Password").Single().Value;
            string SmtpName = "Smartbox";//smtpelement.Descendants("DisplayName").Single().Value;
            string SmtpAddress = @"smartmailbox2019@gmail.com";//smtpelement.Descendants("EmailAddress").Single().Value;
            OutAddress = new MailboxAddress(SmtpName, SmtpAddress);


            /*XElement imapelement = logindoc.Root.Descendants("Login").Single(x => x.Attribute("Type").Value == "IMAP");
            ImapHostname = imapelement.Descendants("Hostname").Single().Value;
            ImapPort = int.Parse(imapelement.Descendants("Port").Single().Value);
            ImapUsername = imapelement.Descendants("Username").Single().Value;
            ImapPassword = imapelement.Descendants("Password").Single().Value;
            string ImapName = smtpelement.Descendants("DisplayName").Single().Value;
            string ImapAddress = smtpelement.Descendants("EmailAddress").Single().Value;
            InAddress = new MailboxAddress(ImapName, ImapAddress);*/

            //Now create our login clients
            //_imap = new ImapClient();
            _smtp = new SmtpClient();
        }


        /// <summary>
        /// Sends a series of emails in one go, with a single connection.
        /// </summary>
        /// <param name="emails">A list of fully-formed MimeMessages ready to send</param>
        public void SendEmails(List<MimeMessage> emails)
        {
            if (emails.Count == 0)
            {
                return;
            }
            foreach (MimeMessage message in emails)
            {
                smtp.Send(message);
            }
        }

        /// <summary>
        /// Makes an email from the DisplayName, EmailAddress, Subject and Body.
        /// </summary>
        /// <param name="DisplayName">DisplayName of the user you want to compose an email to</param>
        /// <param name="EmailAddress">EmailAddress of the user you want to compose an email to</param>
        /// <param name="Subject">Subject of the email</param>
        /// <param name="Body">Body of the email</param>
        /// <returns>Fully-formed MimeMessage object ready to send</returns>
        private MimeMessage MakeMail(string DisplayName, string EmailAddress, string Subject, string Body)
        {
            BodyBuilder builder = new BodyBuilder();
            builder.TextBody = Body;
            MimeMessage mail = new MimeMessage();
            mail.From.Add(OutAddress);
            mail.To.Add(new MailboxAddress(DisplayName, EmailAddress));
            mail.Subject = Subject;
            mail.Body = builder.ToMessageBody();

            return mail;
        }

        public void SendSmartboxEmail(string imageName, string HTMLbody, String subject)
        {
            MimeMessage message = new MimeMessage();
            var builder = new BodyBuilder();
            var pathImage = Path.Combine(Environment.CurrentDirectory, imageName);
            var image = builder.LinkedResources.Add(pathImage);

            image.ContentId = MimeKit.Utils.MimeUtils.GenerateMessageId();

            builder.HtmlBody = string.Format(HTMLbody, image.ContentId);
            message.From.Add(OutAddress);
            message.To.Add(new MailboxAddress("Alan Marko", "am2677@cam.ac.uk"));
            message.Subject = subject;
            message.Body = builder.ToMessageBody();

            List<MimeMessage> emails = new List<MimeMessage>();
            emails.Add(message);
            SendEmails(emails);
        }
        /// <summary>
        /// Makes a reply to an existing email
        /// </summary>
        /// <param name="DisplayName">The display name of the receiving user</param>
        /// <param name="EmailAddress">The email address of the receiving user</param>
        /// <param name="Body">The body of the email you would like to send</param>
        /// <param name="EmailToReplyTo">The MimeMessage representing the email you wish to reply to</param>
        /// <returns></returns>
        private MimeMessage MakeReply(string DisplayName, string EmailAddress, string Body, MimeMessage EmailToReplyTo)
        {
            string Subject;
            if (EmailToReplyTo.Subject.StartsWith("RE:", StringComparison.OrdinalIgnoreCase))
            {
                Subject = EmailToReplyTo.Subject;
            }
            else
            {
                Subject = "RE: " + EmailToReplyTo.Subject;
            }
            MimeMessage mail = MakeMail(DisplayName, EmailAddress, Subject, Body);
            if (!string.IsNullOrEmpty(EmailToReplyTo.MessageId))
            {
                mail.InReplyTo = EmailToReplyTo.MessageId;
                foreach (var id in EmailToReplyTo.References)
                {
                    mail.References.Add(id);
                }
                mail.References.Add(EmailToReplyTo.MessageId);
            }
            return mail;
        }


        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="EmailAddress">The email address of the receiving user</param>
        /// <param name="Subject">The subject of the email</param>
        /// <param name="Body">The body of the email</param>
        public void SendEmail(string DisplayName, string EmailAddress, string Subject, string Body)
        {
            smtp.Send(MakeMail(DisplayName, EmailAddress, Subject, Body));
        }

        public void Dispose()
        {
            _smtp.Disconnect(true);
            _imap.Disconnect(true);
            _smtp.Dispose();
            _imap.Dispose();
        } //Used to dispose of the email clients.

    }
}