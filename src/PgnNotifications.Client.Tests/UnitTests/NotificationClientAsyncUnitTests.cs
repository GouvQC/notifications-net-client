using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PgnNotifications.Client;
using PgnNotifications.Exceptions;
using PgnNotifications.Models;
using PgnNotifications.Models.Responses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PgnNotifications.Client.Tests.UnitTests
{
    [TestFixture]
    public class NotificationClientAsyncUnitTests
    {
        private Mock<HttpMessageHandler> handler;
        private NotificationClient client;

        [SetUp]
        public void SetUp()
        {
            handler = new Mock<HttpMessageHandler>();

            var w = new HttpClientWrapper(new HttpClient(handler.Object));
            client = new NotificationClient(w, Constants.fakeApiKey, Constants.fakeClientId);
        }

        [TearDown]
        public void TearDown()
        {
            handler = null;
            client = null;
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public void CreateNotificationClientWithInvalidApiKeyFails()
        {
            Assert.Throws<NotifyAuthException>(() => new NotificationClient("someinvalidkey", "someinvalidclientid"));
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public void CreateNotificationClientWithProxyAPIAndClientIdFails()
        {
            Assert.Throws<NotifyAuthException>(() => new NotificationClient(Constants.fakeApiKey, ""));
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public void CreateNotificationClientWithEmptyApiKeyFails()
        {
            Assert.Throws<NotifyAuthException>(() => new NotificationClient("" , ""));
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public void GetNonJsonResponseHandlesException()
        {
            MockRequest("non json response",
                client.GET_ALL_NOTIFICATIONS_URL,
                AssertValidRequest, status: HttpStatusCode.NotFound);

            var ex = Assert.ThrowsAsync<NotifyClientException>(async () => await client.GetNotificationsAsync());
            Assert.That(ex.Message, Does.Contain("Status code 404. Error: non json response"));
        }


        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetNotificationByIdCreatesExpectedRequest()
        {
            MockRequest(Constants.fakeNotificationJson,
                client.GET_NOTIFICATION_URL + Constants.fakeNotificationId,
                AssertValidRequest);

            await client.GetNotificationByIdAsync(Constants.fakeNotificationId);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public async Task CheckHealth_ReturnsErrorMessage_WhenExceptionThrown()
        {
            string result = await client.CheckHealthAsync();
            Assert.IsTrue(result.StartsWith("❌ Erreur lors de la vérification de l'état de santé"));
            Assert.IsTrue(result.Contains("Handler "));
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public async Task CheckHealth_ReturnsSuccess_WhenNoException()
        {
            MockRequest("ok", client.GET_CHECK_HEALTH_URL, AssertValidRequest);
            await client.CheckHealthAsync();
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllNotificationsCreatesExpectedResult()
        {
            MockRequest(Constants.fakeNotificationsJson,
                client.GET_ALL_NOTIFICATIONS_URL,
                AssertValidRequest);

            await client.GetNotificationsAsync();
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllNotificationsWithStatusCreatesExpectedResult()
        {
            MockRequest(Constants.fakeNotificationsJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?status=sending",
                AssertValidRequest);

            await client.GetNotificationsAsync(status: "sending");
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllNotificationsWithReferenceCreatesExpectedResult()
        {
            MockRequest(Constants.fakeNotificationsJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?reference=foo",
                AssertValidRequest);

            await client.GetNotificationsAsync(reference: "foo");
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllSmsNotificationsWithStatusAndReferenceWithCreatesExpectedResult()
        {
            MockRequest(Constants.fakeNotificationsJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=sms&status=sending&reference=foo",
                AssertValidRequest);

            await client.GetNotificationsAsync(templateType: "sms", status: "sending", reference: "foo");
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllSmsNotificationsCreatesExpectedResult()
        {
            MockRequest(Constants.fakeSmsNotificationResponseJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=sms",
                AssertValidRequest);

            await client.GetNotificationsAsync(templateType: "sms");
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllEmailNotificationsCreatesExpectedResult()
        {
            MockRequest(Constants.fakeEmailNotificationResponseJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=email",
                AssertValidRequest);

            await client.GetNotificationsAsync(templateType: "email");
        }       

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetTemplateWithIdCreatesExpectedRequest()
        {
            MockRequest(Constants.fakeTemplateResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId,
                AssertValidRequest);

            await client.GetTemplateByIdAndVersionAsync(Constants.fakeTemplateId);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetTemplateWithIdAndVersionCreatesExpectedRequest()
        {
            MockRequest(Constants.fakeTemplateResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + client.VERSION_PARAM + "2",
                AssertValidRequest);

            await client.GetTemplateByIdAndVersionAsync(Constants.fakeTemplateId, 2);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetNotificationByIdReceivesExpectedResponse()
        {
            var expectedResponse = JsonConvert.DeserializeObject<Notification>(Constants.fakeNotificationJson);

            MockRequest(Constants.fakeNotificationJson);

            var responseNotification = await client.GetNotificationByIdAsync(Constants.fakeNotificationId);
            Assert.AreEqual(expectedResponse, responseNotification);
        }       

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetTemplateWithIdReceivesExpectedResponse()
        {
            var expectedResponse = JsonConvert.DeserializeObject<TemplateResponse>(Constants.fakeTemplateResponseJson);

            MockRequest(Constants.fakeTemplateResponseJson);

            var responseTemplate = await client.GetTemplateByIdAsync(Constants.fakeTemplateId);
            Assert.AreEqual(expectedResponse, responseTemplate);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetTemplateWithIdAndVersionReceivesExpectedResponse()
        {
            var expectedResponse =
                JsonConvert.DeserializeObject<TemplateResponse>(Constants.fakeTemplateResponseJson);

            MockRequest(Constants.fakeTemplateResponseJson);

            var responseTemplate = await client.GetTemplateByIdAndVersionAsync(Constants.fakeTemplateId, 2);
            Assert.AreEqual(expectedResponse, responseTemplate);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GenerateTemplatePreviewGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> {
                    { "name", "someone" }
            };

            var o = new JObject
            {
                { "personalisation", JObject.FromObject(personalisation) }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + "/preview", AssertValidRequest, HttpMethod.Post);

            await client.GenerateTemplatePreviewAsync(Constants.fakeTemplateId, personalisation);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GenerateTemplatePreviewReceivesExpectedResponse()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> {
                    { "name", "someone" }
            };

            var expected = new JObject
            {
                { "personalisation", JObject.FromObject(personalisation) }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + "/preview",
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            await client.GenerateTemplatePreviewAsync(Constants.fakeTemplateId, personalisation);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllTemplatesCreatesExpectedRequest()
        {
            MockRequest(Constants.fakeTemplateListResponseJson,
                 client.GET_ALL_TEMPLATES_URL, AssertValidRequest);

            await client.GetAllTemplatesAsync();
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllTemplatesBySmsTypeCreatesExpectedRequest()
        {
            const string type = "sms";
            MockRequest(Constants.fakeTemplateSmsListResponseJson,
                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

            await client.GetAllTemplatesAsync(type);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllTemplatesByEmailTypeCreatesExpectedRequest()
        {
            const string type = "email";

            MockRequest(Constants.fakeTemplateEmailListResponseJson,
                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

            await client.GetAllTemplatesAsync(type);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllTemplatesForEmptyListReceivesExpectedResponse()
        {
            var expectedResponse = JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateEmptyListResponseJson);

            MockRequest(Constants.fakeTemplateEmptyListResponseJson);

            TemplateList templateList = await client.GetAllTemplatesAsync();

            List<TemplateResponse> templates = templateList.templates;

            Assert.AreEqual(templates.Count, 0);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllTemplatesReceivesExpectedResponse()
        {
            TemplateList expectedResponse = JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateListResponseJson);

            MockRequest(Constants.fakeTemplateListResponseJson);

            TemplateList templateList = await client.GetAllTemplatesAsync();

            List<TemplateResponse> templates = templateList.templates;

            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                Assert.AreEqual(expectedResponse.templates[i], templates[i]);
            }
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllTemplatesBySmsTypeReceivesExpectedResponse()
        {
            const string type = "sms";

            TemplateList expectedResponse =
                JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateSmsListResponseJson);

            MockRequest(Constants.fakeTemplateSmsListResponseJson,
                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

            TemplateList templateList = await client.GetAllTemplatesAsync(type);

            List<TemplateResponse> templates = templateList.templates;

            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                Assert.AreEqual(expectedResponse.templates[i], templates[i]);
            }
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllTemplatesByEmailTypeReceivesExpectedResponse()
        {
            const string type = "email";

            TemplateList expectedResponse =
                JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateEmailListResponseJson);

            MockRequest(Constants.fakeTemplateEmailListResponseJson,
                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

            TemplateList templateList = await client.GetAllTemplatesAsync(type);

            List<TemplateResponse> templates = templateList.templates;

            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                Assert.AreEqual(expectedResponse.templates[i], templates[i]);
            }
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllReceivedTextsCreatesExpectedRequest()
        {
            MockRequest(Constants.fakeReceivedTextListResponseJson,
                 client.GET_RECEIVED_TEXTS_URL, AssertValidRequest);

            await client.GetReceivedTextsAsync();
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task GetAllReceivedTextsReceivesExpectedResponse()
        {
            MockRequest(Constants.fakeReceivedTextListResponseJson,
                 client.GET_RECEIVED_TEXTS_URL, AssertValidRequest);

            ReceivedTextListResponse expectedResponse =
                JsonConvert.DeserializeObject<ReceivedTextListResponse>(Constants.fakeReceivedTextListResponseJson);

            MockRequest(Constants.fakeReceivedTextListResponseJson,
                         client.GET_RECEIVED_TEXTS_URL, AssertValidRequest);

            ReceivedTextListResponse receivedTextList = await client.GetReceivedTextsAsync();

            List<ReceivedTextResponse> receivedTexts = receivedTextList.receivedTexts;

            Assert.AreEqual(receivedTexts.Count, expectedResponse.receivedTexts.Count);
            for (int i = 0; i < receivedTexts.Count; i++)
            {
                Assert.AreEqual(expectedResponse.receivedTexts[i], receivedTexts[i]);
            }
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendSmsNotificationGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            JObject expected = new JObject
            {
                { "phone_number", Constants.fakePhoneNumber },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) }
            };

            MockRequest(Constants.fakeSmsNotificationResponseJson,
                client.SEND_SMS_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            await client.SendSmsAsync(Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendSmsNotificationWithSmsSenderIdGeneratesExpectedRequest()
        {
            var personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            var expected = new JObject
            {
                { "phone_number", Constants.fakePhoneNumber },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "sms_sender_id", Constants.fakeSMSSenderId }
            };

            MockRequest(Constants.fakeSmsNotificationWithSMSSenderIdResponseJson,
                client.SEND_SMS_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            await client.SendSmsAsync(
                Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation: personalisation, smsSenderId: Constants.fakeSMSSenderId);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendSmsNotificationGeneratesExpectedResponse()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            SmsNotificationResponse expectedResponse = JsonConvert.DeserializeObject<SmsNotificationResponse>(Constants.fakeSmsNotificationResponseJson);

            MockRequest(Constants.fakeSmsNotificationResponseJson);

            SmsNotificationResponse actualResponse = await client.SendSmsAsync(Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation);

            Assert.AreEqual(expectedResponse, actualResponse);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendEmailNotificationGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            JObject expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "reference", Constants.fakeNotificationReference }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            await client.SendEmailAsync(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendEmailNotificationGeneratesExpectedResponse()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            EmailNotificationResponse expectedResponse = JsonConvert.DeserializeObject<EmailNotificationResponse>(Constants.fakeEmailNotificationResponseJson);

            MockRequest(Constants.fakeEmailNotificationResponseJson);

            EmailNotificationResponse actualResponse = await client.SendEmailAsync(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);

            Assert.AreEqual(expectedResponse, actualResponse);

        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendEmailNotificationWithReplyToIdGeneratesExpectedRequest()
        {
            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            var expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "reference", Constants.fakeNotificationReference },
                { "email_reply_to_id", Constants.fakeReplyToId}
            };

            MockRequest(Constants.fakeTemplateEmailListResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent,
                expected.ToString(Formatting.None));

            await client.SendEmailAsync(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference, Constants.fakeReplyToId);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendEmailNotificationWithReplyToIdGeneratesExpectedResponse()
        {
            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            var expectedResponse = JsonConvert.DeserializeObject<EmailNotificationResponse>(Constants.fakeEmailNotificationResponseJson);

            MockRequest(Constants.fakeEmailNotificationResponseJson);

            var actualResponse = await client.SendEmailAsync(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference, Constants.fakeReplyToId);

            Assert.AreEqual(expectedResponse, actualResponse);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendEmailNotificationWithoneClickUnsubscribeURLGeneratesExpectedRequest()
        {
            var personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            var expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "one_click_unsubscribe_url", Constants.fakeoneClickUnsubscribeURL },
            };

            MockRequest(
                Constants.fakeEmailNotificationResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent,
                expected.ToString(Formatting.None)
            );

            await client.SendEmailAsync(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, oneClickUnsubscribeURL: Constants.fakeoneClickUnsubscribeURL);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendEmailNotificationWithDocumentGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "document", NotificationClient.PrepareUpload(Encoding.UTF8.GetBytes("%PDF-1.5 testpdf")) }
                };
            JObject expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", new JObject
                  {
                    {"document", new JObject
                      {
                        {"file", "JVBERi0xLjUgdGVzdHBkZg=="},
                        {"filename", null},
                        {"confirm_email_before_download", null},
                        {"retention_period", null}
                      }
                    }
                  }
                },
                { "reference", Constants.fakeNotificationReference }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            await client.SendEmailAsync(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendEmailNotificationWithCSVDocumentGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "document", NotificationClient.PrepareUpload(Encoding.UTF8.GetBytes("%PDF-1.5 testpdf"), "report.csv") }
                };
            JObject expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", new JObject
                  {
                    {"document", new JObject
                      {
                        {"file", "JVBERi0xLjUgdGVzdHBkZg=="},
                        {"filename", "report.csv"},
                        {"confirm_email_before_download", null},
                        {"retention_period", null}
                      }
                    }
                  }
                },
                { "reference", Constants.fakeNotificationReference }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            await client.SendEmailAsync(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public async Task SendEmailNotificationCanSetConfirmEmailBeforeDownloadAndRetentionPeriod()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "document", NotificationClient.PrepareUpload(Encoding.UTF8.GetBytes("%PDF-1.5 testpdf"), "report.csv", false, "1 weeks") }
                };
            JObject expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", new JObject
                  {
                    {"document", new JObject
                      {
                        {"file", "JVBERi0xLjUgdGVzdHBkZg=="},
                        {"filename", "report.csv"},
                        {"confirm_email_before_download", false},
                        {"retention_period", "1 weeks"}
                      }
                    }
                  }
                },
                { "reference", Constants.fakeNotificationReference }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            await client.SendEmailAsync(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public void PrepareUploadWithLargeDocumentGeneratesAnError()
        {
            Assert.That(
                    () => { NotificationClient.PrepareUpload(new byte[3 * 1024 * 1024]); },
                    Throws.ArgumentException
                    );
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendBulkNotificationWithRowsGeneratesExpectedRequest()
        {
            JObject expected = new JObject()
            {
                { "template_id", Constants.fakeTemplateId },
                { "name",        Constants.fakeSmsBulkName },
                { "rows",        JArray.FromObject(Constants.fakeRowsEmailBulk) },
                { "reference",   Constants.fakeNotificationReference },
                { "replyToId",   Constants.fakeReplyToId }
            };

            MockRequest(
                Constants.fakeEmailBulkResponseJson,
                client.SEND_BULK_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent,
                expected.ToString(Formatting.None)
            );

            _ = client.SendBulkNotificationsAsync(
                templateId: Constants.fakeTemplateId,
                name: Constants.fakeSmsBulkName,
                rows: Constants.fakeRowsEmailBulk,
                reference: Constants.fakeNotificationReference,
                replyToId: Constants.fakeReplyToId
            );
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendBulkNotificationsWithPhoneNumberCsvGeneratesExpectedRequest()
        {
            JObject expected = new JObject()
            {
                { "template_id", Constants.fakeTemplateId },
                { "name",        Constants.fakeSmsBulkName },
                { "csv",         Constants.fakeCsvBulkJson },
                { "reference",   Constants.fakeNotificationReference },
                { "replyToId",   Constants.fakeReplyToId }
            };

            MockRequest(
                Constants.fakeSmsBulkResponseJson,
                client.SEND_BULK_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent,
                expected.ToString(Formatting.None)
            );

            _ = client.SendBulkNotificationsAsync(
                templateId: Constants.fakeTemplateId,
                name: Constants.fakeSmsBulkName,
                csv: Constants.fakeCsvBulkJson,
                reference: Constants.fakeNotificationReference,
                replyToId: Constants.fakeReplyToId
            );
        }

        private static void AssertGetExpectedContent(string expected, string content)
        {
            Assert.IsNotNull(content);
            Assert.AreEqual(expected, content);
        }

        private void AssertValidRequest(string uri, HttpRequestMessage r, HttpMethod method = null)
        {
            if (method == null)
            {
                method = HttpMethod.Get;
            }

            Assert.AreEqual(r.Method, method);
            Assert.AreEqual(r.RequestUri.ToString(), client.BaseUrl + uri);
            Assert.IsNotNull(r.Headers.Authorization);
            Assert.IsNotNull(r.Headers.UserAgent);
            Assert.AreEqual(r.Headers.UserAgent.ToString(), client.GetUserAgent());
            Assert.AreEqual(r.Headers.Accept.ToString(), "application/json");
        }

        private void MockRequest(string content, string uri,
                          Action<string, HttpRequestMessage, HttpMethod> _assertValidRequest = null,
                          HttpMethod method = null,
                          Action<string, string> _assertGetExpectedContent = null,
                          string expected = null,
                          HttpStatusCode status = HttpStatusCode.OK)
        {
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage
                {
                    StatusCode = status,
                    Content = new StringContent(content)
                }))
                .Callback<HttpRequestMessage, CancellationToken>((r, c) =>
                {
                    _assertValidRequest(uri, r, method);

                    if (r.Content == null || _assertGetExpectedContent == null) return;

                    var response = r.Content.ReadAsStringAsync().Result;
                    _assertGetExpectedContent(expected, response);
                });
        }

        private void MockRequest(string content)
        {

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content)
                }));
        }
    }
}
