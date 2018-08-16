using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class TradesUpdateRegistration : SubscriptionRegistration
    {
        private readonly Action<BitfinexSocketEvent<BitfinexTradeDetails[]>> handler;

        public TradesUpdateRegistration(Action<BitfinexSocketEvent<BitfinexTradeDetails[]>> handler, int streamId) 
            : base(typeof(BitfinexTradeDetails), streamId, BitfinexEventType.TradeExecuted, 
                                                           BitfinexEventType.TradeExecutionUpdate)
        {
            this.handler = handler;
        }

        protected override void Handle(BitfinexEventType type, object obj)
        {
            handler(new BitfinexSocketEvent<BitfinexTradeDetails[]>(type, (BitfinexTradeDetails[])obj));
        }
    }
}
