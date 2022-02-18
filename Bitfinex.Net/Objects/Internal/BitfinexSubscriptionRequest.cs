using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Net.Objects.Internal
{
    internal class BitfinexUnsubscribeRequest
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("chanId")]
        public int ChannelId { get; set; }

        public BitfinexUnsubscribeRequest(int channelId)
        {
            Event = "unsubscribe";
            ChannelId = channelId;
        }
    }

    internal class BitfinexSubscriptionRequest
    {
        [JsonIgnore]
        public int ChannelId { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        public BitfinexSubscriptionRequest(string channel, string symbol)
        {
            Event = "subscribe";
            Channel = channel;
            Symbol = symbol;
        }

        public virtual bool CheckResponse(JToken responseMessage)
        {
            if (responseMessage["channel"] == null || responseMessage["channel"]!.ToString() != Channel)
                return false;

            if (responseMessage["symbol"] == null)
                return false;

            var symbol = responseMessage["symbol"]!.ToString().ToLower(CultureInfo.InvariantCulture);
            if (symbol != Symbol.ToLower(CultureInfo.InvariantCulture))
            {
                if (symbol.StartsWith("t"))
                {
                    // Check if 
                    if(symbol.Substring(1) != Symbol.ToLower(CultureInfo.InvariantCulture))
                        return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }

    internal class BitfinexRawBookSubscriptionRequest: BitfinexSubscriptionRequest
    {
        [JsonProperty("prec")]
        public string Precision { get; set; }
        [JsonProperty("len")]
        public int Length { get; set; }

        public BitfinexRawBookSubscriptionRequest(string symbol, string precision, int length) : base("book", symbol)
        {
            Precision = precision;
            Length = length;
        }

        public override bool CheckResponse(JToken responseMessage)
        {
            if (!base.CheckResponse(responseMessage))
                return false;

            if (responseMessage["prec"] == null || (responseMessage["prec"]!.ToString()) != Precision)
                return false;

            if (responseMessage["len"] == null || responseMessage["len"]!.Value<int>() != Length)
                return false;

            return true;
        }
    }

    internal class BitfinexBookSubscriptionRequest : BitfinexRawBookSubscriptionRequest
    {
        [JsonProperty("freq")]
        public string Frequency { get; set; }

        public BitfinexBookSubscriptionRequest(string symbol, string precision, string frequency, int length): base(symbol, precision, length)
        {
            Frequency = frequency;
        }

        public override bool CheckResponse(JToken responseMessage)
        {
            if (!base.CheckResponse(responseMessage))
                return false;

            if (responseMessage["freq"] == null || responseMessage["freq"]!.ToString() != Frequency)
                return false;

            return true;
        }
    }

    internal class BitfinexKlineSubscriptionRequest : BitfinexSubscriptionRequest
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        public BitfinexKlineSubscriptionRequest(string symbol, string interval): base("candles", symbol)
        {
            Key = "trade:" + interval + ":" + symbol;
        }

        public override bool CheckResponse(JToken responseMessage)
        {
            if (responseMessage["channel"] == null || responseMessage["channel"]!.ToString() != Channel)
                return false;

            if (responseMessage["key"] == null || responseMessage["key"]!.ToString() != Key)
                return false;

            return true;
        }
    }
}
