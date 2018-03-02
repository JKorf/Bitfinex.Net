using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    public class OrderUpdateRegistration: SubscriptionRegistration
    {
        private Action<BitfinexOrder[]> handler;

        public OrderUpdateRegistration(Action<BitfinexOrder[]> handler) : base(typeof(BitfinexOrder), "os", "ou", "on", "oc")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexOrder[]) obj);
        }
    }
}
