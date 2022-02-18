using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
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
    public interface IBitfinexClientGeneralApiFunding
    {
        /// <summary>
        /// Get the active funding offers
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-funding-offers" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to return the funding offer for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexFundingOffer>>> GetActiveFundingOffersAsync(string symbol, CancellationToken ct = default);

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
        Task<WebCallResult<IEnumerable<BitfinexFundingOffer>>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Submit a new funding offer.
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-submit-funding-offer" /></para>
        /// </summary>
        /// <param name="fundingOrderType">Order Type (LIMIT, FRRDELTAVAR, FRRDELTAFIX).</param>
        /// <param name="symbol">Symbol for desired pair (fUSD, fBTC, etc..).</param>
        /// <param name="quantity">Quantity (positive for offer, negative for bid).</param>
        /// <param name="rate">Daily rate.</param>
        /// <param name="period">Time period of offer. Minimum 2 days. Maximum 120 days.</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResult<BitfinexFundingOffer>>> SubmitFundingOfferAsync(FundingOrderType fundingOrderType, string symbol, decimal quantity, decimal rate, int period, CancellationToken ct = default);

        /// <summary>
        /// Cancels an existing Funding Offer based on the offer ID entered.
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-cancel-funding-offer" /></para>
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResult<BitfinexFundingOffer>>> CancelFundingOfferAsync(long offerId, CancellationToken ct = default);

        /// <summary>
        /// Get the funding loans
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-funding-loans" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the funding loans for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexFunding>>> GetFundingLoansAsync(string symbol, CancellationToken ct = default);

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
        Task<WebCallResult<IEnumerable<BitfinexFunding>>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the funding credits
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-funding-credits" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the funding credits for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexFundingCredit>>> GetFundingCreditsAsync(string symbol, CancellationToken ct = default);

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
        Task<WebCallResult<IEnumerable<BitfinexFundingCredit>>> GetFundingCreditsHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

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
        Task<WebCallResult<IEnumerable<BitfinexFundingTrade>>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Submit a new offer
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-new-offer" /></para>
        /// </summary>
        /// <param name="asset">The asset</param>
        /// <param name="quantity">The quantity</param>
        /// <param name="price">Rate to lend or borrow at in percent per 365 days (0 for FRR)</param>
        /// <param name="period">Number of days</param>
        /// <param name="direction">Direction of the offer</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOffer>> NewOfferAsync(string asset, decimal quantity, decimal price, int period, FundingType direction, CancellationToken ct = default);

        /// <summary>
        /// Cancel an offer
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-cancel-offer" /></para>
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOffer>> CancelOfferAsync(long offerId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an offer
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-offer-status" /></para>
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOffer>> GetOfferAsync(long offerId, CancellationToken ct = default);

        /// <summary>
        /// Close margin funding
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-close-margin-funding" /></para>
        /// </summary>
        /// <param name="swapId">The id to close</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingContract>> CloseMarginFundingAsync(long swapId, CancellationToken ct = default);

        /// <summary>
        /// Get funding info for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-info-funding" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol, CancellationToken ct = default);
    }
}
