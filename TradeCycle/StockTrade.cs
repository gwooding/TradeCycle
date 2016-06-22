using System;

namespace TradeCycle
{
    class StockTrade : Trade
    {
        public StockTrade(
            string name, 
            DateTimeOffset tradeDate, 
            DateTimeOffset valueDate, 
            int quantity, 
            Client client)
        :base(name, tradeDate, valueDate, quantity, client)
        { }

        public override void SetCounterPartyExpectedLoss(double multiplier, double tradeValue)
        {
            CounterPartyExpectedLoss = multiplier * tradeValue;
        }

        public override double GetCurrentAssetPrice()
        {
            Random randObj = new Random();
            double price = randObj.NextDouble();

            return price;
        }

        public override double GetTradeValue(double assetPrice)
        {
            return Quantity * assetPrice;
        }
    }
}
