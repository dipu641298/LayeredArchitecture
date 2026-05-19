using LayeredArchitecture.Domain.Entities;

namespace LayeredArchitecture.Business.Services;

public class DisbursementCalculatorService
{
    public decimal CalculatePayoutAmount(LoanApplication loan)
    {
        if (loan.Status != LoanStatus.Approved)
            throw new InvalidOperationException("Only approved loans can be disbursed.");

        // Rule: 2% origination fee for loans under $10k, flat $200 for loans over $10k
        decimal originationFee = loan.RequestedAmount < 10000
            ? loan.RequestedAmount * 0.02m
            : 200m;

        return loan.RequestedAmount - originationFee;
    }
}