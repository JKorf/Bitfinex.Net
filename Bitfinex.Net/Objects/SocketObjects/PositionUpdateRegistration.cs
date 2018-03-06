using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class PositionUpdateRegistration : SubscriptionRegistration
    {
        private Action<BitfinexPosition[]> handler;

        public PositionUpdateRegistration(Action<BitfinexPosition[]> handler, int streamId) : base(typeof(BitfinexPosition), streamId, "ps", "pn", "pu", "pc")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexPosition[])obj);
        }
    }
}
