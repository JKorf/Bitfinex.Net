using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class PositionUpdateRegistration : SubscriptionRegistration
    {
        private readonly Action<BitfinexSocketEvent<BitfinexPosition[]>> handler;

        public PositionUpdateRegistration(Action<BitfinexSocketEvent<BitfinexPosition[]>> handler, int streamId) 
            : base(typeof(BitfinexPosition), streamId, BitfinexEventType.PositionSnapshot,
                                                       BitfinexEventType.PositionNew,
                                                       BitfinexEventType.PositionUpdate,
                                                       BitfinexEventType.PositionClose)
        {
            this.handler = handler;
        }

        protected override void Handle(BitfinexEventType type, object obj)
        {
            handler(new BitfinexSocketEvent<BitfinexPosition[]>(type, (BitfinexPosition[])obj));
        }
    }
}
