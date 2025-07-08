namespace UOBCMS.Models.IBO
{
    public class InstrumentHolding
    {
        public string ClntCode { get; set; }
        public string Market { get; set; }

        public string AcctType { get; set; }
        public string TradingCcy { get; set; }
        public string Instrument {  get; set; }

        public string BalanceType {  get; set; }
        public decimal AsAt { get; set; }

    }
}
