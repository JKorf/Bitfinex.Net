using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class FundingCreditsUpdateRegistration : SubscriptionRegistration
    {
        private readonly Action<BitfinexSocketEvent<BitfinexFundingCredit[]>> handler;

        public FundingCreditsUpdateRegistration(Action<BitfinexSocketEvent<BitfinexFundingCredit[]>> handler, int streamId) 
            : base(typeof(BitfinexFundingCredit), streamId, BitfinexEventType.FundingCreditsSnapshot,
                                                            BitfinexEventType.FundingCreditsNew,
                                                            BitfinexEventType.FundingCreditsUpdate,
                                                            BitfinexEventType.FundingCreditsClose)
        {
            this.handler = handler;
        }

        protected override void Handle(BitfinexEventType type, object obj)
        {
            handler(new BitfinexSocketEvent<BitfinexFundingCredit[]>(type, (BitfinexFundingCredit[])obj));
        }
    }
}
