using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    internal class BitfinexError
    {
        [ArrayProperty(1)]
        public int ErrorCode { get; set; }
        [ArrayProperty(2)]
        public string ErrorMessage { get; set; } = string.Empty;

        public BitfinexError(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public BitfinexError() { }
    }
}
