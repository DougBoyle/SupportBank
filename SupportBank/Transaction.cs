using System;
using System.Globalization;

namespace SupportBank {
    public class Transaction {
        public DateTime Date { get; } 
        public string From { get; }  
        public string To { get; }  
        public string Narrative { get; } 
        public decimal Amount { get; } 

        public Transaction (DateTime Date, string FromAccount, string ToAccount, string Narrative, decimal Amount)
        {
            this.Date = Date; 
            this.From = FromAccount ; 
            this.To = ToAccount ; 
            this.Narrative = Narrative; 
            this.Amount = Amount;
        }

        public override string ToString() {
            return $"From: {From}, To: {To}, Amt: {Amount.ToString("C", CultureInfo.CurrentCulture)}, " +
                   $"Date: {Date.ToString("dd/MM/yyyy")}, For: {Narrative}";
        }

        public override bool Equals(object obj) {
            if ((obj != null) && obj is Transaction t) {
                return Date == t.Date && To == t.To && From == t.From
                       && Amount == t.Amount && Narrative == t.Narrative;
            }
            return false;
        }

        public override int GetHashCode() {
            return Narrative.GetHashCode() + Date.GetHashCode();
        }
    }
}