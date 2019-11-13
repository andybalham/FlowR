namespace BusinessExample.Core.Entities
{
    public class LoanApplication
    {
        public string Id { get; set; }

        public string Reference { get; set; }
        
        public string EmailAddress { get; set; }

        public decimal MonthlyIncomeAmount { get; set; }

        public decimal MonthlyOutgoingAmount { get; set; }
        
        public string ApplicantName { get; set; }
        
        public decimal LoanAmount { get; set; }

        public bool IsEmployed { get; set; }

        public override string ToString() => this.Reference ?? this.Id;
    }
}
