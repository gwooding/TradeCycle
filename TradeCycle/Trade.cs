using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeCycle
{
    abstract class Trade
    {
        public DateTimeOffset TradeDate { get; private set; }

        public DateTimeOffset MaturityDate { get; set; }

        public DateTimeOffset FinalDeliveryDate { get; set; }

        public DateTimeOffset ValueDate { get; private set; }

        public DateTimeOffset SettlementDate { get; set; }

        public bool Settled { get; set; }

        public double ExecutionPrice { get; set; }

        public string Name { get; }

        public int Quantity { get; private set; }

        public double CounterPartyExpectedLoss { get; protected set; }

        public Client Client { get; }

        public Trade(string name, DateTimeOffset tradeDate, DateTimeOffset valueDate, int quantity, Client client)
        {
            Name = name;
            TradeDate = tradeDate;
            ValueDate = valueDate;
            Quantity = quantity;
            Client = client;
        }

        public abstract void SetCounterPartyExpectedLoss(double multiplier, double tradeValue);

        public abstract double GetCurrentAssetPrice();

        public abstract double GetTradeValue(double price);
    }
}
