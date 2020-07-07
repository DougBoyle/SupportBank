using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SupportBank {
    internal class Program {
        public static void Main(string[] args)
        {
            var TransactionList = new List<Transaction>();
            
            using(var reader = new StreamReader("C:/work/training/SupportBank/SupportBank/Transactions2014.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var strings = line.Split(',');
                    
                    var newTransaction = new Transaction(strings[0], strings[1], strings[2], strings[3], float.Parse(strings[4]));
                    TransactionList.Add(newTransaction);
                }
            }
        }
    }

    internal class Transaction
    {
        string Date; string From; string To; string Narrative; float Amount;

        public Transaction (string Date, string From, string To, string Narrative, float Amount)
        {
            this.Date = Date; this.From = From ; this.To = To ; this.Narrative = Narrative; this.Amount = Amount;
        }
    } 
}