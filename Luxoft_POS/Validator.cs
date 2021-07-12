using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luxoft_POS
{
    public class Validator
    {
        public static Boolean ValidateAmountInput(List<decimal> amounts, List<decimal> currencies, decimal price)
        {
            if(amounts.Sum(x => x) < price)
            {
                Console.WriteLine("We're sorry but your payment is lower than the item price.");
                return false;
            }
            foreach(decimal a in amounts)
            {
                if(!currencies.Contains(a))
                {
                    Console.WriteLine("We're sorry but your bill/coin {0} is not a validate currency", a);
                    return false;
                }
            }

            return true;
        }
    }
}
