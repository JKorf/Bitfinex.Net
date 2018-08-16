using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class FundingLoansUpdateRegistration : SubscriptionRegistration
    {
        private readonly Action<BitfinexSocketEvent<BitfinexFundingLoan[]>> handler;

        public FundingLoansUpdateRegistration(Action<BitfinexSocketEvent<BitfinexFundingLoan[]>> handler, int streamId)
            : base(typeof(BitfinexFundingLoan), streamId, BitfinexEventType.FundingLoanSnapshot,
                                                          BitfinexEventType.FundingLoanNew,
                                                          BitfinexEventType.FundingLoanUpdate,
                                                          BitfinexEventType.FundingLoanClose)
        {
            this.handler = handler;
        }

        protected override void Handle(BitfinexEventType type, object obj)
        {
            handler(new BitfinexSocketEvent<BitfinexFundingLoan[]>(type, (BitfinexFundingLoan[])obj));
        }
    }
}
