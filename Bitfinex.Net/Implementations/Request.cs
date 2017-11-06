using System.Net;
using Bitfinex.Net.Interfaces;

namespace Bitfinex.Net.Implementations
{
    public class Request : IRequest
    {
        private readonly WebRequest request;

        public Request(WebRequest request)
        {
            this.request = request;
        }

        public WebHeaderCollection Headers
        {
            get => request.Headers;
            set => request.Headers = value;
        }

        public string ContentType
        {
            get => request.ContentType;
            set => request.ContentType = value;
        }

        public string Method
        {
            get => request.Method;
            set => request.Method = value;
        }

        public IResponse GetResponse()
        {
            return new Response(request.GetResponse());
        }
    }
}
