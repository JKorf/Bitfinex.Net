using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class FundingOffersUpdateRegistration: SubscriptionRegistration
    {
        private readonly Action<BitfinexSocketEvent<BitfinexFundingOffer[]>> handler;

        public FundingOffersUpdateRegistration(Action<BitfinexSocketEvent<BitfinexFundingOffer[]>> handler, int streamId) 
            : base(typeof(BitfinexFundingOffer), streamId, BitfinexEventType.FundingOfferSnapshot,
                                                           BitfinexEventType.FundingOfferNew,
                                                           BitfinexEventType.FundingOfferUpdate,
                                                           BitfinexEventType.FundingOfferCancel)
        {
            this.handler = handler;
        }

        protected override void Handle(BitfinexEventType type, object obj)
        {
            handler(new BitfinexSocketEvent<BitfinexFundingOffer[]>(type, (BitfinexFundingOffer[])obj));
        }
    }
}
