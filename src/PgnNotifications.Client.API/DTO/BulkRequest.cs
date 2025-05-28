
using System.ComponentModel.DataAnnotations;

public class BulkRequest(string templateId, string name, List<List<string?>> rows)
{

    /// <summary>
    /// The unique identifier of the template to use for sending the notifications. (Required)
    /// </summary>
    /// <example>template-abc123-uuid</example>
    [Required]
    public string TemplateId { get; set; } = templateId;

    /// <summary>
    /// The name of the bulk send operation. (Required)
    /// </summary>
    /// <example>My Bulk Send Operation</example>
    [Required]
    public string Name { get; set; } = name;

    /// <summary>
    /// A list of lists representing the data rows for personalisation. (Required)
    /// </summary>
    /// <example>
    /// [
    ///     ["email address", "name"],
    ///     ["user@example.com", "John"]
    /// ]
    /// </example>
    [Required]
    public List<List<string?>> Rows { get; set; } = rows;

    /// <summary>
    /// The raw CSV content for sending notifications. Use this as an alternative to rows. (Optional)
    /// </summary>
    /// <example>email address,name\nuser@example.com,John</example>
    public string? Csv { get; set; }

    /// <summary>
    /// A client-defined reference used for tracking or grouping notifications. (Optional)
    /// </summary>
    /// <example>bulk-send-ref-001</example>
    public string? Reference { get; set; }

    /// <summary>
    /// The ISO 8601 date-time string indicating when the notification should be sent. (Optional)
    /// </summary>
    /// <example>2025-05-27T10:00:00Z</example>
    public string? ScheduledFor { get; set; }

    /// <summary>
    /// The ID of the reply-to email address configured in the system. (Optional)
    /// </summary>
    /// <example>reply-to-id-456</example>
    public string? EmailReplyToId { get; set; }
}
