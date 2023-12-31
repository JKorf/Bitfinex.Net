//using Bitfinex.Net.Objects.Internal;
//using CryptoExchange.Net.Converters;
//using CryptoExchange.Net.Interfaces;
//using CryptoExchange.Net.Objects.Sockets;
//using CryptoExchange.Net.Sockets;

//namespace Bitfinex.Net.Objects.Sockets
//{
//    internal class BitfinexSocketConverter : SocketConverter
//    {
//        public override MessageInterpreterPipeline InterpreterPipeline { get; } = new MessageInterpreterPipeline
//        {
//            GetStreamIdentifier = GetStreamIdentifier,
//            GetTypeIdentifier = GetTypeIdentifier
//        };

//        private static string? GetStreamIdentifier(IMessageAccessor accessor)
//        {
//            if (!accessor.IsObject(null))
//            {
//                return accessor.GetArrayIntValue(null, 0).ToString();
//            }

//            var evnt = accessor.GetStringValue("event");
//            if (evnt == "info")
//                return "info";

//            var channel = accessor.GetStringValue("channel");
//            var symbol = accessor.GetStringValue("symbol");
//            var prec = accessor.GetStringValue("prec");
//            var freq = accessor.GetStringValue("freq");
//            var len = accessor.GetStringValue("len");
//            var key = accessor.GetStringValue("key");
//            return evnt + channel + symbol + prec + freq + len + key;
//        }

//        private static string? GetTypeIdentifier(IMessageAccessor accessor)
//        {
//            if (accessor.IsObject(null))
//                return null;

//            var topic = accessor.GetArrayStringValue(null, 1);
//            if (topic == "hb")
//                return "hb";
//            if (topic == "cs")
//                return "cs";

//            var dataIndex = topic == null ? 1 : 2;
//            var x = "single";
//            if (accessor.IsArray(new[] { dataIndex, 0 }))
//                x = "array";

//            return x + dataIndex;
//        }
//    }
//}
