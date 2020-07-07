using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBank {
    internal class Program {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        
        public static void ListAll(List<Transaction> transactions) {
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

        public static void ListAccount(List<Transaction> transactions, string name) {
            transactions.FindAll(t => t.From == name || t.To == name)  
               .ForEach(t => Console.WriteLine(t));
        }
        
        public static List<Transaction> readCsv(string filename) {
            var TransactionList = new List<Transaction>();
            
            using(var reader = new StreamReader(filename)) {
                reader.ReadLine(); // header
                var lineNum = 1;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    lineNum++;
                    var strings = line.Split(',');
                    if (strings.Length != 5) {
                        Console.WriteLine($"Malformed transaction - line {lineNum}: {line}");
                        continue;
                    }
                    try {
                        var date = DateTime.Parse(strings[0]);
                        TransactionList.Add(new Transaction(strings[0], strings[1], 
                            strings[2], strings[3], decimal.Parse(strings[4])));
                    } catch {
                        Console.WriteLine($"Malformed transaction - line {lineNum}: {line}");
                    }
                }
            }
            return TransactionList;
        }

        public static List<Transaction> readJson(string filename) {
            return JsonConvert.DeserializeObject<List<Transaction>>(
                File.ReadAllText(filename));
        }
        
        public static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
            
            var TransactionList = readJson("Transactions2013.json");
            
            while (true)
            {
                Console.WriteLine(@"""List All"" or ""List (Account)""");
                var input = Console.ReadLine();

                if (input.Equals("List All"))
                {
                    ListAll(TransactionList);
                    break;
                }
                else
                {
                    var r = new Regex("List ([a-z]+( [a-z])?)", RegexOptions.IgnoreCase);
                    var m = r.Match(input);
                    if (m.Success)
                    {
                        ListAccount(TransactionList, m.Groups[1].ToString());
                        break;
                    }
                    Console.WriteLine("Incorrect Format");
                }
            }
        }
    }
}