using System.Threading;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class WaitAction<T>
    {
        private readonly AutoResetEvent evnt;
        private CallResult<T> data;
        private bool set;

        public WaitAction()
        {
            evnt = new AutoResetEvent(false);
            data = new CallResult<T>(default(T), new UnknownError("Timeout waiting for action to complete"));
        }

        public bool Wait(out CallResult<T> result, int timeout)
        {
            evnt.WaitOne(timeout);
            result = data;
            return set;
        }

        public void Set(CallResult<T> obj)
        {
            data = obj;
            set = true;
            evnt.Set();
        }
    }
}
