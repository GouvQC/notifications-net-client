using Microsoft.AspNetCore.Mvc;
using PgnNotifications.Client.API.Services;

namespace PgnNotifications.Client.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationClientController(INotificationService notificationService) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;        

        [HttpGet("GetAllTemplates")]
        public IActionResult GetAllTemplates([FromHeader] string templateType = "")
        {
            var templates = _notificationService.GetAllTemplates(templateType);
            return Ok(templates);
        }

        [HttpGet("GetNotificationById")]
        public IActionResult GetNotificationById([FromQuery] string notificationId)
        {
            var notification = _notificationService.GetNotificationById(notificationId);
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
            var notifications = _notificationService.GetNotifications(templateType, status, reference, olderThanId, includeSpreadsheetUploads);
            return Ok(notifications);
        }

        [HttpGet("GetReceivedTexts")]
        public IActionResult GetReceivedTexts([FromQuery] string olderThanId = "")
        {
            var received = _notificationService.GetReceivedTexts(olderThanId);
            return Ok(received);
        }

        [HttpGet("GetTemplateById")]
        public IActionResult GetTemplateById([FromQuery] string templateId)
        {
            var template = _notificationService.GetTemplateById(templateId);
            return Ok(template);
        }

        [HttpGet("GetTemplateByIdAndVersion")]
        public IActionResult GetTemplateByIdAndVersion([FromQuery] string templateId, [FromQuery] int version = 0)
        {
            var template = _notificationService.GetTemplateByIdAndVersion(templateId, version);
            return Ok(template);
        }

        [HttpGet("CheckHealth")]
        public IActionResult CheckHealth()
        {
            var received = _notificationService.CheckHealth();
            return Ok(received);
        }

        [HttpPost("GenerateTemplatePreview")]
        public IActionResult GenerateTemplatePreview([FromQuery] string templateId)
        {
            var preview = _notificationService.GenerateTemplatePreview(templateId, new Dictionary<string, dynamic> { { "name", "Test Generate Template Preview" } });
            return Ok(preview);
        }

        [HttpPost("SendSms")]
        public IActionResult SendSms([FromBody] SmsRequest request)
        {
            var response = _notificationService.SendSms(
                request.MobileNumber,
                request.TemplateId,
                request.Personalisation,
                request.Reference,
                request.SmsSenderId
            );

            return Ok(response);
        }

        [HttpPost("SendEmail")]
        public IActionResult SendEmail([FromBody] EmailRequest request)
        {
            //if (string.IsNullOrWhiteSpace(request.EmailAddress))
            //{
            //    return BadRequest("The 'EmailAddress' field is required.");
            //}
            //if (string.IsNullOrWhiteSpace(request.TemplateId))
            //{
            //    return BadRequest("The 'TemplateId' field is required.");
            //}

            var response = _notificationService.SendEmail(
                request.EmailAddress,
                request.TemplateId,
                request.Personalisation,
                request.Reference,
                request.EmailReplyToId,
                request.OneClickUnsubscribeURL,
                request.ScheduledFor,
                request.Importance,
                request.CcAddress
            );

            return Ok(response);
        }
        [HttpPost("SendBulk")]
        public IActionResult SendBulk([FromBody] BulkRequest request)
        {
            var response = _notificationService.SendBulk(                
                request.TemplateId, 
                request.Name,
                request.Rows,
                request.Csv,
                request.Reference,
                request.ScheduledFor,
                request.EmailReplyToId
            );

            return Ok(response);
        }
    }
}
