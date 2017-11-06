using System.IO;

namespace Bitfinex.Net.Interfaces
{
    public interface IResponse
    {
        Stream GetResponseStream();
    }
}
