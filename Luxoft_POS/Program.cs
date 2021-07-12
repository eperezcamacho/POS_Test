using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;

namespace Luxoft_POS
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //Creates a file (located in C folder) that works as Database for currencies and another for currency config
            Currency.CreateCurrencyDB();           

            //Read CurrencyDB file
            List<Currency> currencies = Currency.ReadCurrencyDB();

            //Get Default currency
            string defaultCurrency = Currency.GetDefaultCurrency();

            //Verify is exists a default currency 
            if(String.IsNullOrEmpty(defaultCurrency))
            {
                //Set the default currency on app config
                Currency.SetDefaultCurrency(currencies);
            }

            
            // do while to asking for new calculates
            bool newCalculate = false;
            do
            {
                //Crear console 
                Console.Clear();
                //Ask fot the item price | Conver to decimal
                Console.WriteLine("Enter the item price: ");
                decimal price = Convert.ToDecimal(Console.ReadLine());

                // Ask for the bills/coins amounts in a single String, delete whitespaces, split by comma and we get a list decimal | convert each value to decimal
                Console.WriteLine("Please, enter the bills/coins amounts separate by comma: ");
                List<decimal> paymentAmounts = Console.ReadLine().Trim().Split(",").Select(x => Convert.ToDecimal(x)).ToList();

                decimal TotalPayment = paymentAmounts.Sum(x => x);
                List<decimal> actualCurrency = currencies.Find(x => x.name == defaultCurrency).value;

                if (!Validator.ValidateAmountInput(paymentAmounts, actualCurrency, price))
                {
                    Console.WriteLine("Do you need to capture a new order? (Y/N)");
                    newCalculate = Console.ReadLine().ToLower() == "y" ? true : false;
                    continue;
                }


                List<decimal> correctChange = Currency.calculateChange(price, TotalPayment, actualCurrency);

                Console.WriteLine("Change: ");
                foreach (decimal c in correctChange)
                {
                    Console.WriteLine(c);
                }

                Console.WriteLine("Do you need to capture a new order? (Y/N)");
                newCalculate = Console.ReadLine().ToLower() == "y" ? true : false;
            }
            while (newCalculate);
            Environment.Exit(0);
        }

        
    }
}
