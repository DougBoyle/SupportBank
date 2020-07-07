using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportBank {
    internal class Program {
        public void ListAll(List<Transaction> transactions) {
            var accounts = new Dictionary<string, float>();
            foreach (var t in transactions) {
                if (!accounts.ContainsKey(t.From)) {
                    accounts.Add(t.From, 0.0f);
                }
                if (!accounts.ContainsKey(t.To)) {
                    accounts.Add(t.To, 0.0f);
                }
                accounts[t.From] -= t.Amount;
                accounts[t.To] += t.Amount;
            }
            foreach (var entry in accounts) {
                Console.WriteLine("{0}: {1}", entry.Key, entry.Value);
            }
        }

        public void ListAccount(List<Transaction> transactions, string name) {
            transactions.FindAll(t => t.From == name || t.To == name)  
               .ForEach(t => Console.WriteLine(t));
        }
        
        public static void Main(string[] args)
        {
            var TransactionList = new List<Transaction>();
            
            using(var reader = new StreamReader("C:/work/training/SupportBank/SupportBank/Transactions2014.csv")) {
                reader.ReadLine(); // header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var strings = line.Split(',');
                    TransactionList.Add(new Transaction(strings[0], strings[1], strings[2], 
                        strings[3],float.Parse(strings[4])));
                }
            }
        }
    }

    internal class Transaction
    {
        internal string Date; 
        internal string From; 
        internal string To; 
        internal string Narrative;
        internal float Amount;

        public Transaction (string Date, string From, string To, string Narrative, float Amount)
        {
            this.Date = Date; this.From = From ; this.To = To ; this.Narrative = Narrative; this.Amount = Amount;
        }

        public override string ToString() {
            return $"From: {From}, To:{To}, Amt:{Amount}, Date:{Date}, For{Narrative}";
        }
    } 
}