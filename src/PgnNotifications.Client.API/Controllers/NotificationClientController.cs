using Microsoft.AspNetCore.Mvc;

namespace PgnNotifications.Client.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationClientController : BaseAuthorizedController
    {
        [HttpPost("SendSms")]
        public IActionResult SendSms(
            [FromHeader(Name = "mobileNumber")] string mobileNumber,
            [FromHeader(Name = "templateId")] string templateId)
        {
            var validationResult = ValidateHeaders(out var client);
            if (validationResult != null) return validationResult;

            var response = client.SendSms(mobileNumber, templateId, new Dictionary<string, dynamic> { { "name", "Test User" } });
            return Ok(response);
        }

        [HttpPost("SendEmail")]
        public IActionResult SendEmail([FromHeader] string emailAddress, [FromHeader] string templateId)
        {
            var validationResult = ValidateHeaders(out var client);
            if (validationResult != null) return validationResult;

            var response = client.SendEmail(emailAddress, templateId, new Dictionary<string, dynamic> { { "name", "Test User" } });
            return Ok(response);
        }

        [HttpGet("GetAllTemplates")]
        public IActionResult GetAllTemplates([FromQuery] string templateType = "")
        {
            var validationResult = ValidateHeaders(out var client);
            if (validationResult != null) return validationResult;

            var templates = client.GetAllTemplates(templateType);
            return Ok(templates);
        }

        [HttpGet("GetTemplateById")]
        public IActionResult GetTemplateById([FromQuery] string templateId)
        {
            var validationResult = ValidateHeaders(out var client);
            if (validationResult != null) return validationResult;

            var template = client.GetTemplateById(templateId);
            return Ok(template);
        }

        [HttpGet("GetTemplateByIdAndVersion")]
        public IActionResult GetTemplateByIdAndVersion([FromQuery] string templateId, [FromQuery] int version = 0)
        {
            var validationResult = ValidateHeaders(out var client);
            if (validationResult != null) return validationResult;

            var template = client.GetTemplateByIdAndVersion(templateId, version);
            return Ok(template);
        }

        [HttpPost("GenerateTemplatePreview")]
        public IActionResult GenerateTemplatePreview([FromQuery] string templateId)
        {
            var validationResult = ValidateHeaders(out var client);
            if (validationResult != null) return validationResult;

            var preview = client.GenerateTemplatePreview(templateId, new Dictionary<string, dynamic> { { "name", "Test User" } });
            return Ok(preview);
        }

        [HttpGet("GetNotificationById")]
        public IActionResult GetNotificationById([FromQuery] string notificationId)
        {
            var validationResult = ValidateHeaders(out var client);
            if (validationResult != null) return validationResult;

            var notification = client.GetNotificationById(notificationId);
            return Ok(notification);
        }

        [HttpGet("GetNotifications")]
        public IActionResult GetNotifications(
            [FromQuery] string templateType = "",
            [FromQuery] string status = "",
            [FromQuery] string reference = "",
            [FromQuery] string olderThanId = "",
            [FromQuery] bool includeSpreadsheetUploads = false)
        {
            var validationResult = ValidateHeaders(out var client);
            if (validationResult != null) return validationResult;

            var notifications = client.GetNotifications(templateType, status, reference, olderThanId, includeSpreadsheetUploads);
            return Ok(notifications);
        }

        [HttpGet("GetReceivedTexts")]
        public IActionResult GetReceivedTexts([FromQuery] string olderThanId = "")
        {
            var validationResult = ValidateHeaders(out var client);
            if (validationResult != null) return validationResult;

            var received = client.GetReceivedTexts(olderThanId);
            return Ok(received);
        }
    }
}
