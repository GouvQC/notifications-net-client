using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PgnNotifications.Interfaces
{
    public interface IHttpClient : IDisposable
    {
        HttpRequestHeaders DefaultRequestHeaders { get; }
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
        Uri BaseAddress { get; set; }

        void SetClientBaseAddress();

        void AddAcceptHeader(string mediaType);

        void AddUserAgent(string userAgent);
    }
}
