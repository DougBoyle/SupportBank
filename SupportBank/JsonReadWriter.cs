using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SupportBank
{
    public class JsonReadWriter : IReadWriter
    {
        public List<Transaction> read(string filename)
        {
            return JsonConvert.DeserializeObject<List<Transaction>>(
                File.ReadAllText(filename));
        }

        public void write(string filename, List<Transaction> transactions)
        {
            var writer = new JsonTextWriter(new StreamWriter(filename));
            var ser = new JsonSerializer();
            writer.Formatting = Newtonsoft.Json.Formatting.Indented;
            ser.Serialize(writer, transactions);            
            writer.Close();
        }
    }
}