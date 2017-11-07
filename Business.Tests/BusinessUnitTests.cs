using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service;
using System;

namespace Business.Tests
{
    [TestClass]
    public class BusinessUnitTests
    {
        /// <summary>
        /// Validate that trade will be a sell if the 
        /// last trade was a buy and the current price is
        /// at least 15% higher than the buy.
        /// </summary>
        [TestMethod]
        public void SellWhenLastTradeWasBuy_Test()
        {
            #region setup the stock service
            decimal simulatedCurrentPrice = 11.50m;

            Trade simulatedLastTrade = new Trade
            {
                Ticker = "ABC",
                Side = "buy",
                TradeDate = DateTimeOffset.Now.AddHours(-4),
                TradePrice = 10.00m
            };

            Trade simulatedSell = new Trade
            {
                Ticker = "ABC",
                Side = "sell",
                TradeDate = DateTimeOffset.Now,
                TradePrice = 11.50m
            };

            //Here we simulate the current price being exactly 15% higher
            //than our last stock trade, and that the trade was a buy.
            //This should exercise the logic of a sell.
            var stockServiceMock = new Mock<IStockService>()
                .GetCurrentPrice_Mock(simulatedCurrentPrice)
                .GetLastTrade_Mock(simulatedLastTrade)
                .Sell_Mock(simulatedSell);
            #endregion

            #region setup the log service
            var logServiceMock = new Mock<ILogService>().Log_Mock("Should not get called");
            #endregion

            StocksLogic stocksLogic = new StocksLogic(stockServiceMock.Object, logServiceMock.Object);

            Trade theSell = stocksLogic.GetRich("ABC");

            Assert.IsNotNull(theSell);
            Assert.AreEqual("sell", theSell.Side);
            Assert.AreEqual(theSell.TradeDate, simulatedSell.TradeDate);

            stockServiceMock.Verify(m => m.Sell(It.IsAny<string>(), It.IsAny<decimal>()), Times.Once);

            //This should be a sell so validate the Buy() method is never called
            stockServiceMock.Verify(m => m.Buy(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);

            //Only exception will log so validate log was not called
            logServiceMock.Verify(m => m.Log(It.IsAny<string>()), Times.Never);

        }

        [TestMethod]
        public void BuyWhenLastTradeWasSell_Test()
        {
            #region setup the stock service
            decimal simulatedCurrentPrice = 8.03m;

            Trade simulatedLastTrade = new Trade
            {
                Ticker = "ABC",
                Side = "sell",
                TradeDate = DateTimeOffset.Now.AddHours(-1),
                TradePrice = 10.00m
            };

            Trade simulatedBuy = new Trade
            {
                Ticker = "ABC",
                Side = "buy",
                TradeDate = DateTimeOffset.Now,
                TradePrice = 11.50m
            };

            //Here we simulate the current price being lower than 15% less 
            //than our last sell.
            //This should exercise the logic of a sell only.
            var stockServiceMock = new Mock<IStockService>()
                .GetCurrentPrice_Mock(simulatedCurrentPrice)
                .GetLastTrade_Mock(simulatedLastTrade)
                .Buy_Mock(simulatedBuy);
            #endregion

            #region setup the log service
            var logServiceMock = new Mock<ILogService>().Log_Mock("Should not get called");
            #endregion


            StocksLogic stocksLogic = new StocksLogic(stockServiceMock.Object, logServiceMock.Object);

            Trade theBuy = stocksLogic.GetRich("ABC");

            Assert.IsNotNull(theBuy);
            Assert.AreEqual("buy", theBuy.Side);
            Assert.AreEqual(theBuy.TradeDate, simulatedBuy.TradeDate, "Simulated buy date equals buy date");

            stockServiceMock.Verify(m => m.Buy(It.IsAny<string>(), It.IsAny<decimal>()), Times.Once);

            stockServiceMock.Verify(m => m.Sell(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);

            //Only exception will log so validate log was not called
            logServiceMock.Verify(m => m.Log(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void GetCurrentPriceThrowsException_Test()
        {
            var stockServiceMock = new Mock<IStockService>()
              .GetCurrentPrice_Mock(0, new Exception("Unable to get current price"));

            #region setup the log service
            var logServiceMock = new Mock<ILogService>().Log_Mock("Should not get called");
            #endregion

            try
            {
                StocksLogic stocksLogic = new StocksLogic(stockServiceMock.Object, logServiceMock.Object);
            }
            catch (Exception ex)
            {
                logServiceMock.Verify(m => m.Log(It.IsAny<string>()), Times.Once);
                Assert.AreEqual("Unable to get current price", ex.Message);
            }
        }

        /// <summary>
        /// The last trade was a buy but the current price is not 15% higher
        /// so no trade will be made. The "side" of the trade returned by
        /// GetRich() should be none, and the Buy and Sell methods should
        /// never be called.
        /// </summary>
        [TestMethod]
        public void NeitherSellNorBuyTradeIsMade_Test()
        {
            #region setup the stock service
            decimal simulatedCurrentPrice = 10.00m;

            Trade simulatedLastTrade = new Trade
            {
                Ticker = "ABC",
                Side = "buy",
                TradeDate = DateTimeOffset.Now.AddHours(-4),
                TradePrice = 9.00m
            };
                        
            var stockServiceMock = new Mock<IStockService>()
                .GetCurrentPrice_Mock(simulatedCurrentPrice)
                .GetLastTrade_Mock(simulatedLastTrade);
            #endregion

            #region setup the log service
            var logServiceMock = new Mock<ILogService>().Log_Mock("Should not get called");
            #endregion

            StocksLogic stocksLogic = new StocksLogic(stockServiceMock.Object, logServiceMock.Object);

            Trade theSell = stocksLogic.GetRich("ABC");

            Assert.IsNotNull(theSell);
            Assert.AreEqual("none", theSell.Side);
            Assert.IsNull(theSell.TradeDate);

            stockServiceMock.Verify(m => m.Buy(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
            stockServiceMock.Verify(m => m.Sell(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
            logServiceMock.Verify(m => m.Log(It.IsAny<string>()), Times.Never);
        }
    }
}
