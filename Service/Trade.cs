using System;

namespace Service
{
    public class Trade
    {
        public string Ticker { get; set; }
        public decimal? TradePrice { get; set; }
        public DateTimeOffset? TradeDate { get; set; }
        public string Side { get; set; }
    }
}