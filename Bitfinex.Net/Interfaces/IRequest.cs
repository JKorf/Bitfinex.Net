using System;
using System.IO;
using System.Net;

namespace Bitfinex.Net.Interfaces
{
    public interface IRequest
    {
        WebHeaderCollection Headers { get; set; }
        string Method { get; set; }
        string ContentType { get; set; }
        string Accept { get; set; }
        long ContentLength { get; set; }
        Uri RequestUri { get; }

        Stream GetRequestStream();
        IResponse GetResponse();
    }
}
