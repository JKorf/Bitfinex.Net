using CryptoExchange.Net.Objects.Errors;

namespace Bitfinex.Net
{
    internal static class BitfinexErrors
    {
        public static ErrorMapping Errors { get; } = new ErrorMapping(
            [
                new ErrorInfo(ErrorType.Unauthorized, false, "Invalid API key", "10100"),
                new ErrorInfo(ErrorType.UnknownSymbol, false, "Unknown symbol", "10020", "10300"),

                new ErrorInfo(ErrorType.InvalidTimestamp, false, "Invalid nonce", "10114"),
            ]
        ,
            [

            new ErrorEvaluator("10001", (code, msg) => {

                if (string.IsNullOrEmpty(msg))
                    return ErrorInfo.Unknown;

                if (msg!.Equals("price: invalid"))
                    return new ErrorInfo(ErrorType.InvalidPrice, false, "Invalid price", code);

                if (msg!.Equals("amount: invalid"))
                    return new ErrorInfo(ErrorType.InvalidQuantity, false, "Quantity price", code);

                if (msg!.Equals("Order not found."))
                    return new ErrorInfo(ErrorType.UnknownOrder, false, "Unknown order", code);

                if (msg!.StartsWith("Invalid order: not enough tradable balance for")
                || msg!.StartsWith("Invalid order: not enough exchange balance for"))
                {
                    return new ErrorInfo(ErrorType.InsufficientBalance, false, "Insufficient balance", code);
                }

                return ErrorInfo.Unknown with { Message = msg };
            })

            ]);
    }
}
