using System.Collections.Generic;

namespace SupportBank
{
    public interface IReadWriter
    {
        List<Transaction> Read(string filename);
        void Write(string filename, HashSet<Transaction> transactions);
    }
}