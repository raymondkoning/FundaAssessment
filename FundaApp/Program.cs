using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;

using FundaApp.BusinessLogic;
using FundaApp.DataAccess;
using FundaApp.DomainEntities;

using Newtonsoft.Json;

namespace FundaApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IDataProcessor dataProcessor = new DataProcessor();
            String command;
            Boolean quitNow = false;

            ShowMenu();

            while (!quitNow)
            {
                try
                {
                    command = Console.ReadLine();
                    switch (command)
                    {
                        case "h":
                            ShowMenu();
                            break;

                        case "1":
                            Console.WriteLine("Makelaars in Amsterdam die de meeste objecten te koop hebben staan :");
                            WriteGetTop10MakelaarsResult(dataProcessor);
                            break;

                        case "2":
                            Console.WriteLine("Makelaars in Amsterdam die de meeste objecten met een tuin te koop hebben staan :");
                            WriteGetTop10MakelaarsResult(dataProcessor, true);
                            break;

                        case "q":
                            quitNow = true;
                            break;

                        default:
                            Console.WriteLine($"Onbekend commando => '{command}', kies 'h' voor hulp.");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"LET OP :");
                    Console.WriteLine($"Er is een overwachte fout opgetreden : {exception.Message}.!");
                    Console.WriteLine($"Wacht een minuut en probeer het opnieuw....!");
                }
            }
        }

        private static void WriteGetTop10MakelaarsResult(IDataProcessor dataProcessor, bool withGarden = false)
        {
            Console.WriteLine("------------------------------------------------------------");
            var results = dataProcessor.GetTop10Makelaars("Amsterdam", withGarden);
            results.ForEach(x => Console.WriteLine($"{x.Count} objecten bij makelaar '{x.Name}'."));
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("");
        }

        private static void ShowMenu()
        {
            Console.WriteLine("Kies een commando uit de volgende opties : ");
            Console.WriteLine("h : Uitleg over de commando's.");
            Console.WriteLine("q : Verlaat het programma. ");
            Console.WriteLine("1 : Toon de top 10 van makelaars in Amsterdam die de meeste objecten te koop hebben staan .");
            Console.WriteLine("2 : Toon de top 10 van makelaars in Amsterdam die de meeste objecten met een tuin te koop hebben staan .");
            Console.WriteLine("---------------------------------------------------------------------------------------------");
            Console.WriteLine("");
        }
    }
}
