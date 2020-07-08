using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBank {
    internal class Program {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private static List<Transaction> transactions = new List<Transaction>();
        private static IReadWriter csvReadWriter = new CsvReadWriter();
        private static IReadWriter jsonReadWriter = new JsonReadWriter();
        private static IReadWriter xmlReadWriter = new XmlReadWriter();

        public static void Main()
        {
            LoggingSetup();

            while (true)
            {
                runCommand();
            }
        }

        private static void LoggingSetup()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget
            {
                FileName = @"C:\Work\Logs\SupportBank.log",
                Layout = @"${longdate} ${level} - ${logger}: ${message}"
            };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
        }

        public static void runCommand()
        {
            var r = new Regex("List ([a-z]+( [a-z])?)", RegexOptions.IgnoreCase);
            Console.WriteLine(@"""List All"" or ""List (Account)"" or ""Import File (FileName)"" or ""Export File (FileName)""");
            var input = Console.ReadLine();
            var m = r.Match(input);
                
            if (input.ToLower().StartsWith("import file "))
            {
                input = input.Substring(12);
                try {
                    if (input.EndsWith(".json")) {
                        transactions = jsonReadWriter.read(input);
                    }
                    else if (input.EndsWith(".csv")) {
                        transactions = csvReadWriter.read(input);
                    }
                    else {
                        transactions = xmlReadWriter.read(input);
                    }
                }
                catch {
                    Console.WriteLine($"Could not read file: {input}");
                }
            }
            else if (input.ToLower().StartsWith("export file "))
            {
                input = input.Substring(12);
                try {
                    if (input.EndsWith(".json")) {
                        jsonReadWriter.write(input, transactions);
                    }
                    else if (input.EndsWith(".csv")) {
                        csvReadWriter.write(input, transactions);
                    }
                    else {
                        xmlReadWriter.write(input, transactions);
                    }
                }
                catch {
                    Console.WriteLine($"Could not write file: {input}");
                }
            }
            else if (input.ToLower().Equals("list all"))
            {
                ListAll();
            }
            else if (m.Success)
            {
                ListAccount(m.Groups[1].ToString());
            }
            else
            { 
                Console.WriteLine("Incorrect Format");
            }
        }
        
        public static void ListAll() {
            var accounts = new Dictionary<string, decimal>(); // make an object, use decimal as appose to float?
            foreach (var t in transactions) {
                if (!accounts.ContainsKey(t.From)) {
                    accounts.Add(t.From, decimal.Zero);
                }
                if (!accounts.ContainsKey(t.To)) {
                    accounts.Add(t.To, decimal.Zero);
                }
                accounts[t.From] -= t.Amount;
                accounts[t.To] += t.Amount;
            }
            foreach (var entry in accounts) {
                Console.WriteLine($"{entry.Key}: {entry.Value}"); // string interpolation
            }
        }

        public static void ListAccount(string name) {
            transactions.FindAll(t => t.From == name || t.To == name)  
               .ForEach(t => Console.WriteLine(t));
        }
    }
}