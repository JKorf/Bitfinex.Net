using Bitfinex.Net.Interfaces.Clients;
using CryptoExchange.Net.Authentication;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the Bitfinex services
builder.Services.AddBitfinex();

// OR to provide API credentials for accessing private endpoints, or setting other options:
/*
builder.Services.AddBitfinex(options =>
{    
   options.ApiCredentials = new ApiCredentials("<APIKEY>", "<APISECRET>");
   options.Rest.RequestTimeout = TimeSpan.FromSeconds(5);
});
*/

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// Map the endpoints and inject the Bitfinex rest client
app.MapGet("/{Symbol}", async ([FromServices] IBitfinexRestClient client, string symbol) =>
{
    var result = await client.SpotApi.ExchangeData.GetTickersAsync(new[] { symbol });
    return (object)(result.Success ? result.Data.Single() : result.Error!);
})
.WithOpenApi();

app.MapGet("/Balances", async ([FromServices] IBitfinexRestClient client) =>
{
    var result = await client.SpotApi.Account.GetBalancesAsync();
    return (object)(result.Success ? result.Data : result.Error!);
})
.WithOpenApi();

app.Run();