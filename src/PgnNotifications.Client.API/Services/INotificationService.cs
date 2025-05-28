using PgnNotifications.Models.Responses;

namespace PgnNotifications.Client.API.Services
{   
    public interface INotificationService
    {

        object GenerateTemplatePreview(string templateId, Dictionary<string, dynamic> personalisation);
        IEnumerable<TemplateList> GetAllTemplates(string templateType);
        object GetNotificationById(string notificationId);
        IEnumerable<object> GetNotifications(string templateType, string status, string reference, string olderThanId, bool includeSpreadsheetUploads);
        IEnumerable<object> GetReceivedTexts(string olderThanId);
        object GetTemplateById(string templateId);
        object GetTemplateByIdAndVersion(string templateId, int version);
        string CheckHealth();

        SmsNotificationResponse SendSms(string mobileNumber, string templateId, Dictionary<string, dynamic> personalisation,
                                        string? reference, string? smsSenderId);
        EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation,
                                            string? reference, string? emailReplyToId , string? oneClickUnsubscribeURL,
                                            string? scheduledFor, string? importance , string? ccAddress );
        HttpResponseMessage SendBulk(string templateId, string name, List<List<string?>> rows, string? csv,
                                                         string? reference, string? scheduledFor, string? emailReplyToId);
    }
}
