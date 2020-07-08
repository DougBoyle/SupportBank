using System;
using System.Collections.Generic;
using System.IO;

namespace SupportBank
{
    public class CsvReadWriter : IReadWriter
    {
        public List<Transaction> Read(string filename)
        {
            var transactions = new List<Transaction>();
            
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
                        transactions.Add(new Transaction(date, strings[1], 
                            strings[2], strings[3], decimal.Parse(strings[4])));
                    } catch {
                        Console.WriteLine($"Malformed transaction - line {lineNum}: {line}");
                    }
                }
            }

            return transactions;
        }

        public void Write(string filename, HashSet<Transaction> transactions)
        {
            var streamWriter = new StreamWriter(filename);

            streamWriter.WriteLine("Date,From,To,Narrative,Amount");
            foreach (var transaction in transactions)
            {
                streamWriter.WriteLine($"{transaction.Date.ToString("dd/MM/yyyy")},{transaction.From}," +
                                       $"{transaction.To},{transaction.Narrative},{transaction.Amount}");
            }
            streamWriter.Close();
        }
    }
}