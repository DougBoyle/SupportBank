using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SupportBank
{
    public class JsonReadWriter : IReadWriter
    {
        public List<Transaction> Read(string filename) {
            
            var transactions = new List<Transaction>();
            var ar = JArray.Parse(File.ReadAllText(filename));
            foreach (var t in ar) {
                try {
                    transactions.Add(t.ToObject<Transaction>());
                }
                catch {
                    Console.WriteLine($"Malformed json object: {t}");
                }
            }
         return transactions;
        }

        public void Write(string filename, HashSet<Transaction> transactions)
        {
            var writer = new JsonTextWriter(new StreamWriter(filename));
            var ser = new JsonSerializer();
            writer.Formatting = Formatting.Indented;
            ser.Serialize(writer, transactions);            
            writer.Close();
        }

    }
}