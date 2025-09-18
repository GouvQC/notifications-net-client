using System;
using System.Net.Http;

namespace PgnNotifications.Client.Builders
{
    internal class NotificationClientBuilder
    {
        private string _apiKey;
        private string _clientId;
        private TimeSpan? _timeout;
        private string _baseUrl;
        private HandlerBuilder _handlerBuilder;

        private NotificationClientBuilder() { }

        public static NotificationClientBuilder Create()
        {
            return new NotificationClientBuilder();
        }

        public NotificationClientBuilder WithApiKey(string apiKey)
        {
            _apiKey = apiKey;
            return this;
        }

        public NotificationClientBuilder WithClientId(string clientId)
        {
            _clientId = clientId;
            return this;
        }

        public NotificationClientBuilder WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        public NotificationClientBuilder WithBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }

        public NotificationClientBuilder WithHandlerBuilder(Action<HandlerBuilder> config)
        {
            var builder = HandlerBuilder.Create();
            config(builder);
            _handlerBuilder = builder;
            return this;
        }

        public NotificationClient Build()
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new PgnNotifications.Exceptions.NotifyAuthException("API key is required");
            if (string.IsNullOrWhiteSpace(_clientId))
                throw new PgnNotifications.Exceptions.NotifyAuthException("Client ID is required");

            HttpMessageHandler handler = null;
            if (_handlerBuilder != null)
                handler = _handlerBuilder.Build();

            if (handler != null)
            {
                var httpClient = new HttpClient(handler, disposeHandler: true);
                if (_timeout.HasValue)
                    httpClient.Timeout = _timeout.Value;
                if (!string.IsNullOrWhiteSpace(_baseUrl))
                    return new NotificationClient(new HttpClientWrapper(httpClient), _apiKey, _clientId, _baseUrl);
                else
                    return new NotificationClient(new HttpClientWrapper(httpClient), _apiKey, _clientId);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(_baseUrl))
                    return new NotificationClient(_baseUrl, _apiKey, _clientId);
                else
                    return new NotificationClient(_apiKey, _clientId);
            }
        }
    }
}
