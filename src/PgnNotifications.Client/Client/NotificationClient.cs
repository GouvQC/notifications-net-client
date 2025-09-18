using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PgnNotifications.Exceptions;
using PgnNotifications.Interfaces;
using PgnNotifications.Models;
using PgnNotifications.Models.Responses;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PgnNotifications.Client
{
    public class NotificationClient : BaseClient, INotificationClient, IAsyncNotificationClient
    {
        public string GET_RECEIVED_TEXTS_URL = "v2/received-text-messages";
        public string GET_NOTIFICATION_URL = "v2/notifications/";
        public string GET_CHECK_HEALTH_URL = "health";
        public string SEND_SMS_NOTIFICATION_URL = "v2/notifications/sms";
        public string SEND_EMAIL_NOTIFICATION_URL = "v2/notifications/email";
        public string SEND_BULK_NOTIFICATION_URL = "v2/notifications/bulk";
        public string GET_TEMPLATE_URL = "v2/template/";
        public string GET_ALL_NOTIFICATIONS_URL = "v2/notifications";
        public string GET_ALL_TEMPLATES_URL = "v2/templates";
        public string TYPE_PARAM = "?type=";
        public string VERSION_PARAM = "/version/";

        internal NotificationClient(string apiKey, string clientId) : base(new HttpClientWrapper(new HttpClient()), apiKey, clientId)
        {
        }

        internal NotificationClient(string baseUrl, string apiKey, string clientId) : base(new HttpClientWrapper(new HttpClient()), apiKey, clientId,
            baseUrl)
        {
        }

        internal NotificationClient(IHttpClient client, string apiKey, string clientId, string baseUrl = null) : base(client, apiKey, clientId, baseUrl)
        {
        }

        public async Task<Notification> GetNotificationByIdAsync(string notificationId)
        {
            var url = GET_NOTIFICATION_URL + notificationId;

            var response = await this.GET(url).ConfigureAwait(false);

            try
            {
                var notification = JsonConvert.DeserializeObject<Notification>(response);
                return notification;
            }
            catch (JsonReaderException)
            {
                throw new NotifyClientException("Could not create Notification object from response: {0}", response);
            }
        }

        public static string ToQueryString(NameValueCollection nvc)
        {
            if (nvc.Count == 0) return "";

            IEnumerable<string> segments = from key in nvc.AllKeys
                                           from value in nvc.GetValues(key)
                                           select string.Format("{0}={1}",
                                           WebUtility.UrlEncode(key),
                                           WebUtility.UrlEncode(value));
            return "?" + string.Join("&", segments);
        }

        public async Task<NotificationList> GetNotificationsAsync(string templateType = "", string status = "", string reference = "",
            string olderThanId = "", bool includeSpreadsheetUploads = false)
        {
            var query = new NameValueCollection();
            if (!string.IsNullOrWhiteSpace(templateType))
            {
                query.Add("template_type", templateType);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query.Add("status", status);
            }

            if (!string.IsNullOrWhiteSpace(reference))
            {
                query.Add("reference", reference);
            }

            if (!string.IsNullOrWhiteSpace(olderThanId))
            {
                query.Add("older_than", olderThanId);
            }

            if (includeSpreadsheetUploads)
            {
                query.Add("include_jobs", "True");
            }

            var finalUrl = GET_ALL_NOTIFICATIONS_URL + ToQueryString(query);
            var response = await GET(finalUrl).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<NotificationList>(response);

        }

        public async Task<TemplateList> GetAllTemplatesAsync(string templateType = "")
        {
            var finalUrl = string.Format(
                "{0}{1}",
                GET_ALL_TEMPLATES_URL,
                templateType == string.Empty ? string.Empty : TYPE_PARAM + templateType
            );

            var response = await GET(finalUrl).ConfigureAwait(false);

            var templateList = JsonConvert.DeserializeObject<TemplateList>(response);

            return templateList;
        }

        public async Task<ReceivedTextListResponse> GetReceivedTextsAsync(string olderThanId = "")
        {
            var finalUrl = string.Format(
                "{0}{1}",
                GET_RECEIVED_TEXTS_URL,
                string.IsNullOrWhiteSpace(olderThanId) ? "" : "?older_than=" + olderThanId
            );

            var response = await this.GET(finalUrl).ConfigureAwait(false);

            var receivedTexts = JsonConvert.DeserializeObject<ReceivedTextListResponse>(response);

            return receivedTexts;
        }

        public async Task<SmsNotificationResponse> SendSmsAsync(
            string mobileNumber,
            string templateId,
            Dictionary<string, dynamic> personalisation = null,
            string reference = null,
            string smsSenderId = null)
        {
            // Create the object with required and optional parameters
            var o = CreateRequestParams(templateId, personalisation, reference);

            // Add phone number to the object
            o.AddFirst(new JProperty("phone_number", mobileNumber));

            // Add SMS sender ID if provided
            if (!string.IsNullOrWhiteSpace(smsSenderId))
                o.Add(new JProperty("sms_sender_id", smsSenderId));

            // Send the request and deserialize the response
            var response = await POST(SEND_SMS_NOTIFICATION_URL, o.ToString(Formatting.None))
                .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<SmsNotificationResponse>(response);
        }

        public async Task<EmailNotificationResponse> SendEmailAsync(
            string emailAddress,
            string templateId,
            Dictionary<string, dynamic> personalisation = null,
            string reference = null,
            string emailReplyToId = null,
            string oneClickUnsubscribeURL = null,
            string scheduledFor = null,
            string importance = null,
            string ccAddress = null)
        {
            // Create the object with required and optional parameters
            var o = CreateRequestParams(templateId, personalisation, reference);

            // Add email address at the beginning of the JSON object
            o.AddFirst(new JProperty("email_address", emailAddress));

            // Add optional fields if provided
            if (!string.IsNullOrWhiteSpace(emailReplyToId))
                o.Add(new JProperty("email_reply_to_id", emailReplyToId));

            if (!string.IsNullOrWhiteSpace(oneClickUnsubscribeURL))
                o.Add(new JProperty("one_click_unsubscribe_url", oneClickUnsubscribeURL));

            if (!string.IsNullOrWhiteSpace(scheduledFor))
                o.Add(new JProperty("scheduled_for", scheduledFor));

            if (!string.IsNullOrWhiteSpace(importance))
            {
                var validImportances = new[] { "high", "normal", "low" };
                var importanceNormalized = importance.Trim().ToLowerInvariant();

                // Validate importance value
                if (!validImportances.Contains(importanceNormalized))
                    throw new NotifyClientException("The 'importance' field must be: high, normal, or low.");

                o.Add(new JProperty("importance", importanceNormalized));
            }

            if (!string.IsNullOrWhiteSpace(ccAddress))
                o.Add(new JProperty("cc_address", ccAddress));

            // Send the request and deserialize the response
            var response = await POST(SEND_EMAIL_NOTIFICATION_URL, o.ToString(Formatting.None))
                .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<EmailNotificationResponse>(response);
        }

        public async Task<HttpResponseMessage> SendBulkNotificationsAsync(
                string templateId,
                string name,                
                List<List<string>> rows = null,
                string csv = null,                
                string reference = null,
                string scheduledFor = null,
                string replyToId = null)
        {
            if (rows == null && string.IsNullOrWhiteSpace(csv))
            {
                throw new ArgumentException("Vous devez fournir soit 'rows', soit 'csv'.");
            }

            if (rows != null && !string.IsNullOrWhiteSpace(csv))
            {
                throw new ArgumentException("Vous ne pouvez pas fournir à la fois 'rows' et 'csv'.");
            }

            // Create the object with required and optional parameters
            var o = new JObject
            {
                {"template_id", templateId},
                { "name", name }
            };

            if (rows != null)
                 o.Add(new JProperty("rows", JArray.FromObject(rows)));
               
      
            if (!string.IsNullOrWhiteSpace(csv))
                 o.Add(new JProperty("csv", csv)); 
           
            if (!string.IsNullOrWhiteSpace(reference))
                o.Add(new JProperty("reference", reference));

            if (!string.IsNullOrWhiteSpace(scheduledFor))
                 o.Add(new JProperty("scheduledFor", scheduledFor));

            if (!string.IsNullOrWhiteSpace(replyToId))
                 o.Add(new JProperty("replyToId", replyToId));

            var response = await POST(SEND_BULK_NOTIFICATION_URL, o.ToString(Formatting.None))
               .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<HttpResponseMessage>(response);
        }       
        
        public async Task<string> CheckHealthAsync()
        {
            try
            {
                return await GET(GET_CHECK_HEALTH_URL);
            }
            catch (Exception ex)
            {
                return $"❌ Erreur lors de la vérification de l'état de santé : {ex.Message}";
            }
        }       


        public async Task<TemplateResponse> GetTemplateByIdAsync(string templateId)
        {
            var url = GET_TEMPLATE_URL + templateId;

            return await GetTemplateFromURLAsync(url).ConfigureAwait(false);
        }

        public async Task<TemplateResponse> GetTemplateByIdAndVersionAsync(string templateId, int version = 0)
        {
            var pattern = "{0}{1}" + (version > 0 ? VERSION_PARAM + "{2}" : "");
            var url = string.Format(pattern, GET_TEMPLATE_URL, templateId, version);

            return await GetTemplateFromURLAsync(url).ConfigureAwait(false);
        }

        public async Task<TemplatePreviewResponse> GenerateTemplatePreviewAsync(string templateId,
            Dictionary<string, dynamic> personalisation = null)
        {
            var url = string.Format("{0}{1}/preview", GET_TEMPLATE_URL, templateId);

            var o = new JObject
            {
                {"personalisation", JObject.FromObject(personalisation)}
            };

            var response = await this.POST(url, o.ToString(Formatting.None)).ConfigureAwait(false);

            try
            {
                var template = JsonConvert.DeserializeObject<TemplatePreviewResponse>(response);
                return template;
            }
            catch (JsonReaderException)
            {
                throw new NotifyClientException("Could not create Template object from response: {0}", response);
            }
        }

        public static JObject PrepareUpload(byte[] documentContents, string filename, bool confirmEmailBeforeDownload, string retentionPeriod)
        {
            if (documentContents.Length > 2 * 1024 * 1024)
            {
                throw new System.ArgumentException("File is larger than 2MB");
            }
            return new JObject
            {
                {"file", System.Convert.ToBase64String(documentContents)},
                {"filename", filename},
                {"confirm_email_before_download", confirmEmailBeforeDownload},
                {"retention_period", retentionPeriod}
            };
        }

        public static JObject PrepareUpload(byte[] documentContents, string filename)
        {
            if (documentContents.Length > 2 * 1024 * 1024)
            {
                throw new System.ArgumentException("File is larger than 2MB");
            }
            return new JObject
            {
                {"file", System.Convert.ToBase64String(documentContents)},
                {"filename", filename},
                {"confirm_email_before_download", null},
                {"retention_period", null}
            };
        }

        public static JObject PrepareUpload(byte[] documentContents)
        {
            if (documentContents.Length > 2 * 1024 * 1024)
            {
                throw new System.ArgumentException("File is larger than 2MB");
            }
            return new JObject
            {
                {"file", System.Convert.ToBase64String(documentContents)},
                {"filename", null},
                {"confirm_email_before_download", null},
                {"retention_period", null}
            };
        }

        private async Task<TemplateResponse> GetTemplateFromURLAsync(string url)
        {
            var response = await this.GET(url).ConfigureAwait(false);

            try
            {
                var template = JsonConvert.DeserializeObject<TemplateResponse>(response);
                return template;
            }
            catch (JsonReaderException)
            {
                throw new NotifyClientException("Could not create Template object from response: {0}", response);
            }
        }

        private static JObject CreateRequestParams(string templateId, Dictionary<string, dynamic> personalisation = null,
            string reference = null)
        {
            var personalisationJson = new JObject();

            if (personalisation != null)
            {
                personalisationJson = JObject.FromObject(personalisation);
            }

            var o = new JObject
            {
                {"template_id", templateId},
                {"personalisation", personalisationJson}
            };

            if (reference != null)
            {
                o.Add("reference", reference);
            }

            return o;
        }

        private static Exception HandleAggregateException(AggregateException ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            if (ex.InnerExceptions != null && ex.InnerExceptions.Count == 1)
            {
                return ex.InnerException;
            }
            else
            {
                return ex;
            }
        }

        public TemplatePreviewResponse GenerateTemplatePreview(string templateId, Dictionary<string, dynamic> personalisation = null)
        {
            try
            {
                return GenerateTemplatePreviewAsync(templateId, personalisation).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public TemplateList GetAllTemplates(string templateType = "")
        {
            try
            {
                return GetAllTemplatesAsync(templateType).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public Notification GetNotificationById(string notificationId)
        {
            try
            {
                return GetNotificationByIdAsync(notificationId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public NotificationList GetNotifications(string templateType = "", string status = "", string reference = "", string olderThanId = "", bool includeSpreadsheetUploads = false)
        {
            try
            {
                return GetNotificationsAsync(templateType, status, reference, olderThanId, includeSpreadsheetUploads).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public ReceivedTextListResponse GetReceivedTexts(string olderThanId = "")
        {
            try
            {
                return GetReceivedTextsAsync(olderThanId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public TemplateResponse GetTemplateById(string templateId)
        {
            try
            {
                return GetTemplateByIdAsync(templateId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public TemplateResponse GetTemplateByIdAndVersion(string templateId, int version = 0)
        {
            try
            {
                return GetTemplateByIdAndVersionAsync(templateId, version).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public string CheckHealth()
        {
            try
            {
                return CheckHealthAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return $"❌ Erreur lors de la synchronisation: {ex.Message}";
            }
        }

        public SmsNotificationResponse SendSms(string mobileNumber, string templateId, Dictionary<string, dynamic> personalisation = null, string reference = null, string smsSenderId = null)
        {
            try
            {
                return SendSmsAsync(mobileNumber, templateId, personalisation, reference, smsSenderId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null,
                                                   string reference = null, string emailReplyToId = null, string oneClickUnsubscribeURL = null,
                                                   string scheduledFor = null, string importance = null, string ccAddress = null)
        {
            try
            {
                return SendEmailAsync(emailAddress, templateId, personalisation, reference, emailReplyToId, oneClickUnsubscribeURL, scheduledFor, importance, ccAddress).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public HttpResponseMessage SendBulkNotifications(string templateId, string name, List<List<string>> rows = null, string csv = null,
                                                         string reference = null, string scheduledFor = null, string replyToId = null )
        {
            try
            {
                return SendBulkNotificationsAsync(templateId, name, rows, csv, reference, scheduledFor, replyToId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }
    }
}
