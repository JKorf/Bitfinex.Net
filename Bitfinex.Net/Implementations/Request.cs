using System;
using System.IO;
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

        public string Accept
        {
            get => ((HttpWebRequest) request).Accept;
            set => ((HttpWebRequest) request).Accept = value;
        }

        public long ContentLength
        {
            get => ((HttpWebRequest)request).ContentLength;
            set => ((HttpWebRequest)request).ContentLength = value;
        }

        public string Method
        {
            get => request.Method;
            set => request.Method = value;
        }

        public Uri RequestUri => request.RequestUri;

        public Stream GetRequestStream()
        {
            return request.GetRequestStream();
        }

        public IResponse GetResponse()
        {
            return new Response(request.GetResponse());
        }
    }
}
