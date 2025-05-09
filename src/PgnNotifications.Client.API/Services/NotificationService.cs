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
            _client = new NotificationClient(context.BaseUrl, context.ApiKey, context.ClientId);
        }

        public SmsNotificationResponse SendSms(string mobileNumber, string templateId, Dictionary<string, dynamic> parameters)     
            => _client.SendSms(mobileNumber, templateId, parameters);

        public EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> parameters)
            => _client.SendEmail(emailAddress, templateId, parameters);

        public IEnumerable<TemplateList> GetAllTemplates(string templateType)
            => (IEnumerable<TemplateList>)_client.GetAllTemplates(templateType);

        public object GetTemplateById(string templateId)
            => _client.GetTemplateById(templateId);

        public object GetTemplateByIdAndVersion(string templateId, int version)
            => _client.GetTemplateByIdAndVersion(templateId, version);

        public object GenerateTemplatePreview(string templateId, Dictionary<string, dynamic> parameters)
            => _client.GenerateTemplatePreview(templateId, parameters);

        public object GetNotificationById(string notificationId)
            => _client.GetNotificationById(notificationId);

        public IEnumerable<object> GetNotifications(string templateType, string status, string reference, string olderThanId, bool includeSpreadsheetUploads)
            => (IEnumerable<object>)_client.GetNotifications(templateType, status, reference, olderThanId, includeSpreadsheetUploads);

        public IEnumerable<object> GetReceivedTexts(string olderThanId)
            => (IEnumerable<object>)_client.GetReceivedTexts(olderThanId);
    }

}
