using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PgnNotifications.Client;
using PgnNotifications.Exceptions;
using PgnNotifications.Interfaces;
using PgnNotifications.Models;
using PgnNotifications.Models.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using Moq;


namespace Notify.Tests.IntegrationTests
{
    [TestFixture]
    public class NotificationClientIntegrationTests
    {
        //private NotificationClient client;
        private Mock<INotificationClient> mockClient;
        private string mockPhoneNumber = "+15145550000";
        private string mockEmail = "fake@example.com";
        private string mockTemplateId = "mock-sms-template";
        private string mockTemplateIdEmail = "mock-email-template";
        private string mockReplyToId = "mock-reply-to-id";
        private string mockReference = "sample-test-ref";
        private string mockBulkReference = "bulk_ref_integration_test";
        private string mockCsvBulkReference = "bulk_ref_integration_test_csv";
        private string mockNotificationId = "mock-notification-id";


        // private readonly String NOTIFY_API_URL = Environment.GetEnvironmentVariable("NOTIFY_API_URL");
        // private readonly String API_KEY = Environment.GetEnvironmentVariable("API_KEY");
        // private readonly String CLIENT_ID = Environment.GetEnvironmentVariable("CLIENT_ID");
        // private readonly String API_SENDING_KEY = Environment.GetEnvironmentVariable("API_SENDING_KEY");

        // private readonly String FUNCTIONAL_TEST_NUMBER = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_NUMBER");
        // private readonly String FUNCTIONAL_TEST_EMAIL = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_EMAIL");

        // private readonly String EMAIL_TEMPLATE_ID = Environment.GetEnvironmentVariable("EMAIL_TEMPLATE_ID");
        // private readonly String SMS_TEMPLATE_ID = Environment.GetEnvironmentVariable("SMS_TEMPLATE_ID");        
        // private readonly String EMAIL_REPLY_TO_ID = Environment.GetEnvironmentVariable("EMAIL_REPLY_TO_ID");
        // private readonly String SMS_SENDER_ID = Environment.GetEnvironmentVariable("SMS_SENDER_ID");
        // private readonly String INBOUND_SMS_QUERY_KEY = Environment.GetEnvironmentVariable("INBOUND_SMS_QUERY_KEY");

        const String TEST_TEMPLATE_SMS_BODY = "HELLO WORLD v2";
        const String TEST_SMS_BODY = "HELLO WORLD v2";
        const String TEST_TEMPLATE_EMAIL_BODY = "HELLO WORLD";
        const String TEST_EMAIL_BODY = "HELLO WORLD";
        const String TEST_EMAIL_SUBJECT = "BASIC";        
      
        [SetUp]
        //[Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SetUp()
        {
            mockClient = new Mock<INotificationClient>();
        }


        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendSmsTestWithPersonalisation()
        {
            // Étape 1 : Configurer le mock avant l'appel réel
            mockClient
                .Setup(c => c.SendSms(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new SmsNotificationResponse
                {
                    content = new SmsNotificationContent { body = TEST_SMS_BODY },
                    reference = "sample-test-ref"
                });

            // Étape 2 : Exécuter comme si réel
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            SmsNotificationResponse response =
                this.client.SendSms(mockPhoneNumber,  mockTemplateId, personalisation, "sample-test-ref");

            // Étape 3 : Assertions
            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, TEST_SMS_BODY);
            Assert.IsNotNull(response.reference);
            Assert.AreEqual(response.reference, "sample-test-ref");
        }


        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestWithPersonalisation_Mocked()
        {
            // Arrange
            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            var expectedResponse = new EmailNotificationResponse
            {
                content = new EmailResponseContent
                {
                    body = "HELLO WORLD",
                    subject = "BASIC",
                    oneClickUnsubscribeURL = null
                },
            };

            var mockClient = new Mock<INotificationClient>();
            mockClient
                .Setup(c => c.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>()))
                .Returns(expectedResponse);

            var client = mockClient.Object;

            // Act
            var response = client.SendEmail(mockEmail, mockTemplateIdEmail, personalisation);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual("HELLO WORLD", response.content.body);
            Assert.AreEqual("BASIC", response.content.subject);
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

            // Arrange
            var fakeResponse = new EmailNotificationResponse
            {
                id = Guid.NewGuid().ToString(),
                template = new Template
                {
                    id = mockTemplateIdEmail,
                    uri = "http://fake.template.uri",
                    version = 1
                },
                content = new EmailNotificationResponse.Content
                {
                    subject = TEST_EMAIL_SUBJECT,
                    body = TEST_EMAIL_BODY
                }
            };

            mockClient.Setup(x =>
                x.SendEmail(mockEmail, mockTemplateIdEmail, personalisation, null, null, null)
            ).Returns(fakeResponse);

