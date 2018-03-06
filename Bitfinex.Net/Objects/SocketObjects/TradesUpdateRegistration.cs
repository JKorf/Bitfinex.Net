using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class TradesUpdateRegistration : SubscriptionRegistration
    {
        private Action<BitfinexTradeDetails[]> handler;

        public TradesUpdateRegistration(Action<BitfinexTradeDetails[]> handler, int streamId) : base(typeof(BitfinexTradeDetails), streamId, "te", "tu")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexTradeDetails[])obj);
        }
    }
}
