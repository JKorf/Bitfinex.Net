using CryptoExchange.Net;

namespace Bitfinex.Net
{
    public class BitfinexClientOptions : ExchangeOptions
    {
        public string BaseAddress { get; set; } = "https://api.bitfinex.com";
    }

    public class BitfinexSocketClientOptions: ExchangeOptions
    {
        public string BaseAddress { get; set; } = "wss://api.bitfinex.com/ws/2";
    }
}
