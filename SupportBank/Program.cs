using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NDesk.Options;
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
            var r = new Regex("(?<command>list |import file |export file )(?<args>.*)", RegexOptions.IgnoreCase);
            var rSearch = new Regex("(?<name>[a-z]+( [a-z])?)(?<date> .*)?", RegexOptions.IgnoreCase);

            Console.WriteLine("\"List All\" or \"List (Account)\" or \"Import File (FileName)\" " +
                              "or \"Export File (FileName)\"");
            var input = Console.ReadLine();
            var m = r.Match(input);

            if (!m.Success) {
                Console.WriteLine("Command not recognised");
                return;
            }
            
            switch (m.Groups["command"].ToString().ToLower()) {
                case "list ": {
                    var m2 = rSearch.Match(m.Groups["args"].ToString());
                    if (!m2.Success) {
                        Console.WriteLine("Name/date not recognised");
                        return;
                    }
                    var date = DateTime.Now;
                    if (!m2.Groups["date"].ToString().Equals("")) {
                        try {
                            date = DateTime.Parse(m2.Groups["date"].ToString());
                        }
                        catch {
                            Console.WriteLine("Date not recognised");
                        }
                    }
                    var name = m2.Groups["name"].ToString();
                    if (name.ToLower().Equals("all")) {
                        ListAll(date);
                    } else {
                        ListAccount(name, date);
                    }
                    break;
                }
                case "import file ": {
                    var filename = m.Groups["args"].ToString();
                    try {
                        if (filename.EndsWith(".json")) {
                            transactions.UnionWith(jsonReadWriter.Read(filename));
                        }
                        else if (filename.EndsWith(".xml")) {
                            transactions.UnionWith(xmlReadWriter.Read(filename));
                        }
                        else {
                            transactions.UnionWith(csvReadWriter.Read(filename));
                        }
                    } catch {
                        Console.WriteLine($"Could not read file: {filename}");
                    }
                    break;
                }
                case "export file ": {
                    var filename = m.Groups["args"].ToString();
                    try {
                        if (filename.EndsWith(".json")) {
                            jsonReadWriter.Write(filename, transactions);
                        }
                        else if (filename.EndsWith(".xml")) {
                            xmlReadWriter.Write(filename, transactions);
                        }
                        else {
                            csvReadWriter.Write(filename, transactions);
                        }
                    } catch {
                        Console.WriteLine($"Could not write file: {filename}");
                    }
                    break;
                }
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