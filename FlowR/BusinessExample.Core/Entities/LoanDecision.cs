namespace BusinessExample.Core.Entities
{
    public class LoanDecision
    {
        public string Id { get; set; }

        public string LoanApplicationId { get; set; }

        public AffordabilityRating? AffordabilityRating { get; set; }

        public bool? IsEligible { get; set; }

        public IdentityCheckResult? IdentityCheckResult { get; set; }

        public LoanDecisionResult? Result { get; set; }

        public override string ToString() => this.Id;
    }

    public enum AffordabilityRating
    {
        Good,
        Fair,
        Poor
    }

    public enum IdentityCheckResult
    {
        IdentityFound,
        IdentityNotFound,
        ServiceUnavailable
    }

    public enum LoanDecisionResult
    {
        Accept,
        Refer,
        Decline
    }
}
