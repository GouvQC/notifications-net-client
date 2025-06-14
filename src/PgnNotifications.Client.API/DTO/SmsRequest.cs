using System.ComponentModel.DataAnnotations;

public class SmsRequest(string mobileNumber, string templateId)
{

    /// <summary>The mobile phone number to receive the SMS</summary>
    /// <example>+5511999999999</example>
    [Required]
    public string MobileNumber { get; set; } = mobileNumber;

    /// <summary>
    /// The unique identifier of the notification template to use. (Required)
    /// </summary>
    /// <example>template-12345-uuid</example>
    [Required]
    public string TemplateId { get; set; } = templateId;

    /// <summary>
    /// A dictionary containing the personalisation data to fill in the template placeholders. (Optional)
    /// </summary>
    /// <example>{ "name": "John Doe", "code": "123456" }</example>
    public Dictionary<string, dynamic> Personalisation { get; set; } = new();

    /// <summary>
    /// A client-defined reference that can be used for tracking or grouping notifications. (Optional)
    /// </summary>
    /// <example>order-98765</example>
    public string? Reference { get; set; }

    /// <summary>
    /// The ID of the sender to display as the originator of the SMS. This must be configured in the notification system. (Optional)
    /// </summary>
    /// <example>MyCompanySMS</example>
    public string? SmsSenderId { get; set; }
}