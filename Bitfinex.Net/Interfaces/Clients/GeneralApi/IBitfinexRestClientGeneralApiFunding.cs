using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Interfaces.Clients.GeneralApi
{
    /// <summary>
    /// Bitfinex funding endpoints.
    /// </summary>
    public interface IBitfinexRestClientGeneralApiFunding
    {
        /// <summary>
        /// Get the active funding offers
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-funding-offers" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to return the funding offer for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingOffer[]>> GetActiveFundingOffersAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get the funding offer history
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-funding-offers-hist" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingOffer[]>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Submit a new funding offer.
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-submit-funding-offer" /></para>
        /// </summary>
        /// <param name="fundingOrderType">Order Type (LIMIT, FRRDELTAVAR, FRRDELTAFIX).</param>
        /// <param name="symbol">Symbol for desired pair (fUSD, fBTC, etc..).</param>
        /// <param name="quantity">Quantity (positive for offer, negative for bid).</param>
        /// <param name="rate">Daily rate.</param>
        /// <param name="period">Time period of offer. Minimum 2 days. Maximum 120 days.</param>
        /// <param name="flags">Funding flags</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResultFundingOffer>> SubmitFundingOfferAsync(FundingOrderType fundingOrderType, string symbol, decimal quantity, decimal rate, int period, int? flags = null, CancellationToken ct = default);

        /// <summary>
        /// Return Taken "Used" or "Unused" funding.
        /// <para><a href="https://docs.bitfinex.com/reference/rest-auth-funding-close" /></para>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResult>> CloseFundingAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Cancels an existing Funding Offer based on the offer ID entered.
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-cancel-funding-offer" /></para>
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResultFundingOffer>> CancelFundingOfferAsync(long offerId, CancellationToken ct = default);

        /// <summary>
        /// Cancel all funding offers
        /// <para><a href="https://docs.bitfinex.com/reference/rest-auth-cancel-all-funding-offers" /></para>
        /// </summary>
        /// <param name="asset">Only cancel funding offers in this asset</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResult>> CancelAllFundingOffersAsync(string? asset = null, CancellationToken ct = default);

        /// <summary>
        /// Get the funding loans
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-funding-loans" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the funding loans for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFunding[]>> GetFundingLoansAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get the funding loan history
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-funding-loans-hist" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFunding[]>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the funding credits
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-funding-credits" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the funding credits for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingCredit[]>> GetFundingCreditsAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get the funding credits history
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-funding-credits-hist" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingCredit[]>> GetFundingCreditsHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the funding trades history
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-funding-trades-hist" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingTrade[]>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get funding info for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-info-funding" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Activate or deactivate auto-renew. Allows you to specify the currency, amount, rate, and period.
        /// <para><a href="https://docs.bitfinex.com/reference/rest-auth-funding-auto-renew" /></para>
        /// </summary>
        /// <param name="asset">Currency for which to enable auto-renew</param>
        /// <param name="status">1 to activate, 0 to deactivate.</param>
        /// <param name="quantity">Amount to be auto-renewed (Minimum 50 USD equivalent).</param>
        /// <param name="rate">Percentage rate at which to auto-renew. (rate == 0 to renew at FRR).</param>
        /// <param name="period">Period in days.</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResultFundingAutoRenew>> SubmitFundingAutoRenewAsync(string asset, bool status, decimal? quantity = null, decimal? rate = null, int? period = null, CancellationToken ct = default);

        /// <summary>
        /// Toggle to keep funding taken. Specify loan for unused funding and credit for used funding.
        /// <para><a href="https://docs.bitfinex.com/reference/rest-auth-keep-funding" /></para>
        /// </summary>
        /// <param name="type">Funding type</param>
        /// <param name="ids">Ids</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResult>> KeepFundingAsync(FundType type, IEnumerable<long>? ids = null, CancellationToken ct = default);

        /// <summary>
        /// Status of auto-renew.
        /// </summary>
        /// <param name="asset">Currency for which to enable auto-renew</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingAutoRenewStatus>> GetFundingAutoRenewStatusAsync(string asset, CancellationToken ct = default);
    }
}
