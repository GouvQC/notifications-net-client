using NUnit.Framework;
using PgnNotifications.Client;
using PgnNotifications.Exceptions;
using PgnNotifications.Interfaces;
using PgnNotifications.Models;
using PgnNotifications.Models.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Notify.Tests.IntegrationTests
{
    [TestFixture]
    public class NotificationClientIntegrationTests
    {
        private NotificationClient client;

        private readonly String NOTIFY_API_URL = Environment.GetEnvironmentVariable("NOTIFY_API_URL");
        private readonly String API_KEY = Environment.GetEnvironmentVariable("API_KEY");
        private readonly String CLIENT_ID = Environment.GetEnvironmentVariable("CLIENT_ID");
        private readonly String API_SENDING_KEY = Environment.GetEnvironmentVariable("API_SENDING_KEY");

        private readonly String FUNCTIONAL_TEST_NUMBER = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_NUMBER");
        private readonly String FUNCTIONAL_TEST_EMAIL = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_EMAIL");

        private readonly String EMAIL_TEMPLATE_ID = Environment.GetEnvironmentVariable("EMAIL_TEMPLATE_ID");
        private readonly String SMS_TEMPLATE_ID = Environment.GetEnvironmentVariable("SMS_TEMPLATE_ID");        
        private readonly String EMAIL_REPLY_TO_ID = Environment.GetEnvironmentVariable("EMAIL_REPLY_TO_ID");
        private readonly String SMS_SENDER_ID = Environment.GetEnvironmentVariable("SMS_SENDER_ID");
        private readonly String INBOUND_SMS_QUERY_KEY = Environment.GetEnvironmentVariable("INBOUND_SMS_QUERY_KEY");

        private String smsNotificationId;
        private String emailNotificationId;     
        const String TEST_TEMPLATE_SMS_BODY = "HELLO WORLD v2";
        const String TEST_SMS_BODY = "HELLO WORLD v2";
        const String TEST_TEMPLATE_EMAIL_BODY = "HELLO WORLD";
        const String TEST_EMAIL_BODY = "HELLO WORLD";
        const String TEST_EMAIL_SUBJECT = "BASIC";        
      
        [SetUp]
        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SetUp()
        {
            this.client = new NotificationClient(NOTIFY_API_URL, API_KEY, CLIENT_ID);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendSmsTestWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            SmsNotificationResponse response =
                this.client.SendSms(FUNCTIONAL_TEST_NUMBER, SMS_TEMPLATE_ID, personalisation, "sample-test-ref");
            this.smsNotificationId = response.id;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, TEST_SMS_BODY);

            Assert.IsNotNull(response.reference);
            Assert.AreEqual(response.reference, "sample-test-ref");
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            EmailNotificationResponse response =
                this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, EMAIL_TEMPLATE_ID, personalisation);
            this.emailNotificationId = response.id;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, TEST_EMAIL_BODY);
            Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailWithDocumentPersonalisationTest()
        {
            byte[] pdfContents;

            try
            {
                pdfContents = File.ReadAllBytes("../../../IntegrationTests/test_files/one_page_pdf.pdf");
            }
            catch (DirectoryNotFoundException)
            {
                pdfContents = File.ReadAllBytes("IntegrationTests/test_files/one_page_pdf.pdf");
            }
           
             Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            EmailNotificationResponse response =
                this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, EMAIL_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.template.id);
            Assert.IsNotNull(response.template.uri);
            Assert.IsNotNull(response.template.version);
            Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailWithCSVDocumentPersonalisationTestUsingEmailConfirmationAndRetentionPeriod()
        {
            byte[] pdfContents;

            try
            {
                pdfContents = File.ReadAllBytes("../../../IntegrationTests/test_files/one_page_pdf.pdf");
            }
            catch (DirectoryNotFoundException)
            {
                pdfContents = File.ReadAllBytes("IntegrationTests/test_files/one_page_pdf.pdf");
            }

            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            EmailNotificationResponse response =
                this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, EMAIL_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.template.id);
            Assert.IsNotNull(response.template.uri);
            Assert.IsNotNull(response.template.version);
            Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllNotifications()
        {
            NotificationList notificationsResponse = this.client.GetNotifications();
            Assert.IsNotNull(notificationsResponse);
            Assert.IsNotNull(notificationsResponse.notifications);

            List<Notification> notifications = notificationsResponse.notifications;

            foreach (Notification notification in notifications)
            {
                NotifyAssertions.AssertNotification(notification);
            }

        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetNotificationWithInvalidIdRaisesClientException()
        {
            var ex = Assert.Throws<NotifyClientException>(() =>
                this.client.GetNotificationById("fa5f0a6e-5293-49f1-b99f-3fade784382f")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetTemplateWithInvalidIdRaisesClientException()
        {
            var ex = Assert.Throws<NotifyClientException>(() =>
                this.client.GetTemplateById("id is not a valid UUID")
            );
            Assert.That(ex.Message, Does.Contain("Status code 404"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetTemplateWithIdWithoutResultRaisesClientException()
        {
            var ex = Assert.Throws<NotifyClientException>(() =>
                this.client.GetTemplateById("fa5f0a6e-5293-49f1-b99f-3fade784382f")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllTemplates()
        {
            TemplateList templateList = this.client.GetAllTemplates();
            Assert.IsNotNull(templateList);
            Assert.AreNotEqual(templateList.templates.Count, 0);

            foreach (TemplateResponse template in templateList.templates)
            {
                NotifyAssertions.AssertTemplateResponse(template);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllSMSTemplates()
        {
            const String type = "sms";
            TemplateList templateList = this.client.GetAllTemplates(type);
            Assert.IsNotNull(templateList);
            Assert.AreNotEqual(templateList.templates.Count, 0);

            foreach (TemplateResponse template in templateList.templates)
            {
                NotifyAssertions.AssertTemplateResponse(template, type);
            }
        }    

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetSMSTemplateWithId()
        {
            TemplateResponse template = this.client.GetTemplateById(SMS_TEMPLATE_ID);
            Assert.AreEqual(template.id, SMS_TEMPLATE_ID);
            Assert.AreEqual(template.body, TEST_TEMPLATE_SMS_BODY);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetEmailTemplateWithId()
        {
            TemplateResponse template = this.client.GetTemplateById(EMAIL_TEMPLATE_ID);
            Assert.AreEqual(template.id, EMAIL_TEMPLATE_ID);
            Assert.AreEqual(template.body, TEST_TEMPLATE_EMAIL_BODY);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GenerateSMSPreviewWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            TemplatePreviewResponse response =
                this.client.GenerateTemplatePreview(SMS_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.body, TEST_SMS_BODY);
            Assert.AreEqual(response.subject, null);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GenerateEmailPreviewWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            TemplatePreviewResponse response =
                this.client.GenerateTemplatePreview(EMAIL_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.body, TEST_EMAIL_BODY);
            Assert.AreEqual(response.subject, TEST_EMAIL_SUBJECT);
        }    

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestEmailReplyToNotPresent()
        {
            String fakeReplayToId = System.Guid.NewGuid().ToString();
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            var ex = Assert.Throws<NotifyClientException>(() => this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, EMAIL_TEMPLATE_ID, personalisation, emailReplyToId: fakeReplayToId));
            Assert.That(ex.Message, Does.Contain("email_reply_to_id " + fakeReplayToId));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestAllArguments()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            EmailNotificationResponse response = this.client.SendEmail(
                FUNCTIONAL_TEST_EMAIL,
                EMAIL_TEMPLATE_ID,
                personalisation,
                clientReference: "TestReference",
                emailReplyToId: EMAIL_REPLY_TO_ID,
                oneClickUnsubscribeURL: null
            );
            this.emailNotificationId = response.id;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, TEST_EMAIL_BODY);
            Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
            Assert.AreEqual(response.reference, "TestReference");
            Assert.AreEqual(response.content.oneClickUnsubscribeURL, null);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendSmsTestWithPersonalisationAndSmsSenderId()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            NotificationClient client_sending = new NotificationClient(NOTIFY_API_URL, API_SENDING_KEY, CLIENT_ID);

            SmsNotificationResponse response =
                client_sending.SendSms(FUNCTIONAL_TEST_NUMBER, SMS_TEMPLATE_ID, personalisation, "sample-test-ref", SMS_SENDER_ID);
            this.smsNotificationId = response.id;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, TEST_SMS_BODY);

            Assert.IsNotNull(response.reference);
            Assert.AreEqual(response.reference, "sample-test-ref");
        }
    }
}
