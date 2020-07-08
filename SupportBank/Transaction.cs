using System;

namespace SupportBank {
    public class Transaction {
        public DateTime Date { get; set; } 
        public string From { get; set; }  
        public string To { get; set; }  
        public string Narrative { get; set; } 
        public decimal Amount { get; set; } 

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