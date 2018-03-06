using System.Threading;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class WaitAction<T>
    {
        private ManualResetEvent evnt;
        private T data;

        public WaitAction()
        {
            evnt = new ManualResetEvent(false);
        }

        public T Wait(int timeout)
        {
            evnt.WaitOne(timeout);
            return data;
        }

        public void Set(T obj)
        {
            data = obj;
            evnt.Set();
        }
    }

    public class WaitAction
    {
        private ManualResetEvent evnt;

        public WaitAction()
        {
            evnt = new ManualResetEvent(false);
        }

        public bool Wait(int timeout)
        {
            return evnt.WaitOne(timeout);
        }

        public void Set()
        {
            evnt.Set();
        }
    }
}
