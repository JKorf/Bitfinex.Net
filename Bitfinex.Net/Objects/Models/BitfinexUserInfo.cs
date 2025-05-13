using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using System;
using System.Collections.Generic;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// User info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexUserInfo>))]
    [SerializationModel]
    public record BitfinexUserInfo
    {
        /// <summary>
        /// The id of the user
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }
        /// <summary>
        /// The email address of the user
        /// </summary>
        [ArrayProperty(1)]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// The username of the user
        /// </summary>
        [ArrayProperty(2)]
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// The time the account was created
        /// </summary>
        [ArrayProperty(3), JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Indicates if the user has a verified status 
        /// </summary>
        [ArrayProperty(4)]
        public bool Verified { get; set; }
        /// <summary>
        /// Account verification level
        /// </summary>
        [ArrayProperty(5)]
        public int VerificationLevel { get; set; }
        /// <summary>
        /// Time zone
        /// </summary>
        [ArrayProperty(7)]
        public string TimeZone { get; set; } = string.Empty;
        /// <summary>
        /// Account locale setting
        /// </summary>
        [ArrayProperty(8)]
        public string Locale { get; set; } = string.Empty;
        /// <summary>
        /// Shows where the account is registered. Accounts registered at Bitfinex will show 'bitfinex' and accounts registered at eosfinex will show 'eosfinex'
        /// </summary>
        [ArrayProperty(9)]
        public string Company { get; set; } = string.Empty;
        /// <summary>
        /// Indicates if the user has a verified status 
        /// </summary>
        [ArrayProperty(10)]
        public bool EmailVerified { get; set; }
        /// <summary>
        /// The time the master account was created
        /// </summary>
        [ArrayProperty(14), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? MasterAccountCreateTime { get; set; }
        /// <summary>
        /// Account group id
        /// </summary>
        [ArrayProperty(15)]
        public long GroupId { get; set; }
        /// <summary>
        /// Master account id
        /// </summary>
        [ArrayProperty(16)]
        public long MasterAccountId { get; set; }
        /// <summary>
        /// True if account inherits verification from master account
        /// </summary>
        [ArrayProperty(17)]
        public bool InheritMasterAccountVerification { get; set; }
        /// <summary>
        /// Indicates if the user is group master
        /// </summary>
        [ArrayProperty(18)]
        public bool IsGroupMaster { get; set; }
        /// <summary>
        /// Whether group withdraw is enabled 
        /// </summary>
        [ArrayProperty(19)]
        public bool GroupWithdrawEnabled { get; set; }
        /// <summary>
        /// Whether paper trading is enabled
        /// </summary>
        [ArrayProperty(21)]
        public bool PaperTradingEnabled { get; set; }
        /// <summary>
        /// Whether merchant is enabled
        /// </summary>
        [ArrayProperty(22)]
        public bool MerchantEnabled { get; set; }
        /// <summary>
        /// Whether competition is enabled
        /// </summary>
        [ArrayProperty(23)]
        public bool CompetitionEnabled { get; set; }
        /// <summary>
        /// Enabled 2fa methods
        /// </summary>
        [ArrayProperty(26), JsonConversion]
        public string[]? TwoFactorAuthenticationMethods { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Whether the account has a securities sub-account
        /// </summary>
        [ArrayProperty(28)]
        public bool IsSecuritiesMaster { get; set; }
        /// <summary>
        /// Whether the account has securities enabled
        /// </summary>
        [ArrayProperty(29)]
        public bool SecuritiesEnabled { get; set; }
        /// <summary>
        /// Account can disable context switching by master account into this account
        /// </summary>
        [ArrayProperty(38)]
        public bool AllowDisableContextSwitch { get; set; }
        /// <summary>
        /// Master account cannot context switch into this account
        /// </summary>
        [ArrayProperty(39)]
        public bool ContextSwitchDisabled { get; set; }
        /// <summary>
        /// The time of last login
        /// </summary>
        [ArrayProperty(44), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// Highest account verification level submitted
        /// </summary>
        [ArrayProperty(47)]
        public int VerificationLevelSubmitted { get; set; }
        /// <summary>
        /// Array of country codes based on your verification data (residence and nationality)
        /// </summary>
        [ArrayProperty(49), JsonConversion]
        public string[]? ComplCountries { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Array of country codes based on your verification data(residence only)
        /// </summary>
        [ArrayProperty(50), JsonConversion]
        public string[]? ComplCountriesResidence { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Type of verification ("individual" or "corporate")
        /// </summary>
        [ArrayProperty(51)]
        public string AccountType { get; set; } = string.Empty;
        /// <summary>
        /// Whether account is enterprise merchant
        /// </summary>
        [ArrayProperty(54)]
        public bool IsMerchantEnterprice { get; set; }
    }
}
