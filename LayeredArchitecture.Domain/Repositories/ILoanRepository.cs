using LayeredArchitecture.Domain.Entities;

namespace LayeredArchitecture.Domain.Repositories;

public interface ILoanRepository
{
    Task<LoanApplication?> GetByIdAsync(Guid loanId);
    Task SaveAsync(LoanApplication application);
    Task UpdateAsync(LoanApplication application);
}
