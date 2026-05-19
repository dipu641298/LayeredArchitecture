using LayeredArchitecture.Business.DTOs;
using LayeredArchitecture.Domain.Entities;

namespace LayeredArchitecture.Business.Services;

public class RiskAssessmentService
{
    private const decimal MaxDebtToIncomeRatio = 0.40m;

    public AssessmentResult Evaluate(
        LoanApplication loan,
        int creditScore,
        decimal monthlyIncome,
        decimal existingMonthlyDebt)
    {
        // 1. Hard Boundaries (Knockout Rules) returning LoanStatus.Rejected
        if (loan.RequestedAmount <= 0)
            return new AssessmentResult(LoanStatus.Rejected, 0, "Requested amount must be greater than zero.");

        if (creditScore < 600)
            return new AssessmentResult(LoanStatus.Rejected, 0, "Credit score is below the minimum threshold of 600.");

        if (monthlyIncome <= 0)
            return new AssessmentResult(LoanStatus.Rejected, 0, "Verifiable monthly income is required.");

        // 2. Debt-to-Income (DTI) Calculation
        decimal estimatedNewPayment = loan.RequestedAmount * 0.02m;
        decimal totalMonthlyObligations = existingMonthlyDebt + estimatedNewPayment;
        decimal dtiRatio = totalMonthlyObligations / monthlyIncome;

        if (dtiRatio > MaxDebtToIncomeRatio)
            return new AssessmentResult(LoanStatus.Rejected, 0, $"Debt-to-Income ratio ({dtiRatio:P}) exceeds the maximum allowed 40%.");

        // 3. Dynamic Interest Rate Calculation (Pricing) for Approved loans
        decimal baseRate = GetTieredBaseRate(creditScore);
        decimal riskPremium = loan.RequestedAmount > 100000 ? 1.5m : 0m;
        decimal finalInterestRate = baseRate + riskPremium;

        // Return successful state using LoanStatus.Approved
        return new AssessmentResult(LoanStatus.Approved, finalInterestRate, null);
    }

    private decimal GetTieredBaseRate(int creditScore) => creditScore switch
    {
        >= 800 => 3.5m,
        >= 740 => 4.5m,
        >= 670 => 6.0m,
        _ => 8.5m
    };
}