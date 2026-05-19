using LayeredArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LayeredArchitecture.DataAccess;

public class LoanDbContext : DbContext
{
    public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options) { }

    public DbSet<LoanApplication> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API configuration for the entity
        modelBuilder.Entity<LoanApplication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestedAmount).HasPrecision(18, 2);
            entity.Property(e => e.ApprovedInterestRate).HasPrecision(5, 2);

            // Storing the enum as a string in the database is highly recommended 
            // so it remains readable if you look directly at the SQL tables.
            entity.Property(e => e.Status).HasConversion<string>();
        });
    }
}
