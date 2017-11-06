using System.IO;
using System.Net;
using Bitfinex.Net.Interfaces;

namespace Bitfinex.Net.Implementations
{
    public class Response : IResponse
    {
        private readonly WebResponse response;

        public Response(WebResponse response)
        {
            this.response = response;
        }

        public Stream GetResponseStream()
        {
            return response.GetResponseStream();
        }
    }
}
