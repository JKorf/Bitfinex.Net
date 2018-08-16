using System;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class WalletUpdateRegistration: SubscriptionRegistration
    {
        private readonly Action<BitfinexSocketEvent<BitfinexWallet[]>> handler;

        public WalletUpdateRegistration(Action<BitfinexSocketEvent<BitfinexWallet[]>> handler, int streamId) 
            : base(typeof(BitfinexWallet), streamId, BitfinexEventType.WalletSnapshot, 
                                                     BitfinexEventType.WalletUpdate)
        {
            this.handler = handler;
        }

        protected override void Handle(BitfinexEventType type, object obj)
        {
            handler(new BitfinexSocketEvent<BitfinexWallet[]>(type, (BitfinexWallet[]) obj));
        }
    }
}
