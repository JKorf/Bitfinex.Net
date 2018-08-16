using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class OrderUpdateRegistration: SubscriptionRegistration
    {
        private readonly Action<BitfinexSocketEvent<BitfinexOrder[]>> handler;

        public OrderUpdateRegistration(Action<BitfinexSocketEvent<BitfinexOrder[]>> handler, int streamId) 
            : base(typeof(BitfinexOrder), streamId, BitfinexEventType.OrderSnapshot,
                                                    BitfinexEventType.OrderNew,
                                                    BitfinexEventType.OrderUpdate,
                                                    BitfinexEventType.OrderCancel)
        {
            this.handler = handler;
        }

        protected override void Handle(BitfinexEventType type, object obj)
        {
            handler(new BitfinexSocketEvent<BitfinexOrder[]>(type,(BitfinexOrder[]) obj));
        }
    }
}
