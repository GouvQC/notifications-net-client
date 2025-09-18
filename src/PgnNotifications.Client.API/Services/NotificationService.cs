using PgnNotifications.Models.Responses;

namespace PgnNotifications.Client.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
  
        public NotificationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var context = new NotificationServiceContext(_httpContextAccessor);
            _client = PgnNotifications.Client.Builders.NotificationClientBuilder.Create()
                .WithApiKey(context.ApiKey)
                .WithClientId(context.ClientId)
                .WithBaseUrl(context.BaseUrl)
                .Build();
        }

        public object GenerateTemplatePreview(string templateId, Dictionary<string, dynamic> parameters)
            => _client.GenerateTemplatePreview(templateId, parameters);
        
        public IEnumerable<TemplateList> GetAllTemplates(string templateType)
            => (IEnumerable<TemplateList>)_client.GetAllTemplates(templateType);

        public object GetNotificationById(string notificationId)
            => _client.GetNotificationById(notificationId);

        public IEnumerable<object> GetNotifications(string templateType, string status, string reference, string olderThanId, bool includeSpreadsheetUploads)
            => (IEnumerable<object>)_client.GetNotifications(templateType, status, reference, olderThanId, includeSpreadsheetUploads);
       
        public IEnumerable<object> GetReceivedTexts(string olderThanId)
            => (IEnumerable<object>)_client.GetReceivedTexts(olderThanId);

        public object GetTemplateById(string templateId)
            => _client.GetTemplateById(templateId);

        public object GetTemplateByIdAndVersion(string templateId, int version)
            => _client.GetTemplateByIdAndVersion(templateId, version);
         
        public string CheckHealth()
            => _client.CheckHealth();

        public SmsNotificationResponse SendSms(string mobileNumber, string templateId, Dictionary<string, dynamic> personalisation,
                                        string? reference, string? smsSenderId)
            => _client.SendSms(mobileNumber, templateId, personalisation);

        public EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation,
                                                   string? reference, string? emailReplyToId, string? oneClickUnsubscribeURL,
                                                   string? scheduledFor, string? importance, string? ccAddress)
            => _client.SendEmail(emailAddress, templateId, personalisation, reference, emailReplyToId, oneClickUnsubscribeURL,
                                 scheduledFor, importance, ccAddress);
        public HttpResponseMessage SendBulk(string templateId, string name, List<List<string?>> rows, string? csv, string? reference, string? scheduledFor, string? emailReplyToId)
            => _client.SendBulkNotifications(templateId, name, rows, csv, reference, scheduledFor, emailReplyToId);
    }

}
