using System.Net;

namespace Bitfinex.Net.Interfaces
{
    public interface IRequest
    {
        WebHeaderCollection Headers { get; set; }
        string Method { get; set; }

        IResponse GetResponse();
    }
}
