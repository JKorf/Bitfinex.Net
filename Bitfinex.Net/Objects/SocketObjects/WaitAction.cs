using System.Threading;
using CryptoExchange.Net;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class WaitAction<T>
    {
        private AutoResetEvent evnt;
        private CallResult<T> data;

        public WaitAction()
        {
            evnt = new AutoResetEvent(false);
        }

        public CallResult<T> Wait(int timeout)
        {
            evnt.WaitOne(timeout);
            return data;
        }

        public void Set(CallResult<T> obj)
        {
            data = obj;
            evnt.Set();
        }
    }
}
