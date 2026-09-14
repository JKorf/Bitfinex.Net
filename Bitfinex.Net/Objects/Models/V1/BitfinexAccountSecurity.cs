using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// Account security settings, including the whitelisted withdrawal addresses
    /// </summary>
    [SerializationModel]
    public record BitfinexAccountSecurity
    {
        /// <summary>
        /// The status of the request
        /// </summary>
        [JsonIgnore]
        public bool Success => Status == "success";
        /// <summary>
        /// ["<c>status</c>"] Status string
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>whitelisted_addresses</c>"] Whitelisted withdrawal addresses, keyed by currency. Note that
        /// the key is a Bitfinex currency code which also implies a network, for example UDC for USD Coin,
        /// USE for Tether on Ethereum and USX for Tether on Tron; see <see cref="TetherProtocols"/>.
        /// Currencies with whitelisting disabled are returned with a placeholder entry rather than an empty
        /// list, for example a string beginning "disabled", or "Disabled (System)".
        /// </summary>
        [JsonPropertyName("whitelisted_addresses")]
        public Dictionary<string, string[]> WhitelistedAddresses { get; set; } = new Dictionary<string, string[]>();
        /// <summary>
        /// ["<c>tether_protocols</c>"] The Tether currency codes, keyed by code, describing which asset and
        /// network each one refers to
        /// </summary>
        [JsonPropertyName("tether_protocols")]
        public Dictionary<string, BitfinexTetherProtocol> TetherProtocols { get; set; } = new Dictionary<string, BitfinexTetherProtocol>();
        /// <summary>
        /// ["<c>any_addresses_locked</c>"] Whether any withdrawal addresses are locked
        /// </summary>
        [JsonPropertyName("any_addresses_locked")]
        public bool AnyAddressesLocked { get; set; }
        /// <summary>
        /// ["<c>pending_otp</c>"] Whether a one time password is pending
        /// </summary>
        [JsonPropertyName("pending_otp")]
        public bool PendingOtp { get; set; }
    }

    /// <summary>
    /// A Tether currency code, and the asset and network it refers to
    /// </summary>
    [SerializationModel]
    public record BitfinexTetherProtocol
    {
        /// <summary>
        /// ["<c>method</c>"] The withdrawal method for this code
        /// </summary>
        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>description</c>"] Description of the asset and network, for example "Tether(USD) on Ethereum"
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>ccy</c>"] The currency code
        /// </summary>
        [JsonPropertyName("ccy")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>transport_ccy</c>"] The currency code of the network the asset is transported on
        /// </summary>
        [JsonPropertyName("transport_ccy")]
        public string TransportAsset { get; set; } = string.Empty;
    }
}