// 02-margin-funding.cs
//
// Demonstrates: margin positions, public funding market data and authenticated funding offers.
//
// Setup: dotnet add package JKorf.Bitfinex.Net

using Bitfinex.Net;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;

var client = new BitfinexRestClient(options =>
{
    options.ApiCredentials = new BitfinexCredentials("API_KEY", "API_SECRET");
});

// ---- 1. MARGIN POSITIONS LIVE UNDER SPOT TRADING ----
var positions = await client.SpotApi.Trading.GetPositionsAsync();
if (!positions.Success)
{
    Console.WriteLine($"Positions failed: {positions.Error}");
}
else
{
    foreach (var position in positions.Data)
    {
        Console.WriteLine($"{position.Symbol}: quantity={position.Quantity}, pnl={position.ProfitLoss}, leverage={position.Leverage}");
    }
}

// ---- 2. PUBLIC FUNDING MARKET DATA IS UNDER SPOT EXCHANGE DATA ----
const string fundingSymbol = "fUSD";

var fundingTicker = await client.SpotApi.ExchangeData.GetFundingTickerAsync(fundingSymbol);
if (!fundingTicker.Success)
{
    Console.WriteLine($"Funding ticker failed: {fundingTicker.Error}");
}
else
{
    Console.WriteLine($"{fundingSymbol} funding last rate: {fundingTicker.Data.LastPrice}");
}

// ---- 3. AUTHENTICATED FUNDING MANAGEMENT IS UNDER GENERAL API FUNDING ----
var activeOffers = await client.GeneralApi.Funding.GetActiveFundingOffersAsync(fundingSymbol);
if (!activeOffers.Success)
{
    Console.WriteLine($"Active funding offers failed: {activeOffers.Error}");
}
else
{
    Console.WriteLine($"Active funding offers: {activeOffers.Data.Count()}");
}

var offer = await client.GeneralApi.Funding.SubmitFundingOfferAsync(
    fundingOrderType: FundingOrderType.Limit,
    symbol: fundingSymbol,
    quantity: 100m,
    rate: 0.0002m,
    period: 2);

Console.WriteLine(offer.Success
    ? $"Funding offer submitted: {offer.Data.Data.Id}"
    : $"Funding offer rejected: {offer.Error}");
