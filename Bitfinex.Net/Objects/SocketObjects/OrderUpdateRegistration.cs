using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class OrderUpdateRegistration: SubscriptionRegistration
    {
        private Action<BitfinexOrder[]> handler;

        public OrderUpdateRegistration(Action<BitfinexOrder[]> handler, int streamId) : base(typeof(BitfinexOrder), streamId, "os", "ou", "on", "oc")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexOrder[]) obj);
        }
    }
}
