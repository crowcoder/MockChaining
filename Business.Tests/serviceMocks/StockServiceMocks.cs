using Moq;
using Service;
using System;

namespace Business.Tests
{
    public static class StockServiceMocks
    {
        /// <summary>
        /// Causes GetCurrentPrice to return a specific price or optionally throws an exception.
        /// </summary>
        /// <param name="mock">Extension object</param>
        /// <param name="currentPrice">The current price you want to simulate receiving from the Stock service.</param>
        /// <param name="ex">Optional exception to cause the GetCurrentPrice method to throw.</param>
        /// <returns></returns>
        public static Mock<IStockService> GetCurrentPrice_Mock(this Mock<IStockService> mock, 
            decimal currentPrice, Exception ex = null)
        {
            if (ex != null)
            {
                mock.Setup(m => m.GetCurrentPrice(It.IsAny<string>())).Throws(ex);
                return mock;
            }

            mock.Setup(m => m.GetCurrentPrice(It.IsAny<string>())).Returns(currentPrice);

            return mock;
        }

        /// <summary>
        /// Simulates throwing an exception when calling GetLastTrade.
        /// </summary>
        /// <param name="mock">Extension</param>
        /// <param name="ex">The exception to throw.</param>
        /// <returns></returns>
        public static Mock<IStockService> GetLastTradeThrowsException_Mock(this Mock<IStockService> mock, Exception ex)
        {
            mock.Setup(m => m.GetLastTrade(It.IsAny<string>())).Throws(ex);
            return mock;
        }

        /// <summary>
        /// Simulates calling GetLastTrade
        /// </summary>
        /// <param name="mock">Extension</param>
        /// <param name="tradeToReturn">The simulated trade to return</param>
        /// <returns></returns>
        public static Mock<IStockService> GetLastTrade_Mock(this Mock<IStockService> mock, Trade tradeToReturn)
        {
            mock.Setup(m => m.GetLastTrade(It.IsAny<string>())).Returns(tradeToReturn);
            return mock;
        }

        /// <summary>
        /// Simulates a Buy and optionally throws exception.
        /// </summary>
        /// <param name="mock">Extension</param>
        /// <param name="tradeToReturn">The simulated Trade returned from Buy()</param>
        /// <param name="ex">Optional exception to throw</param>
        /// <returns></returns>
        public static Mock<IStockService> Buy_Mock(this Mock<IStockService> mock, 
            Trade tradeToReturn, Exception ex = null)
        {
            if (ex != null)
            {
                mock.Setup(m => m.Buy(It.IsAny<string>(), It.IsAny<decimal>())).Throws(ex);
                return mock;
            }

            mock.Setup(m => m.Buy(It.IsAny<string>(), It.IsAny<decimal>())).Returns(tradeToReturn);

            return mock;
        }

        /// <summary>
        /// Simulates buying CEX stock specifically.
        /// </summary>
        /// <param name="mock">Extension</param>
        /// <param name="nbrShares">Number of shares to simulate buying</param>
        /// <param name="tradePrice">Simulated trade price</param>
        /// <returns></returns>
        public static Mock<IStockService> BuySharesOfContrivedExample(this Mock<IStockService> mock, 
            decimal nbrShares, decimal tradePrice)
        {
            mock.Setup(m => m.Buy("CEX", nbrShares)).Returns(new Trade
            {
                Ticker = "CEX",
                Side = "buy",
                TradeDate = DateTimeOffset.UtcNow,
                TradePrice = tradePrice
            });

            return mock;
        }


        /// <summary>
        /// Simulates a Sell
        /// </summary>
        /// <param name="mock">Extension</param>
        /// <param name="tradeToReturn">The simulated trade to return from Sell()</param>
        /// <param name="ex">Optional exception to throw when calling Sell()</param>
        /// <returns></returns>
        public static Mock<IStockService> Sell_Mock(this Mock<IStockService> mock, 
            Trade tradeToReturn, Exception ex = null)
        {
            if (ex != null)
            {
                mock.Setup(m => m.Sell(It.IsAny<string>(), It.IsAny<decimal>())).Throws(ex);
                return mock;
            }

            mock.Setup(m => m.Sell(It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns(tradeToReturn);

            return mock;
        }
    }
}
