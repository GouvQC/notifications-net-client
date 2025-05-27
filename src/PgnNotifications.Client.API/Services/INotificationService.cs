using PgnNotifications.Models.Responses;

namespace PgnNotifications.Client.API.Services
{   
    public interface INotificationService
    {
        SmsNotificationResponse SendSms(string mobileNumber, string templateId, Dictionary<string, dynamic> personalisation = null,
                                        string clientReference = null, string smsSenderId = null);
        EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation,
                                            string clientReference = null, string emailReplyToId = null, string oneClickUnsubscribeURL = null,
                                            string scheduledFor = null, string importance = null, string ccAddress = null);
        IEnumerable<TemplateList> GetAllTemplates(string templateType);
        object GetTemplateById(string templateId);
        object GetTemplateByIdAndVersion(string templateId, int version);
        object GenerateTemplatePreview(string templateId, Dictionary<string, dynamic> personalisation);
        object GetNotificationById(string notificationId);
        IEnumerable<object> GetNotifications(string templateType, string status, string reference, string olderThanId, bool includeSpreadsheetUploads);
        IEnumerable<object> GetReceivedTexts(string olderThanId);
    }
}
