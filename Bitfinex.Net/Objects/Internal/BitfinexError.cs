//using Bitfinex.Net.Converters;
//using CryptoExchange.Net.Converters;

//namespace Bitfinex.Net.Objects.Internal
//{
//    [JsonConverter(typeof(ArrayConverter<BitfinexError, BitfinexSourceGenerationContext>))]
//    [SerializationModel]
//    internal class BitfinexError
//    {
//        [ArrayProperty(1)]
//        public int ErrorCode { get; set; }
//        [ArrayProperty(2)]
//        public string ErrorMessage { get; set; } = string.Empty;

//        public BitfinexError(int errorCode, string errorMessage)
//        {
//            ErrorCode = errorCode;
//            ErrorMessage = errorMessage;
//        }

//        public BitfinexError() { }
//    }
//}
