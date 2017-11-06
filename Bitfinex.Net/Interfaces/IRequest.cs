using System.Net;

namespace Bitfinex.Net.Interfaces
{
    public interface IRequest
    {
        WebHeaderCollection Headers { get; set; }
        string Method { get; set; }
        string ContentType { get; set; }

        IResponse GetResponse();
    }
}
