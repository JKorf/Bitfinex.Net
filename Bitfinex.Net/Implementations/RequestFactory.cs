using System.Net;
using Bitfinex.Net.Interfaces;

namespace Bitfinex.Net.Implementations
{
    public class RequestFactory : IRequestFactory
    {
        public IRequest Create(string uri)
        {
            return new Request(WebRequest.Create(uri));
        }
    }
}
