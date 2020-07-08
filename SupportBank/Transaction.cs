using System;

namespace SupportBank {
    public class Transaction {
        internal DateTime Date { get; } 
        internal string From { get; }  
        internal string To { get; }  
        internal string Narrative { get; } 
        internal decimal Amount { get; } 

        public Transaction (DateTime Date, string FromAccount, string ToAccount, string Narrative, decimal Amount)
        {
            this.Date = Date; 
            this.From = FromAccount ; 
            this.To = ToAccount ; 
            this.Narrative = Narrative; 
            this.Amount = Amount;
        }

        public override string ToString() {
            return $"From: {From}, To: {To}, Amt: {Amount}, Date: {Date.ToString("dd/MM/yyyy")}, For: {Narrative}";
        }
    }
}