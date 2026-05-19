using LayeredArchitecture.Application;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ProcessLoanUseCase _processLoan;
    private readonly DisburseLoanUseCase _disburseLoan;

    public LoansController(ProcessLoanUseCase processLoan, DisburseLoanUseCase disburseLoan)
    {
        _processLoan = processLoan;
        _disburseLoan = disburseLoan;
    }

    [HttpPost("apply")]
    public async Task<IActionResult> Apply([FromBody] LoanRequestDto request)
    {
        // The controller just translates HTTP -> Application layer
        var result = await _processLoan.ExecuteAsync(request.CustomerId, request.Amount, 
                                    request.MonthlyIncome, request.ExistingDebt);

        return Ok(new
        {
            result.Id,
            result.Status,
            result.ApprovedInterestRate
        });
    }

    [HttpPost("{id}/disburse")]
    public async Task<IActionResult> Disburse(Guid id)
    {
        try
        {
            bool success = await _disburseLoan.ExecuteAsync(id);

            if (!success)
                return StatusCode(502, "Bank transfer failed. Please try again.");

            return Ok(new { Message = "Loan successfully disbursed." });
        }
        catch (InvalidOperationException ex) // Catching business rule violations
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (ArgumentException ex) // Catching not-found errors
        {
            return NotFound(new { Error = ex.Message });
        }
    }
}

public record LoanRequestDto(Guid CustomerId, decimal Amount, decimal MonthlyIncome, decimal ExistingDebt);