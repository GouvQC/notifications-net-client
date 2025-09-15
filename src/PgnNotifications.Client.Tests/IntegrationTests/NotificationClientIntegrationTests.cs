using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PgnNotifications.Client;
using PgnNotifications.Client.Tests.Utils;
using PgnNotifications.Exceptions;
using PgnNotifications.Interfaces;
using PgnNotifications.Models;
using PgnNotifications.Models.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Net;
using Moq;
using Newtonsoft.Json;
using System.Text;
using Moq.Protected;
using System.Threading.Tasks;



namespace PgnNotifications.Client.Tests.IntegrationTests
{
    [TestFixture]
    public class NotificationClientIntegrationTests
    {
        private Mock<HttpMessageHandler> handler;
        private NotificationClient client;

        [SetUp]
        public void SetUp()
        {
            handler = new Mock<HttpMessageHandler>();
            var httpClientWrapper = new HttpClientWrapper(new HttpClient(handler.Object));
            client = new NotificationClient(httpClientWrapper, Constants.fakeApiKey, Constants.fakeClientId);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendSmsTestWithPersonalisation()
        {
            var responseContent = Constants.fakeSmsNotificationResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            var personalisation = new Dictionary<string, dynamic> { { "name", "someone" } };

            var response = client.SendSms(
                Constants.fakePhoneNumber,
                Constants.fakeTemplateId,
                personalisation,
                Constants.fakeNotificationReference,
                null
            );

            Assert.IsNotNull(response);
            Assert.AreEqual(Constants.fakeSmsBody, response.content.body);
            Assert.IsNotNull(response.reference);
            Assert.AreEqual("sample-test-ref", response.reference);
        }


        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestWithPersonalisation()
        {
            var responseContent = Constants.fakeEmailNotificationResponseJson;

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            var personalisation = new Dictionary<string, dynamic> { { "name", "someone" } };

            var response = client.SendEmail(
                Constants.fakeEmail,
                Constants.fakeTemplateId,
                personalisation,
                null,
                null,
                null,
                null,
                null,
                null
            );

            Assert.IsNotNull(response);
            Assert.AreEqual("Subject", response.content.subject);
        }


        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailWithDocumentPersonalisationTest()
        {
            var responseContent = Constants.fakeEmailNotificationResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            var personalisation = new Dictionary<string, dynamic> { { "name", "someone" } };

            var response = client.SendEmail(
                Constants.fakeEmail,
                Constants.fakeTemplateId,
                personalisation,
                Constants.fakeNotificationReference,
                Constants.fakeReplyToId,
                null,
                null,
                null,
                null
            );

            Assert.IsNotNull(response);
            Assert.AreEqual(Constants.fakeEmailBody, response.content.body); // = TEST_EMAIL_BODY
            Assert.AreEqual("Subject", response.content.subject);               // = TEST_EMAIL_SUBJECT
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

            var responseContent = Constants.fakeEmailNotificationResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" },
                { "document", Convert.ToBase64String(pdfContents) }
            };

            var response = client.SendEmail(
                Constants.fakeEmail,
                Constants.fakeTemplateId,
                personalisation,
                Constants.fakeNotificationReference,
                Constants.fakeReplyToId,
                null,
                null,
                null,
                null
            );

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.template.id);
            Assert.IsNotNull(response.template.uri);
            Assert.IsNotNull(response.template.version);
            Assert.AreEqual("Subject", response.content.subject);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendBulkNotificationsWithRows_WorksAsExpected()
        {
            var responseContent = Constants.fakeSmsBulkResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            string functionalTestEmail = Constants.fakeEmail;
            string templateId = Constants.fakeTemplateId;
            string name = "Test Bulk Notification Integration";
            string reference = "bulk_ref_integration_test";

            var rows = new List<List<string>>
            {
                new List<string> { "email_address", "name" },
                new List<string> { functionalTestEmail, "Name Test" },
                new List<string> { functionalTestEmail, "Name Test" }
            };

            var response = client.SendBulkNotifications(
                templateId: templateId,
                name: name,
                rows: rows,
                reference: reference
            );

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendBulkNotificationsWithCsv_WorksAsExpected()
        {
            var responseContent = @"{
            ""id"": ""bulk-csv-id"",
            ""template_id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
            ""name"": ""Bulk send email with personalisation"",
            ""csv"": ""phone_number,name\n07766565767,Name Test\n07766565767,Name Test"",
            ""reference"": ""bulk_ref_integration_test_csv"",
            ""replyToId"": ""78ded4ad-e915-4a89-a314-2940ed141d40""
            }";

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            string functionalPhoneNumber = Constants.fakePhoneNumber;
            string templateId = Constants.fakeTemplateId;

            string name = "Bulk send email with personalisation";
            string reference = "bulk_ref_integration_test_csv";

            string csvData = $"phone_number,name\n{functionalPhoneNumber},Name Test\n{functionalPhoneNumber},Name Test";

            var response = client.SendBulkNotifications(
                templateId: templateId,
                name: name,
                csv: csvData,
                reference: reference
            );

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }


        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllNotifications()
        {
            var responseContent = Constants.fakeNotificationsJson;

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(Constants.fakeNotificationsJson, Encoding.UTF8, "application/json")
                });


