using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class StocksLogic
    {
        private readonly IStockService _stockService;
        private readonly ILogService _logService;

        public StocksLogic(IStockService stockService, ILogService logService)
        {
            _stockService = stockService;
            _logService = logService;
        }

        public Trade GetRich(string ticker)
        {
            try
            {
                Trade lastTrade = _stockService.GetLastTrade(ticker);
                decimal currentPrice = _stockService.GetCurrentPrice(ticker);

                if (lastTrade.Side == "sell")
                {
                    //Buy 200 shares if current price is at least 15% lower than last sell.
                    if (currentPrice <= (lastTrade.TradePrice - (lastTrade.TradePrice * .15m)))
                    {
                        return _stockService.Buy(ticker, 200);
                    }
                }
                else //was a buy
                {
                    //Sell 200 shares if current price is 15% higher than last buy.
                    if (currentPrice >= (lastTrade.TradePrice + (lastTrade.TradePrice * .15m)))
                    {
                        return _stockService.Sell(ticker, 200);
                    }
                }

                //no trade criteria met, return no trade.
                return new Trade { Ticker = ticker, Side = "none" };
            }
            catch (Exception ex)
            {
                _logService.Log(ex.Message);
                return new Trade { Ticker = ticker, Side = "none" };
            }
        }
    }
}