            // Act
            EmailNotificationResponse response =
                client.SendEmail(mockEmail, mockTemplateId, personalisation);

            // Assert
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

            // Arrange
            var fakeResponse = new EmailNotificationResponse
            {
                id = Guid.NewGuid().ToString(),
                template = new Template
                {
                    id = mockTemplateIdEmail,
                    uri = "http://fake.template.uri",
                    version = 1
                },
                content = new EmailNotificationResponse.Content
                {
                    subject = TEST_EMAIL_SUBJECT,
                    body = TEST_EMAIL_BODY
                }
            };

            mockClient.Setup(x =>
                x.SendEmail(mockEmail, mockTemplateIdEmail, personalisation, null, null, null)
            ).Returns(fakeResponse);

            // Act
            EmailNotificationResponse response =
                client.SendEmail(mockEmail, mockTemplateId, personalisation);

            // Assert
            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.template.id);
            Assert.IsNotNull(response.template.uri);
            Assert.IsNotNull(response.template.version);
            Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
        }


        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendBulkNotificationsWithRows_WorksAsExpected()
        {
            // Valeurs factices pour le test
            var mockReference = "bulk_ref_integration_test";
            var mockName = "Test Bulk Notification Integration";

            var rows = new List<List<string>>
            {
                new List<string> { "email_address", "name" },
                new List<string> { mockEmail, "Name Test" },
                new List<string> { mockEmail, "Name Test" }
            };

            // Arrange - simuler une réponse HTTP 200 OK
            var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            mockClient.Setup(x =>
                x.SendBulkNotifications(
                    mockTemplateId,
                    mockName,
                    rows,
                    mockReference
                )
            ).Returns(fakeResponse);

            // Act
            var response = client.SendBulkNotifications(
                templateId: mockTemplateId,
                name: mockName,
                rows: rows,
                reference: mockReference
            );

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }


        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllNotifications()
        {
            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.GetNotifications()).Returns(
                new NotificationList
                {
                    notifications = new List<Notification> {
                        new Notification { id = "mock-id-1" },
                        new Notification { id = "mock-id-2" }
                    }
                }
            );

            var notificationsResponse = mockNotificationClient.Object.GetNotifications();
            Assert.IsNotNull(notificationsResponse);
            Assert.IsNotNull(notificationsResponse.notifications);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetNotificationWithInvalidIdRaisesClientException()
        {
            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.GetNotificationById(It.IsAny<string>())).Throws(new NotifyClientException("No result found"));

            var ex = Assert.Throws<NotifyClientException>(() =>
                mockNotificationClient.Object.GetNotificationById("invalid-id")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetTemplateWithInvalidIdRaisesClientException()
        {
            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.GetTemplateById(It.IsAny<string>())).Throws(new NotifyClientException("Status code 404"));

            var ex = Assert.Throws<NotifyClientException>(() =>
                mockNotificationClient.Object.GetTemplateById("id is not a valid UUID")
            );
            Assert.That(ex.Message, Does.Contain("Status code 404"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetTemplateWithIdWithoutResultRaisesClientException()
        {
            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.GetTemplateById(It.IsAny<string>())).Throws(new NotifyClientException("No result found"));

            var ex = Assert.Throws<NotifyClientException>(() =>
                mockNotificationClient.Object.GetTemplateById("non-existent-id")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllTemplates()
        {
            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.GetAllTemplates()).Returns(
                new TemplateList
                {
                    templates = new List<TemplateResponse> {
                        new TemplateResponse { id = "mock-template-id-1" },
                        new TemplateResponse { id = "mock-template-id-2" }
                    }
                }
            );

            TemplateList templateList = mockNotificationClient.Object.GetAllTemplates();
            Assert.IsNotNull(templateList);
            Assert.AreNotEqual(templateList.templates.Count, 0);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllSMSTemplates()
        {
            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.GetAllTemplates("sms")).Returns(
                new TemplateList
                {
                    templates = new List<TemplateResponse> {
                        new TemplateResponse { id = "sms-template-id", body = "test body" }
                    }
                }
            );

            TemplateList templateList = mockNotificationClient.Object.GetAllTemplates("sms");
            Assert.IsNotNull(templateList);
            Assert.AreNotEqual(templateList.templates.Count, 0);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetSMSTemplateWithId()
        {
            const string mockTemplateId = "mock-sms-template";
            const string mockBody = "HELLO WORLD v2";

            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.GetTemplateById(mockTemplateId)).Returns(
                new TemplateResponse { id = mockTemplateId, body = mockBody }
            );

            TemplateResponse template = mockNotificationClient.Object.GetTemplateById(mockTemplateId);
            Assert.AreEqual(template.id, mockTemplateId);
            Assert.AreEqual(template.body, mockBody);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetEmailTemplateWithId()
        {
            const string mockTemplateId = "mock-email-template";
            const string mockBody = "HELLO WORLD";

            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.GetTemplateById(mockTemplateId)).Returns(
                new TemplateResponse { id = mockTemplateId, body = mockBody }
            );

            TemplateResponse template = mockNotificationClient.Object.GetTemplateById(mockTemplateId);
            Assert.AreEqual(template.id, mockTemplateId);
            Assert.AreEqual(template.body, mockBody);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GenerateSMSPreviewWithPersonalisation()
        {
            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.GenerateTemplatePreview("mock-sms-template", It.IsAny<Dictionary<string, dynamic>>())).Returns(
                new TemplatePreviewResponse { body = "HELLO WORLD v2" }
            );

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> { { "name", "someone" } };
            TemplatePreviewResponse response = mockNotificationClient.Object.GenerateTemplatePreview("mock-sms-template", personalisation);
            Assert.IsNotNull(response);
            Assert.AreEqual(response.body, "HELLO WORLD v2");
            Assert.AreEqual(response.subject, null);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GenerateEmailPreviewWithPersonalisation()
        {
            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.GenerateTemplatePreview("mock-email-template", It.IsAny<Dictionary<string, dynamic>>())).Returns(
                new TemplatePreviewResponse { body = "HELLO WORLD", subject = "BASIC" }
            );

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> { { "name", "someone" } };
            TemplatePreviewResponse response = mockNotificationClient.Object.GenerateTemplatePreview("mock-email-template", personalisation);
            Assert.IsNotNull(response);
            Assert.AreEqual(response.body, "HELLO WORLD");
            Assert.AreEqual(response.subject, "BASIC");
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestEmailReplyToNotPresent()
        {
            var fakeReplyToId = Guid.NewGuid().ToString();
            var mockNotificationClient = new Mock<INotificationClient>();
            mockNotificationClient.Setup(client => client.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>(), It.IsAny<string>(), fakeReplyToId, null))
                .Throws(new NotifyClientException($"email_reply_to_id {fakeReplyToId}"));

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> { { "name", "someone" } };
            var ex = Assert.Throws<NotifyClientException>(() =>
                mockNotificationClient.Object.SendEmail("fake@example.com", "mock-email-template", personalisation, emailReplyToId: fakeReplyToId)
            );
            Assert.That(ex.Message, Does.Contain($"email_reply_to_id {fakeReplyToId}"));
        }


        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestAllArguments()
        {
            var mockReplyToId = "mock-reply-to-id";

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> { { "name", "someone" } };

            mockNotificationClient.Setup(client => client.SendEmail(
                mockEmail,
                mockTemplateIdEmail,
                personalisation,
                It.IsAny<string>(),
                mockReplyToId,
                It.IsAny<string>()
                )).Returns(new EmailNotificationResponse
                {
                    content = new EmailResponseContent
                    {
                        body = "HELLO WORLD",
                        subject = "BASIC",
                        oneClickUnsubscribeURL = null
                    },
                    reference = "TestReference"
                });

            EmailNotificationResponse response = mockNotificationClient.Object.SendEmail(
                mockEmail,
                mockTemplateIdEmail,
                personalisation,
                reference: "TestReference",
                emailReplyToId: mockReplyToId,
                oneClickUnsubscribeURL: null
            );

            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, "HELLO WORLD");
            Assert.AreEqual(response.content.subject, "BASIC");
            Assert.AreEqual(response.reference, "TestReference");
            Assert.AreEqual(response.content.oneClickUnsubscribeURL, null);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendSmsTestWithPersonalisationAndSmsSenderId()
        {
            var mockSmsSenderId = "mock-sender-id";

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> { { "name", "someone" } };

            mockNotificationClient.Setup(client => client.SendSms(
                mockPhoneNumber,
                mockTemplateId,
                personalisation,
                It.IsAny<string>(),
                mockSmsSenderId
            )).Returns(new SmsNotificationResponse
            {
                content = new SmsResponseContent
                {
                    body = "HELLO WORLD v2",
                    fromNumber = "+15145550000"
                },
                reference = "sample-test-ref"
            });

            SmsNotificationResponse response = mockNotificationClient.Object.SendSms(
                mockPhoneNumber,
                mockTemplateId,
                personalisation,
                reference: "sample-test-ref",
                smsSenderId: mockSmsSenderId
            );

            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, "HELLO WORLD v2");
            Assert.IsNotNull(response.reference);
            Assert.AreEqual(response.reference, "sample-test-ref");
        }

    }
}
