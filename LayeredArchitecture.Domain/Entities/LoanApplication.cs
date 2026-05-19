namespace LayeredArchitecture.Domain.Entities
{
    public class LoanApplication
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal ApprovedInterestRate { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Pending;
        public DateTime? DisbursedDate { get; set; }
    }
}
