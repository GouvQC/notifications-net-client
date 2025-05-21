using System.ComponentModel.DataAnnotations;

public class EmailRequest
{

    public EmailRequest(string emailAddress, string templateId)
    {
        EmailAddress = emailAddress;
        TemplateId = templateId;
    }

   
    [Required]
    public string EmailAddress { get; set; }

    /// <summary>
    /// The unique identifier of the email template to use. (Required)
    /// </summary>
    /// <example>template-abc123-uuid</example>
    [Required]
    public string TemplateId { get; set; }

    /// <summary>
    /// A dictionary containing the personalisation data to fill in the template placeholders. (Optional)
    /// </summary>
    /// <example>{ "name": "Jane Doe", "orderId": "987654" }</example>
    public Dictionary<string, dynamic> Personalisation { get; set; } = new();

    /// <summary>
    /// A client-defined reference used for tracking or grouping email notifications. (Optional)
    /// </summary>
    /// <example>invoice-001</example>
    public string? ClientReference { get; set; }

    /// <summary>
    /// The ID of the reply-to email address configured in the system. (Optional)
    /// </summary>
    /// <example>reply-to-id-789</example>
    public string? EmailReplyToId { get; set; }

    /// <summary>
    /// A URL used for one-click unsubscribe functionality in the email. (Optional)
    /// </summary>
    /// <example>https://unsubscribe.example.com/abc123</example>
    public string? OneClickUnsubscribeURL { get; set; }

    /// <summary>
    /// The ISO 8601 date-time string indicating when the email should be sent. (Optional)
    /// </summary>
    /// <example>2025-05-20T14:00:00Z</example>
    public string? ScheduledFor { get; set; }

    /// <summary>
    /// The importance level of the email. Accepted values: high, normal, low. (Optional)
    /// </summary>
    /// <example>high</example>
    public string? Importance { get; set; }

    /// <summary>
    /// The email address to receive a copy (CC) of the message. (Optional)
    /// </summary>
    /// <example>cc@example.com</example>
    public string? CcAddress { get; set; }
}
