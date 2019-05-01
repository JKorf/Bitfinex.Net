using System;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Objects
{
    public class BitfinexClientOptions : ClientOptions
    {
        public BitfinexClientOptions()
        {
            BaseAddress = "https://api.bitfinex.com";
        }
    }

    public class BitfinexSocketClientOptions: SocketClientOptions
    {
        public BitfinexSocketClientOptions()
        {
            SocketSubscriptionsCombineTarget = 10;
            BaseAddress = "wss://api.bitfinex.com/ws/2";
            SocketNoDataTimeout = TimeSpan.FromSeconds(30);
        }
    }
}
