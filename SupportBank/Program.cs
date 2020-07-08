using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBank {
    internal class Program {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private static HashSet<Transaction> transactions = new HashSet<Transaction>();
        private static IReadWriter csvReadWriter = new CsvReadWriter();
        private static IReadWriter jsonReadWriter = new JsonReadWriter();
        private static IReadWriter xmlReadWriter = new XmlReadWriter();

        public static void Main()
        {
            SetupLogging();
            while (true)
            {
                RunCommand();
            }
        }

        private static void SetupLogging()
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

        public static void RunCommand()
        {
            var r = new Regex("List ([a-z]+( [a-z])?)(?<date> .*)?", RegexOptions.IgnoreCase);
            Console.WriteLine("\"List All\" or \"List (Account)\" or \"Import File (FileName)\" " +
                              "or \"Export File (FileName)\"");
            var input = Console.ReadLine();
            var m = r.Match(input);
                
            if (input.ToLower().StartsWith("import file "))
            {
                input = input.Substring(12);
                try {
                    if (input.EndsWith(".json")) {
                        transactions.UnionWith(jsonReadWriter.Read(input));
                    }
                    else if (input.EndsWith(".csv")) {
                        transactions.UnionWith(csvReadWriter.Read(input));
                    }
                    else {
                        transactions.UnionWith(xmlReadWriter.Read(input));
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
                        jsonReadWriter.Write(input, transactions);
                    }
                    else if (input.EndsWith(".csv")) {
                        csvReadWriter.Write(input, transactions);
                    }
                    else {
                        xmlReadWriter.Write(input, transactions);
                    }
                }
                catch {
                    Console.WriteLine($"Could not write file: {input}");
                }
            }
            else if (input.ToLower().StartsWith("list all"))
            {
                try {
                    var d = DateTime.Parse(input.Substring(8));
                    ListAll(d);
                } catch {
                    if (input.Length > 8) {
                        Console.WriteLine("Date input not recognised");
                    }
                    ListAll(DateTime.Now);
                }
            }
            else if (m.Success)
            {
                var name = m.Groups[1].ToString();
                var date = m.Groups["date"].ToString();
                if (date.Length == 0) {
                    ListAccount(name, DateTime.Now);
                }
                else {
                    try {
                        ListAccount(name, DateTime.Parse(date));
                    }
                    catch {
                        Console.WriteLine("Date input not recognised");
                        ListAccount(name, DateTime.Now);
                    }
                }
            }
            else
            { 
                Console.WriteLine("Incorrect Format");
            }
        }

        public static void ListAll(DateTime date) {
            var accounts = new Dictionary<string, decimal>(); 
            foreach (var t in transactions.Where(t => t.Date <= date)) {
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
                Console.WriteLine($"{entry.Key}: {entry.Value.ToString("C", CultureInfo.CurrentCulture)}"); 
            }
        }

        public static void ListAccount(string name, DateTime date) {
            transactions.ToList().FindAll(t => (t.From == name || t.To == name) && t.Date <= date)  
               .ForEach(t => Console.WriteLine(t));
        }
    }
}