using PgnNotifications.Models;
using PgnNotifications.Models.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace PgnNotifications.Interfaces
{
    public interface INotificationClient : IBaseClient
    {
        TemplatePreviewResponse GenerateTemplatePreview(string templateId, Dictionary<string, dynamic> personalisation = null);

        TemplateList GetAllTemplates(string templateType = "");

        Notification GetNotificationById(string notificationId);

        NotificationList GetNotifications(string templateType = "", string status = "", string reference = "", string olderThanId = "", bool includeSpreadsheetUploads = false);

        ReceivedTextListResponse GetReceivedTexts(string olderThanId = "");

        TemplateResponse GetTemplateById(string templateId);

        TemplateResponse GetTemplateByIdAndVersion(string templateId, int version = 0);
        string CheckHealth();

        SmsNotificationResponse SendSms(string mobileNumber, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string smsSenderId = null);

        EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null, string oneClickUnsubscribeURL = null, string scheduledFor = null, string importance= null, string ccAddress = null);

        HttpResponseMessage SendBulkNotifications(string templateId, string name, List<List<string>> rows = null, string csv = null,
                                                         string clientReference = null, string scheduledFor = null, string emailReplyToId = null);
    }
}