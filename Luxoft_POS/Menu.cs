using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Luxoft_POS
{
    public class Menu
    {
        //
        public static void WriteDefaultCurrency(string currency)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["currency"].Value = currency;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);

            Console.WriteLine(currency + " has been stablished as default currency successfully");
            Console.WriteLine("Press 'X' key to continue...");
        }




        public static void WriteMenu(List<Option> options, Option selectedOption)
        {
            Console.Clear();
            Console.WriteLine("Please, select the default currency for the POS: ");

            foreach (Option option in options)
            {
                if (option == selectedOption)
                {
                    Console.Write("=> ");
                }
                else
                {
                    Console.Write(" ");
                }

                Console.WriteLine(option.Name);
            }
        }
        public class Option
        {
            public string Name { get; }
            public Action Selected { get; }

            public Option(string name, Action selected)
            {
                Name = name;
                Selected = selected;
            }
        }
    }
}
