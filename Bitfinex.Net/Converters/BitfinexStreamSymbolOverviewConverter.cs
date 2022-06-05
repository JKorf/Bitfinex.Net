using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using System.Linq;
using System.Reflection;

namespace Bitfinex.Net.Converters
{
    internal class BitfinexStreamSymbolOverviewConverter : JsonConverter
    {
        public override bool CanWrite { get { return false; } }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var data = JArray.Load(reader);
            if(data[0].Type == JTokenType.Array)
            {
                var result = new List<BitfinexStreamSymbolOverview>();
                foreach(var entry in data)                
                    result.Add(ParseEntry((JArray)entry));                
                return result;
            }
            else
            {
                return ParseEntry(data);
            }           
        }

        private static BitfinexStreamSymbolOverview ParseEntry(JArray data)
        {
            BitfinexStreamSymbolOverview result;

            if (data.Count == 16)
            {
                result = new BitfinexStreamSymbolOverview()
                {
                    Frr = decimal.Parse(data[0].ToString(), NumberStyles.Float),
                    BestBidPrice = decimal.Parse(data[1].ToString(), NumberStyles.Float),
                    BestBidPeriod = decimal.Parse(data[2].ToString(), NumberStyles.Float),
                    BestBidQuantity = decimal.Parse(data[3].ToString(), NumberStyles.Float),
                    BestAskPrice = decimal.Parse(data[4].ToString(), NumberStyles.Float),
                    BestAskPeriod = decimal.Parse(data[5].ToString(), NumberStyles.Float),
                    BestAskQuantity = decimal.Parse(data[6].ToString(), NumberStyles.Float),
                    DailyChange = decimal.Parse(data[7].ToString(), NumberStyles.Float),
                    DailyChangePercentage = decimal.Parse(data[8].ToString(), NumberStyles.Float),
                    LastPrice = decimal.Parse(data[9].ToString(), NumberStyles.Float),
                    Volume = decimal.Parse(data[10].ToString(), NumberStyles.Float),
                    HighPrice = decimal.Parse(data[11].ToString(), NumberStyles.Float),
                    LowPrice = decimal.Parse(data[12].ToString(), NumberStyles.Float),
                    PlaceHolder1 = data[13]?.ToString(),
                    PlaceHolder2 = data[14]?.ToString(),
                    FrrAmountAvailable = decimal.Parse(data[15].ToString(), NumberStyles.Float)
                };
            }
            else
            {
                result = new BitfinexStreamSymbolOverview()
                {
                    BestBidPrice = decimal.Parse(data[0].ToString(), NumberStyles.Float),
                    BestBidQuantity = decimal.Parse(data[1].ToString(), NumberStyles.Float),
                    BestAskPrice = decimal.Parse(data[2].ToString(), NumberStyles.Float),
                    BestAskQuantity = decimal.Parse(data[3].ToString(), NumberStyles.Float),
                    DailyChange = decimal.Parse(data[4].ToString(), NumberStyles.Float),
                    DailyChangePercentage = decimal.Parse(data[5].ToString(), NumberStyles.Float),
                    LastPrice = decimal.Parse(data[6].ToString(), NumberStyles.Float),
                    Volume = decimal.Parse(data[7].ToString(), NumberStyles.Float),
                    HighPrice = decimal.Parse(data[8].ToString(), NumberStyles.Float),
                    LowPrice = decimal.Parse(data[9].ToString(), NumberStyles.Float)
                };
            }

            return result;
        }
    }
}
