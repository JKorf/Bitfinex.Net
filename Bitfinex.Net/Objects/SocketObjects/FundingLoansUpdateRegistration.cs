using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class FundingLoansUpdateRegistration : SubscriptionRegistration
    {
        private Action<BitfinexFundingLoan[]> handler;

        public FundingLoansUpdateRegistration(Action<BitfinexFundingLoan[]> handler, int streamId) : base(typeof(BitfinexFundingLoan), streamId, "fls", "fln", "flu", "flc")
        {
            this.handler = handler;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexFundingLoan[])obj);
        }
    }
}
