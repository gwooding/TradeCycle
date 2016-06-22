using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeCycle
{
    class Client
    {
        public Rating CreditRating { get; }

        public string Name { get; }

        public Client(string name, Rating rating)
        {
            Name = name;
            CreditRating = rating;
        }
    }
}
