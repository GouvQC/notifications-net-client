using Microsoft.AspNetCore.Mvc;


namespace PgnNotifications.Client.API.Controllers
{
    public class BaseAuthorizedController : ControllerBase
    {
        protected IActionResult? ValidateHeaders(out NotificationClient? client)
        {
            var apiKey = Request.Headers["X-Api-Key"].ToString();
            var clientId = Request.Headers["X-Client-Id"].ToString();

            client = null;

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(clientId) )
            {
                return Unauthorized(new { message = "X-Api-Key or X-Client-Id are required." });
            }

            client = new NotificationClient(apiKey, clientId);
            return null;
        }
    }
}