namespace Service
{
    public interface IStockService
    {
        Trade Buy(string ticker, decimal nbrShares);
        decimal GetCurrentPrice(string ticker);
        Trade GetLastTrade(string ticker);
        Trade Sell(string ticker, decimal nbrShares);
    }
}