using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using static Luxoft_POS.Menu;

namespace Luxoft_POS
{
    public class Currency
    {
        public string name { get; set; }
        public List<decimal> value { get; set; }

        public static List<Option> options = new List<Option>();

        public static void CreateCurrencyDB()
        {
            try
            {
                //Create base folder
                string folder = @"c:\Luxoft_POS";
                System.IO.Directory.CreateDirectory(folder);
                //Create txt file that works as database
                string fileName = "CurrencyDB.txt";
                folder = System.IO.Path.Combine(folder, fileName);

                //Check if the database exists
                if (!System.IO.File.Exists(folder))
                {
                    //initialize and Insert default values
                    using (StreamWriter sw = System.IO.File.CreateText(folder))
                    {
                        sw.WriteLine("name;value;");
                        sw.WriteLine("mxn;0.05, 0.10, 0.20, 0.50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00;");
                        sw.WriteLine("usd;0.01, 0.05, 0.10, 0.25, 0.50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00;");
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<Currency> ReadCurrencyDB()
        {
            List<Currency> allCurrencies = new List<Currency>();

            string path = ConfigurationManager.AppSettings["currencyDB"];
            List<string> productlines = File.ReadAllLines(path).ToList();

            //Remove headers
            productlines.RemoveAt(0);

            foreach (string line in productlines)
            {
                string[] parts = line.Split(';');

                Currency currency = new Currency();
                currency.name = parts[0];
                currency.value = parts[1].Split(",").ToList().Select(x => decimal.Parse(x)).ToList();

                allCurrencies.Add(currency);
            }

            return allCurrencies;
        }

        public static string GetDefaultCurrency()
        {
            //Set the default currency value on app config
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return config.AppSettings.Settings["currency"].Value;
        }

        public static void SetDefaultCurrency(List<Currency> currencies)
        {
            //
            // Create options that you want your menu to have
            foreach (Currency currency in currencies)
            {
                options.Add(new Option(currency.name, () => WriteDefaultCurrency(currency.name)));
            }

            // Set the default index of the selected item to be the first
            int index = 0;

            // Write the menu out
            WriteMenu(options, options[index]);

            // Store key info in here
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[index].Selected.Invoke();
                    index = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

        }

        public static List<decimal> calculateChange(decimal price, decimal total, List<decimal> currency)
        {
            //order currencies -> max to min
            currency.Reverse();
            //calculate change amount
            decimal change = total - price;

            List<decimal> changeAmounts = new List<decimal>();

            //if change > 0 repeat operations
            while (change > 0)
            {
                //iterate each currency 
                foreach (decimal c in currency)
                {
                    //if the actual currency is minor or equals to the change amount...(check step1 and step2)                    
                    if (c <= change)
                    {
                        //step1: calculate how many times we can use a bill/coin amount
                        int freq = Convert.ToInt32(Math.Floor(change / c));
                        //step2: calculate change after discounting the current currency
                        change = change % c;
                        //add currency to the change list
                        for (int i = 0; i < freq; i++)
                        {
                            changeAmounts.Add(c);
                        }
                        //break and looking for a new currency
                        break;
                    }
                }

                //if the change amount is minor to the whole currencies amount list, break execution
                if (change < currency.LastOrDefault())
                    break;
            }

            return changeAmounts;
        }
    }
}
