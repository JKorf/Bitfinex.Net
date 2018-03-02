using System;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    public class TradesUpdateRegistration : SubscriptionRegistration
    {
        private Action<BitfinexTradeDetails[]> handler;

        public TradesUpdateRegistration(Action<BitfinexTradeDetails[]> handler) : base(typeof(BitfinexTradeDetails), "te", "tu")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexTradeDetails[])obj);
        }
    }
}
