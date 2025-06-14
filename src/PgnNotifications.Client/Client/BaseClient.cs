﻿using Newtonsoft.Json;
using PgnNotifications.Authentication;
using PgnNotifications.Exceptions;
using PgnNotifications.Interfaces;
using PgnNotifications.Models.Responses;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PgnNotifications.Client
{
    public class BaseClient : IBaseClient
    {
        private const int SERVICE_ID_START_POSITION = 73;
        private const int SERVICE_API_KEY_START_POSITION = 36;
        private const int GUID_LENGTH = 36;
        private const string NOTIFY_CLIENT_NAME = "NOTIFY-API-NET-CLIENT/";
        private const string BASE_URL = "https://gw-gouvqc.mcn.api.gouv.qc.ca/pgn/";
        private const string PROXY_PGGAPI = "mcn.api.gouv.qc.ca";

        public string BaseUrl;

        private readonly IHttpClient client;
        private readonly string serviceId;
        private readonly string apiKey;
        private readonly string clientId;
        private readonly string proxyPggapi;

        public BaseClient(IHttpClient client, string apiKey, string clientId, string baseUrl = BASE_URL)
        {           
            var serviceCredentials = ExtractServiceIdAndApiKey(apiKey);
            serviceId = serviceCredentials.Item1;
            this.apiKey = serviceCredentials.Item2;

            BaseUrl = string.IsNullOrEmpty(baseUrl) ? BASE_URL : baseUrl;


            this.client = client;
            this.client.BaseAddress = ValidateBaseUri(BaseUrl);
            this.proxyPggapi = PROXY_PGGAPI;
            this.clientId = ValidateClientId(clientId, BaseUrl, proxyPggapi);
            this.client.AddContentHeader("application/json");

            var productVersion = typeof(BaseClient).GetTypeInfo().Assembly.GetName().Version.ToString();
            this.client.AddUserAgent(NOTIFY_CLIENT_NAME + productVersion);
        }


        public async Task<string> GET(string url)
        {
            return await MakeRequest(url, HttpMethod.Get).ConfigureAwait(false);
        }

        public async Task<string> POST(string url, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await MakeRequest(url, HttpMethod.Post, content).ConfigureAwait(false);
        }

        public async Task<byte[]> GETBytes(string url)
        {
            return await MakeRequestBytes(url, HttpMethod.Get).ConfigureAwait(false);
        }

        public async Task<byte[]> MakeRequestBytes(string url, HttpMethod method, HttpContent content = null)
        {
            var response = SendRequest(url, method, content).Result;

            var responseContent = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                // if there was an error, rather than a binary pdf, the http body will be a json error message, so
                // encode the bytes as UTF8
                HandleHTTPErrors(response, Encoding.UTF8.GetString(responseContent));
            }
            return responseContent;

        }

        public async Task<string> MakeRequest(string url, HttpMethod method, HttpContent content = null)
        {
            var response = SendRequest(url, method, content).Result;

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                HandleHTTPErrors(response, responseContent);
            }
            return responseContent;
        }

        private async Task<HttpResponseMessage> SendRequest(string url, HttpMethod method, HttpContent content)
        {
            var request = new HttpRequestMessage(method, url);

            if (content != null)
            {
                request.Content = content;
            }

            var notifyToken = Authenticator.CreateToken(this.apiKey, this.serviceId);
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + notifyToken);

            if (!string.IsNullOrWhiteSpace(this.clientId))
            {
                request.Headers.TryAddWithoutValidation("X-QC-Client-Id", this.clientId);
            }

            HttpResponseMessage response;

            try
            {
                response = await this.client.SendAsync(request).ConfigureAwait(false);
            }
            catch (AggregateException ae)
            {
                ae.Handle(x =>
                {
                    if (x is HttpRequestException)
                    {
                        throw new NotifyClientException(x.InnerException.Message);
                    }
                    throw x;
                });
                throw ae.Flatten();
            }
            return response;
        }

        private void HandleHTTPErrors(HttpResponseMessage response, string errorResponseContent)
        {
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<NotifyHTTPErrorResponse>(errorResponseContent);
                throw new NotifyClientException("Status code {0}. The following errors occured {1}", errorResponse.getStatusCode(), errorResponse.getErrorsAsJson());
            }
            catch (Exception ex)
            {
                throw new NotifyClientException("Status code {0}. Error: {1}, Exception: {2}", response.StatusCode.GetHashCode(), errorResponseContent, ex.Message);
            }
        }

        public Tuple<string, string> ExtractServiceIdAndApiKey(string fromApiKey)
        {
            if (fromApiKey.Length < 73 || string.IsNullOrWhiteSpace(fromApiKey) || fromApiKey.Contains(" "))
            {
                throw new NotifyAuthException("The API Key provided is invalid. Please ensure you are using a v2 API Key that is not empty or null");
                                                 
            }

            var serviceId = fromApiKey.Substring(fromApiKey.Length - SERVICE_ID_START_POSITION, GUID_LENGTH);
            var apiKey = fromApiKey.Substring(fromApiKey.Length - SERVICE_API_KEY_START_POSITION, GUID_LENGTH);

            return Tuple.Create(serviceId, apiKey);
        }

        public Uri ValidateBaseUri(string baseUrl)
        {
            var result = Uri.TryCreate(baseUrl, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == "http" || uriResult.Scheme == "https");

            if (!result)
            {
                throw new NotifyAuthException("Invalid URL provided");
            }

            return uriResult;

        }

        private string ValidateClientId(string clientId, string baseUrl, string proxyPggapi)
        {
            if (baseUrl.Contains(proxyPggapi))
            {
                if (string.IsNullOrWhiteSpace(clientId) || clientId.Contains(" "))
                {
                    throw new NotifyAuthException("A valid client identifier (X-QC-Client-Id) is required when using the PGGAPI proxy.");
                }
            }          

            return clientId;
        }

        public string GetUserAgent()
        {
            return NOTIFY_CLIENT_NAME + typeof(BaseClient).GetTypeInfo().Assembly.GetName().Version;
        }
    }
}
