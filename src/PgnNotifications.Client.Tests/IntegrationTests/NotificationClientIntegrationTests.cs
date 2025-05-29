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
        private Mock<INotificationClient> mockNotificationClient;
        private INotificationClient client;
        private string mockPhoneNumber = "+15145550000";
        private string mockEmail = "fake@example.com";
        private string mockTemplateId = "mock-sms-template";
        private string mockTemplateIdEmail = "mock-email-template";
        private string mockReplyToId = "mock-reply-to-id";
        private string mockReference = "sample-test-ref";
        private string mockSmsSenderId = "mock-sender-id";

        const String TEST_TEMPLATE_SMS_BODY = "HELLO WORLD v2";
        const String TEST_SMS_BODY = "HELLO WORLD v2";
        const String TEST_TEMPLATE_EMAIL_BODY = "HELLO WORLD";
        const String TEST_EMAIL_BODY = "HELLO WORLD";
        const String TEST_EMAIL_SUBJECT = "BASIC";        
      
        [SetUp]
        //[Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SetUp()
        {
                mockNotificationClient = new Mock<INotificationClient>();
                client = mockNotificationClient.Object;
        }


        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendSmsTestWithPersonalisation()
        {
            mockNotificationClient
                .Setup(c => c.SendSms(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new SmsNotificationResponse
                {
                    content = new SmsResponseContent { body = TEST_SMS_BODY, fromNumber = mockPhoneNumber },
                    reference = mockReference
                });

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            SmsNotificationResponse response = mockNotificationClient.Object.SendSms(
                mockPhoneNumber,
                mockTemplateId,
                personalisation,
                mockReference,
                null // smsSenderId
            );

            Assert.IsNotNull(response);
            Assert.AreEqual(TEST_SMS_BODY, response.content.body);
            Assert.IsNotNull(response.reference);
            Assert.AreEqual(mockReference, response.reference);
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
                reference = "test-reference"
            };

            mockNotificationClient
                .Setup(c => c.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .Returns(expectedResponse);

            // Act
            var response = client.SendEmail(
                mockEmail,
                mockTemplateIdEmail,
                personalisation,
                null, // reference
                null, // emailReplyToId
                null, // oneClickUnsubscribeURL
                null, // scheduledFor
                null, // importance
                null  // ccAddress
            );

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

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
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
                content = new EmailResponseContent
                {
                    subject = TEST_EMAIL_SUBJECT,
                    body = TEST_EMAIL_BODY
                },
                reference = mockReference
            };

            mockNotificationClient.Setup(x =>
                x.SendEmail(
                    mockEmail,
                    mockTemplateIdEmail,
                    personalisation,
                    mockReference,
                    mockReference,
                    null,
                    null,
                    null,
                    null
                )
            ).Returns(fakeResponse);

            // Act
            EmailNotificationResponse response = client.SendEmail(
                mockEmail,
                mockTemplateIdEmail,
                personalisation,
                mockReference,
                mockReference,
                null,
                null,
                null,
                null
            );

            // Assert
            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.template.id);
            Assert.IsNotNull(response.template.uri);
            Assert.IsNotNull(response.template.version);
            Assert.AreEqual(TEST_EMAIL_SUBJECT, response.content.subject);
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

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
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
                content = new EmailResponseContent
                {
                    subject = TEST_EMAIL_SUBJECT,
                    body = TEST_EMAIL_BODY
                },
                reference = mockReference
            };

            mockNotificationClient.Setup(x =>
                x.SendEmail(
                    mockEmail,
                    mockTemplateIdEmail,
                    personalisation,
                    mockReference,
                    mockReplyToId,
                    null,
                    null,
                    null,
                    null
                )
            ).Returns(fakeResponse);

            // Act
            EmailNotificationResponse response = client.SendEmail(
                mockEmail,
                mockTemplateIdEmail,
                personalisation,
                mockReference,
                mockReplyToId,
                null,
                null,
                null,
                null
            );

            // Assert
            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.template.id);
            Assert.IsNotNull(response.template.uri);
            Assert.IsNotNull(response.template.version);
            Assert.AreEqual(TEST_EMAIL_SUBJECT, response.content.subject);
        }



        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllNotifications()
        {
            mockNotificationClient
                .Setup(client => client.GetNotifications(
                    "",    // templateType
                    "",    // status
                    "",    // reference
                    "",    // olderThanId
                    false  // includeSpreadsheetUploads
                ))
                .Returns(
                    new NotificationList
                    {
                        notifications = new List<Notification> {
                            new Notification { id = "mock-id-1" },
                            new Notification { id = "mock-id-2" }
                        }
                    }
                );

            var notificationsResponse = mockNotificationClient.Object.GetNotifications("", "", "", "", false);
            Assert.IsNotNull(notificationsResponse);
            Assert.IsNotNull(notificationsResponse.notifications);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetNotificationWithInvalidIdRaisesClientException()
        {
            mockNotificationClient.Setup(client => client.GetNotificationById(It.IsAny<string>())).Throws(new NotifyClientException("No result found"));

            var ex = Assert.Throws<NotifyClientException>(() =>
                mockNotificationClient.Object.GetNotificationById("invalid-id")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetTemplateWithInvalidIdRaisesClientException()
        {
            mockNotificationClient.Setup(client => client.GetTemplateById(It.IsAny<string>())).Throws(new NotifyClientException("Status code 404"));

            var ex = Assert.Throws<NotifyClientException>(() =>
                mockNotificationClient.Object.GetTemplateById("id is not a valid UUID")
            );
            Assert.That(ex.Message, Does.Contain("Status code 404"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetTemplateWithIdWithoutResultRaisesClientException()
        {
            mockNotificationClient.Setup(client => client.GetTemplateById(It.IsAny<string>())).Throws(new NotifyClientException("No result found"));

            var ex = Assert.Throws<NotifyClientException>(() =>
                mockNotificationClient.Object.GetTemplateById("non-existent-id")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllTemplates()
        {
            mockNotificationClient.Setup(client => client.GetAllTemplates("", "", "", "", false)).Returns(
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

            mockNotificationClient.Setup(client => client.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, dynamic>>(),
                It.IsAny<string>(),
                fakeReplyToId,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Throws(new NotifyClientException($"email_reply_to_id {fakeReplyToId}"));

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> { { "name", "someone" } };

            var ex = Assert.Throws<NotifyClientException>(() =>
                mockNotificationClient.Object.SendEmail(
                    "fake@example.com",
                    "mock-email-template",
                    personalisation,
                    null,
                    fakeReplyToId,
                    null,
                    null,
                    null,
                    null
                )
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
                mockReference,
                mockReplyToId,
                null,
                null,
                null,
                null
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
                mockReference,
                mockReplyToId,
                null,
                null,
                null,
                null
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
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> { { "name", "someone" } };

            mockNotificationClient.Setup(client => client.SendSms(
                mockPhoneNumber,
                mockTemplateId,
                personalisation,
                mockReference,
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
                mockReference,
                mockSmsSenderId
            );

            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, "HELLO WORLD v2");
            Assert.IsNotNull(response.reference);
            Assert.AreEqual(response.reference, "sample-test-ref");
        }
    }
}