            var notificationsResponse = client.GetNotifications();
            Console.WriteLine($"RESPONSE FROM GET: {notificationsResponse}");
            Console.WriteLine("RESPONSE FROM GET: " + JsonConvert.SerializeObject(notificationsResponse));

            Assert.IsNotNull(notificationsResponse, "NotificationList is null");
            Assert.IsNotNull(notificationsResponse.notifications, "notifications list is null");
            Assert.IsNotEmpty(notificationsResponse.notifications, "notifications list is empty");

            foreach (Notification notification in notificationsResponse.notifications)
            {
                NotifyAssertions.AssertNotification(notification);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetNotificationWithInvalidIdRaisesClientException()
        {
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("{\"errors\": [\"No result found\"]}")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var ex = Assert.Throws<NotifyClientException>(() =>
                client.GetNotificationById("fa5f0a6e-5293-49f1-b99f-3fade784382f")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetTemplateWithInvalidIdRaisesClientException()
        {
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("{\"errors\": [\"Status code 404\"]}")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var ex = Assert.Throws<NotifyClientException>(() =>
                client.GetTemplateById("id is not a valid UUID")
            );
            Assert.That(ex.Message, Does.Contain("Status code 404"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetTemplateWithIdWithoutResultRaisesClientException()
        {
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("{\"errors\": [\"No result found\"]}")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var ex = Assert.Throws<NotifyClientException>(() =>
                client.GetTemplateById("fa5f0a6e-5293-49f1-b99f-3fade784382f")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllTemplates()
        {
            var responseContent = Constants.fakeTemplateListResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            TemplateList templateList = client.GetAllTemplates();
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
            var responseContent = Constants.fakeTemplateSmsListResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            const string type = "sms";
            TemplateList templateList = client.GetAllTemplates(type);
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
            var responseContent = Constants.fakeTemplateSmsResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            TemplateResponse template = client.GetTemplateById(Constants.fakeTemplateId);
            Assert.AreEqual(template.id, Constants.fakeTemplateId);
            Assert.AreEqual(template.body, Constants.fakeSmsBody);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetEmailTemplateWithId()
        {
            var responseContent = Constants.fakeTemplateResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            TemplateResponse template = client.GetTemplateById(Constants.fakeTemplateId);
            Assert.AreEqual(template.id, Constants.fakeTemplateId);
            Assert.AreEqual(template.body, Constants.fakeEmailBody);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GenerateSMSPreviewWithPersonalisation()
        {
            var responseContent = Constants.fakeSmsPreviewResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            Dictionary<string, dynamic> personalisation = new() { { "name", "someone" } };

            TemplatePreviewResponse response = client.GenerateTemplatePreview(Constants.fakeTemplateId, personalisation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.body, Constants.fakeSmsBody);
            Assert.AreEqual(response.subject, null);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GenerateEmailPreviewWithPersonalisation()
        {
            var responseContent = Constants.fakeEmailPreviewResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            Dictionary<string, dynamic> personalisation = new() { { "name", "someone" } };

            TemplatePreviewResponse response = client.GenerateTemplatePreview(Constants.fakeTemplateId, personalisation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.body, Constants.fakeEmailBody);
            Assert.AreEqual(response.subject, Constants.fakeEmailSubject);
        }
        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestEmailReplyToNotPresent()
        {
            string fakeReplyToId = Guid.NewGuid().ToString();

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"errors\": [\"email_reply_to_id " + fakeReplyToId + " does not exist\"]}")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            var ex = Assert.Throws<NotifyClientException>(() =>
                client.SendEmail(
                    Constants.fakeEmail,
                    Constants.fakeTemplateId,
                    personalisation,
                    emailReplyToId: fakeReplyToId
                )
            );

            Assert.That(ex.Message, Does.Contain("email_reply_to_id " + fakeReplyToId));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestAllArguments()
        {
            var responseContent = Constants.fakeEmailNotificationResponseJson;

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            var response = client.SendEmail(
                Constants.fakeEmail,
                Constants.fakeTemplateId,
                personalisation,
                Constants.fakeReference,
                emailReplyToId: Constants.fakeReplyToId,
                oneClickUnsubscribeURL: null
            );

            Assert.IsNotNull(response);
            Assert.AreEqual(Constants.fakeEmailBody, response.content.body);
            Assert.AreEqual(Constants.fakeEmailSubject, response.content.subject);
            Assert.AreEqual(Constants.fakeReference, response.reference);
            Assert.IsNull(response.content.oneClickUnsubscribeURL);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendSmsTestWithPersonalisationAndSmsSenderId()
        {
            var responseContent = Constants.fakeSmsNotificationResponseJson;

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            var response = client.SendSms(
                Constants.fakePhoneNumber,
                Constants.fakeTemplateId,
                personalisation,
                Constants.fakeReference,
                Constants.fakeSMSSenderId
            );

            Assert.IsNotNull(response);
            Assert.AreEqual(Constants.fakeSmsBody, response.content.body);
            Assert.IsNotNull(response.reference);
            Assert.AreEqual(Constants.fakeReference, response.reference);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void ConstructorWithHttpClient_WorksWithSendSms()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            var responseContent = Constants.fakeSmsNotificationResponseJson;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // On instancie le client via le nouveau constructeur
            var client = new NotificationClient(
                baseUrl: "https://fake-api.test",
                apiKey: Constants.fakeApiKey,
                httpClient: new HttpClient(handler.Object),
                clientId: Constants.fakeClientId
            );

            // Act
            var response = client.SendSms(
                Constants.fakePhoneNumber,
                Constants.fakeTemplateId,
                new Dictionary<string, dynamic> { { "name", "someone" } },
                Constants.fakeNotificationReference
            );

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(Constants.fakeSmsBody, response.content.body);
            Assert.AreEqual("sample-test-ref", response.reference);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendSms_RespectsCustomHttpClientTimeout()
        {
            // Handler factice qui attend plus longtemps que le timeout du HttpClient
            var httpClient = new HttpClient(new TimeoutHandler(5000)) { Timeout = TimeSpan.FromMilliseconds(100) };

            var client = new NotificationClient(
                baseUrl: "https://fake-api.test",
                apiKey: Constants.fakeApiKey,
                httpClient: httpClient,
                clientId: Constants.fakeClientId
            );

            // On s'assure que l'exception levée est bien un TaskCanceledException
            var ex = Assert.Throws<AggregateException>(() =>
                client.SendSms("1234567890", "templateId", new Dictionary<string, dynamic>(), "ref")
            );

            Assert.That(ex.InnerExceptions[0], Is.TypeOf<TaskCanceledException>());
        }

    }

}