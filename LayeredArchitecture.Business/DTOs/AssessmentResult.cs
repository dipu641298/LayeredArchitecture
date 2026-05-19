using LayeredArchitecture.Domain.Entities;

namespace LayeredArchitecture.Business.DTOs;

public record AssessmentResult(
    LoanStatus Status,
    decimal InterestRate,
    string? RejectionReason
);