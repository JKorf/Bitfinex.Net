namespace Bitfinex.Net.Objects
{
    public class BitfinexApiKeyPermissions
    {
        public BitfinexReadWritePermission Account { get; set; }
        public BitfinexReadWritePermission History { get; set; }
        public BitfinexReadWritePermission Orders { get; set; }
        public BitfinexReadWritePermission Positions { get; set; }
        public BitfinexReadWritePermission Funding { get; set; }
        public BitfinexReadWritePermission Wallets { get; set; }
        public BitfinexReadWritePermission Withdraw { get; set; }
    }

    public class BitfinexReadWritePermission
    {
        public bool Read { get; set; }
        public bool Write { get; set; }
    }
}
