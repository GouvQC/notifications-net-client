using Microsoft.AspNetCore.Mvc;
using PgnNotifications.Client.API.Services;

namespace PgnNotifications.Client.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationClientController(INotificationService notificationService) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;

        [HttpPost("SendSms")]
        public IActionResult SendSms([FromBody] SmsRequest request)
        {
            var response = _notificationService.SendSms(
                request.MobileNumber,
                request.TemplateId,
                request.Personalisation,
                request.ClientReference,
                request.SmsSenderId
            );

            return Ok(response);
        }      

        [HttpPost("SendEmail")]
        public IActionResult SendEmail([FromBody] EmailRequest request)
        {
            // Garantindo que campos obrigatórios estejam presentes.
            if (string.IsNullOrWhiteSpace(request.EmailAddress))
            {
                return BadRequest("The 'EmailAddress' field is required.");
            }
            if (string.IsNullOrWhiteSpace(request.TemplateId))
            {
                return BadRequest("The 'TemplateId' field is required.");
            }

            // Chamando o serviço para enviar o email.
            var response = _notificationService.SendEmail(
                request.EmailAddress,
                request.TemplateId,
                request.Personalisation,
                request.ClientReference,
                request.EmailReplyToId,
                request.OneClickUnsubscribeURL,
                request.ScheduledFor,
                request.Importance,
                request.CcAddress
            );

            // Retornando a resposta
            return Ok(response);
        }


        [HttpGet("GetAllTemplates")]
        public IActionResult GetAllTemplates([FromHeader] string templateType = "")
        {
            var templates = _notificationService.GetAllTemplates(templateType);
            return Ok(templates);
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

        [HttpPost("GenerateTemplatePreview")]
        public IActionResult GenerateTemplatePreview([FromQuery] string templateId)
        {
            var preview = _notificationService.GenerateTemplatePreview(templateId, new Dictionary<string, dynamic> { { "name", "Test Generate Template Preview" } });
            return Ok(preview);
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
    }
}
