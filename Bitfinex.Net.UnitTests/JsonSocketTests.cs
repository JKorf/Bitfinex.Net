using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Bitfinex.Net.UnitTests
{
    internal class JsonSocketTests
    {
        [Test]
        public async Task ValidateTickerUpdateStreamJson()
        {
            await TestFileToObject<BitfinexStreamSymbolOverview>(@"JsonResponses\Spot\Socket\TickerUpdate.txt");
        }

        [Test]
        public async Task ValidateTradeUpdateStreamJson()
        {
            await TestFileToObject<BitfinexTradeSimple>(@"JsonResponses\Spot\Socket\TradeUpdate.txt");
        }

        [Test]
        public async Task ValidateKlineUpdateStreamJson()
        {
            await TestFileToObject<BitfinexKline>(@"JsonResponses\Spot\Socket\KlineUpdate.txt");
        }

        [Test]
        public async Task ValidatUserTradeUpdateStreamJson()
        {
            await TestFileToObject<BitfinexOrder>(@"JsonResponses\Spot\Socket\UserTradeUpdate1.txt");
            await TestFileToObject<BitfinexTradeDetails>(@"JsonResponses\Spot\Socket\UserTradeUpdate2.txt");
            await TestFileToObject<BitfinexPosition>(@"JsonResponses\Spot\Socket\UserTradeUpdate3.txt");
        }

        [Test]
        public async Task ValidatUserBalanceUpdateStreamJson()
        {
            await TestFileToObject<BitfinexWallet>(@"JsonResponses\Spot\Socket\BalanceUpdate.txt");
        }

        [Test]
        public async Task ValidatFundingUpdateStreamJson()
        {
            await TestFileToObject<BitfinexFundingOffer>(@"JsonResponses\Spot\Socket\FundingUpdate1.txt");
            await TestFileToObject<BitfinexFundingCredit>(@"JsonResponses\Spot\Socket\FundingUpdate2.txt");
            await TestFileToObject<BitfinexFunding>(@"JsonResponses\Spot\Socket\FundingUpdate3.txt");
        }

        private static async Task TestFileToObject<T>(string filePath, List<string> ignoreProperties = null)
        {
            var listener = new EnumValueTraceListener();
            Trace.Listeners.Add(listener);
            var path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string json;
            try
            {
                var file = File.OpenRead(Path.Combine(path, filePath));
                using var reader = new StreamReader(file);
                json = await reader.ReadToEndAsync();
            }
            catch (FileNotFoundException)
            {
                throw;
            }

            var result = JsonConvert.DeserializeObject<T>(json);
            JsonToObjectComparer<IBitfinexSocketClient>.ProcessData("", result, json, ignoreProperties: new Dictionary<string, List<string>>
            {
                { "", ignoreProperties ?? new List<string>() }
            });
            Trace.Listeners.Remove(listener);
        }
    }

    internal class EnumValueTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            if (message.Contains("Cannot map"))
                throw new Exception("Enum value error: " + message);
        }

        public override void WriteLine(string message)
        {
            if (message.Contains("Cannot map"))
                throw new Exception("Enum value error: " + message);
        }
    }
}
