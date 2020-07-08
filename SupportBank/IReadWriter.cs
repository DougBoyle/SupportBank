using System.Collections.Generic;

namespace SupportBank
{
    public interface IReadWriter
    {
        List<Transaction> read(string filename);
        void write(string filename, List<Transaction> transactions);
    }
}