using LayeredArchitecture.Domain.Entities;
using LayeredArchitecture.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LayeredArchitecture.DataAccess.Repositories;

public class SqlLoanRepository : ILoanRepository
{
    private readonly LoanDbContext _dbContext;

    public SqlLoanRepository(LoanDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoanApplication?> GetByIdAsync(Guid loanId)
    {
        // Fetches the entity. EF Core will automatically track it.
        return await _dbContext.Loans.FirstOrDefaultAsync(loan => loan.Id == loanId);
    }

    public async Task SaveAsync(LoanApplication application)
    {
        // Inserts a brand new record
        await _dbContext.Loans.AddAsync(application);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(LoanApplication application)
    {
        // In EF Core, if you fetched the entity using GetByIdAsync in the same 
        // HTTP request/scope, it is already being tracked. You strictly only 
        // need to call SaveChangesAsync(). However, explicitly calling Update() 
        // ensures it works even if the entity became detached.
        _dbContext.Loans.Update(application);
        await _dbContext.SaveChangesAsync();
    }
}
