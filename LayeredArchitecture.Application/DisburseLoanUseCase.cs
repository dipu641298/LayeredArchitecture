using LayeredArchitecture.Business.Services;
using LayeredArchitecture.Domain.Clients;
using LayeredArchitecture.Domain.Entities;
using LayeredArchitecture.Domain.Repositories;

namespace LayeredArchitecture.Application;

public class DisburseLoanUseCase
{
    private readonly ILoanRepository _repository;
    private readonly IBankTransferClient _bankClient;
    private readonly DisbursementCalculatorService _calculatorService;

    public DisburseLoanUseCase(
        ILoanRepository repository,
        IBankTransferClient bankClient,
        DisbursementCalculatorService calculator)
    {
        _repository = repository;
        _bankClient = bankClient;
        _calculatorService = calculator;
    }

    public async Task<bool> ExecuteAsync(Guid loanId)
    {
        // 1. Fetch state
        var loan = await _repository.GetByIdAsync(loanId);
        if (loan == null)
            throw new ArgumentException("Loan not found.");

        // 2. Delegate to Business Layer to calculate actual payout
        decimal payoutAmount = _calculatorService.CalculatePayoutAmount(loan);

        // 3. I/O: Attempt to wire the funds
        bool transferSuccess = await _bankClient.WireFundsAsync(loan.CustomerId, payoutAmount);

        if (!transferSuccess)
        {
            // In a real system, you might queue a retry or raise an alert here.
            return false;
        }

        // 4. Update state
        loan.Status = LoanStatus.Disbursed;
        loan.DisbursedDate = DateTime.UtcNow;

        // 5. Persist state
        await _repository.UpdateAsync(loan);

        return true;
    }
}
