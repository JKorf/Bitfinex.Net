# Operational considerations

## Credentials

Use placeholders in examples and retrieve real API keys and HMAC secrets from environment variables or secret storage. Limit permissions to the requested workflow: account reads, trading, funding management, and withdrawals are separate capabilities. Avoid logging signed requests or credentials.

## Trading and wallet selection

`OrderType.ExchangeLimit` and `ExchangeMarket` use exchange-wallet trading; `Limit` and `Market` are margin order types. Changing the type is not merely changing execution style. Derivatives use native contract symbols and order leverage through `ExchangeApi`; do not substitute another exchange's futures position-mode parameters.

Check symbol constraints and wallet availability before placing an order. Prices and quantities in examples are illustrative. Limit orders can fill immediately, and cancellation is not a guarantee against execution. Keep the exchange notification and its nested entity distinct when reading write results; inspect nested outcomes for multi-order responses.

## Funding offers

A submitted funding offer can match and commit funds before cancellation. Funding offers, loans, and credits are different states; cancelling an offer does not necessarily unwind an existing loan or credit. Inspect the returned state before choosing a follow-up operation.

## Retry decisions

Use `Error.IsTransient` to identify retry candidates, with bounded backoff and cancellation. An order or funding-offer timeout can leave the outcome unknown. Reconcile the relevant open/history state before resubmitting; a transient error alone does not make an account-changing request safe to repeat.

See [API surfaces](api-surfaces.md) for lookup methods and [usage](usage.md) for result handling.
