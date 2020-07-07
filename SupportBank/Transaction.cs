namespace SupportBank {
    public class Transaction {
        internal string Date { get; } 
        internal string From { get; }  
        internal string To { get; }  
        internal string Narrative { get; } 
        internal decimal Amount { get; } 

        public Transaction (string Date, string FromAccount, string ToAccount, string Narrative, decimal Amount)
        {
            this.Date = Date; 
            this.From = FromAccount ; 
            this.To = ToAccount ; 
            this.Narrative = Narrative; 
            this.Amount = Amount;
        }

        public override string ToString() {
            return $"From: {From}, To: {To}, Amt: {Amount}, Date: {Date}, For: {Narrative}";
        }
    }
}