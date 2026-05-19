using LayeredArchitecture.Business.Services;
using LayeredArchitecture.Domain.Clients;
using LayeredArchitecture.Domain.Entities;
using LayeredArchitecture.Domain.Repositories;

namespace LayeredArchitecture.Application;

public class ProcessLoanUseCase
{
    private readonly ILoanRepository _repository;
    private readonly ICreditBureauClient _creditClient;
    private readonly RiskAssessmentService _riskService;

    // Orchestrator injects I/O contracts and Business rules
    public ProcessLoanUseCase(
        ILoanRepository repository,
        ICreditBureauClient creditClient,
        RiskAssessmentService riskService)
    {
        _repository = repository;
        _creditClient = creditClient;
        _riskService = riskService;
    }

    public async Task<LoanApplication> ExecuteAsync(
        Guid customerId,
        decimal amount,
        decimal monthlyIncome,
        decimal existingDebt)
    {
        var loan = new LoanApplication
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            RequestedAmount = amount,
            Status = LoanStatus.Pending // Starts as pending
        };

        int creditScore = await _creditClient.GetCreditScoreAsync(customerId);

        // Call the updated service
        var assessment = _riskService.Evaluate(
            loan,
            creditScore,
            monthlyIncome,
            existingDebt
        );

        // Apply the new strongly-typed status directly
        loan.Status = assessment.Status;

        if (loan.Status == LoanStatus.Approved)
        {
            loan.ApprovedInterestRate = assessment.InterestRate;
        }

        await _repository.SaveAsync(loan);

        return loan;
    }
}