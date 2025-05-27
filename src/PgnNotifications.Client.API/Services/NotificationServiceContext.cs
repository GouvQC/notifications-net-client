namespace PgnNotifications.Client.API.Services
{
    public class NotificationServiceContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string? ApiKey { get; set; }
        public string? ClientId { get; set; }
        public string? BaseUrl { get; }

        public NotificationServiceContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            ApiKey = _httpContextAccessor.HttpContext?.Request.Headers["X-Api-Key"].ToString();
            ClientId = _httpContextAccessor.HttpContext?.Request.Headers["X-Client-Id"].ToString();
            BaseUrl = httpContextAccessor.HttpContext?.Request.Headers["X-Base-Url"].ToString();

            if (string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(ClientId))
            {
                throw new ArgumentException("X-Api-Key or X-Client-Id are required.");
            }
        }
    }
}
