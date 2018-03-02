using System;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    public class PositionUpdateRegistration : SubscriptionRegistration
    {
        private Action<BitfinexPosition[]> handler;

        public PositionUpdateRegistration(Action<BitfinexPosition[]> handler) : base(typeof(BitfinexPosition), "ps", "pn", "pu", "pc")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexPosition[])obj);
        }
    }
}
