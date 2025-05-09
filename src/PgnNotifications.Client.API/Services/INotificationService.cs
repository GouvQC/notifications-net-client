using PgnNotifications.Models.Responses;

namespace PgnNotifications.Client.API.Services
{   
    public interface INotificationService
    {
        SmsNotificationResponse SendSms(string mobileNumber, string templateId, Dictionary<string, dynamic> parameters);
        EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> parameters);
        IEnumerable<TemplateList> GetAllTemplates(string templateType);
        object GetTemplateById(string templateId);
        object GetTemplateByIdAndVersion(string templateId, int version);
        object GenerateTemplatePreview(string templateId, Dictionary<string, dynamic> parameters);
        object GetNotificationById(string notificationId);
        IEnumerable<object> GetNotifications(string templateType, string status, string reference, string olderThanId, bool includeSpreadsheetUploads);
        IEnumerable<object> GetReceivedTexts(string olderThanId);
    }
}
