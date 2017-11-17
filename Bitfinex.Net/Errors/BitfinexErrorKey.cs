namespace Bitfinex.Net.Errors
{
    public enum BitfinexErrorKey
    {
        NoApiCredentialsProvided,
        InputValidationFailed,

        ParseErrorReader,
        ParseErrorSerialization,

        ErrorWeb,
        CantConnectToServer,
        WithdrawFailed,
        DepositAddressFailed,

        SubscriptionNotConfirmed,

        UnknownError
    }
}
