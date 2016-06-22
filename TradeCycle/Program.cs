using System;
using System.Collections.Generic;
using System.Threading;

namespace TradeCycle
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = UserCreatesClient();

            Console.WriteLine("Please enter the asset name you wish to make an order for");
            string name = Console.ReadLine();

            Console.WriteLine("Please enter the asset type \nFor a stock please enter 'Stock'");
            string assetType = Console.ReadLine();

            Console.WriteLine("Please enter the quantity you wish to buy");
            int quantity = ConvertUserInput();

            //TODO: put in separate YAML config file
            var ratingsMultipliersForCounterpartyRiskCalculations = new Dictionary<Rating, double>()
            {
                { Rating.Aaa, 0.1 },
                { Rating.Aa, 0.2 },
                { Rating.A, 0.3 },
                { Rating.Baa, 0.4 },
            };

            if (assetType.Equals("Stock"))
            {
                ExecuteStockTradeSimulation(client, name, quantity, ratingsMultipliersForCounterpartyRiskCalculations);
            }
            else
            {
                Console.WriteLine("{0} is not an asset that has been implemented yet", assetType);
            }
        }

        private static Client UserCreatesClient()
        {
            Console.WriteLine("Please enter the name of client");
            string clientName = Console.ReadLine();

            Console.WriteLine("Please enter their credit rating from the list of options:");
            Console.WriteLine("Aaa, Aa, A, Baa");

            string creditRating = Console.ReadLine();

            Rating rating = ConvertCreditRating(creditRating);

            return new Client(clientName, rating);
        }

        private static Rating ConvertCreditRating(string creditRating)
        {
            Rating rating = Rating.Baa;
            if (!Enum.TryParse(creditRating, out rating))
            {
                throw new Exception("Invalid Credit Rating");
            }

            return rating;
        }

        private static int ConvertUserInput()
        {
            string quantityString = Console.ReadLine();

            int quantity = 0;
            if (!int.TryParse(quantityString, out quantity))
            {
                throw new Exception("Invalid number entered for quantity");
            }

            return quantity;
        }

        private static void ExecuteStockTradeSimulation(Client client, string name, int quantity, Dictionary<Rating, double> ratingsMultipliers)
        {
            Trade trade = ReceiveTradeRequest(client, name, quantity);

            Thread.Sleep(3000);

            ExecuteTrade(client, ratingsMultipliers, trade);

            Thread.Sleep(2000);

            ClientSettlesTrade(client, ratingsMultipliers, trade);

            Thread.Sleep(3000);

            CustodianTransfersOwnershipOfShares(trade);

            Console.ReadLine();
        }

        private static Trade ReceiveTradeRequest(Client client, string name, int quantity)
        {
            DateTimeOffset tradeDate = DateTimeOffset.Now;
            DateTimeOffset valueDate = tradeDate.AddSeconds(4);

            Trade trade = new StockTrade(name, tradeDate, valueDate, quantity, client);

            Console.WriteLine(trade.TradeDate.ToString() + ":");
            Console.WriteLine("Order received for {0} shares in {1}", trade.Quantity, trade.Name);

            return trade;
        }

        private static void ExecuteTrade(Client client, Dictionary<Rating, double> ratingsMultipliers, Trade trade)
        {
            trade.MaturityDate = DateTimeOffset.Now;
            trade.ExecutionPrice = trade.GetCurrentAssetPrice();
            trade.SetCounterPartyExpectedLoss(ratingsMultipliers[client.CreditRating], trade.GetTradeValue(trade.ExecutionPrice));

            Console.WriteLine(trade.MaturityDate.ToString() + ":");
            Console.WriteLine("Trade executed at price {0:c}, trade value at maturity date is {1:c}", trade.ExecutionPrice, trade.Quantity * trade.ExecutionPrice);
            Console.WriteLine("Expected loss due to counterparty risk is {0:c}", trade.CounterPartyExpectedLoss);
        }

        private static void ClientSettlesTrade(Client client, Dictionary<Rating, double> ratingsMultipliers, Trade trade)
        {
            trade.SettlementDate = DateTimeOffset.Now;
            trade.Settled = true;
            trade.SetCounterPartyExpectedLoss(ratingsMultipliers[client.CreditRating], 0);

            Console.WriteLine(trade.SettlementDate.ToString() + ":");
            Console.WriteLine("Trade settled");
            Console.WriteLine("Counterparty risk is now {0:c}", trade.CounterPartyExpectedLoss);

            if (trade.SettlementDate > trade.ValueDate)
            {
                Console.WriteLine("{0} was late with settlement", trade.Client.Name);
            }
        }

        private static void CustodianTransfersOwnershipOfShares(Trade trade)
        {
            trade.FinalDeliveryDate = DateTimeOffset.Now;

            Console.WriteLine(trade.FinalDeliveryDate.ToString() + ":");
            Console.WriteLine("Assets transferred to {0}", trade.Client.Name);
        }
    }
}
