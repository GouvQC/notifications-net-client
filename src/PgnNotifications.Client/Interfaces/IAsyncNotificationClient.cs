using PgnNotifications.Models;
using PgnNotifications.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PgnNotifications.Interfaces
{
    public interface IAsyncNotificationClient : IBaseClient
    {
        Task<TemplatePreviewResponse> GenerateTemplatePreviewAsync(string templateId, Dictionary<string, dynamic> personalisation = null);

        Task<TemplateList> GetAllTemplatesAsync(string templateType = "");

        Task<Notification> GetNotificationByIdAsync(string notificationId);

        Task<NotificationList> GetNotificationsAsync(string templateType = "", string status = "", string reference = "", string olderThanId = "", bool includeSpreadsheetUploads = false);

        Task<ReceivedTextListResponse> GetReceivedTextsAsync(string olderThanId = "");

        Task<TemplateResponse> GetTemplateByIdAsync(string templateId);

        Task<TemplateResponse> GetTemplateByIdAndVersionAsync(string templateId, int version = 0);

        Task<SmsNotificationResponse> SendSmsAsync(string mobileNumber, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string smsSenderId = null);

        Task<EmailNotificationResponse> SendEmailAsync(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null, string oneClickUnsubscribeURL = null, string scheduledFor = null, string importance = null, string ccAddress = null);
    }
}
