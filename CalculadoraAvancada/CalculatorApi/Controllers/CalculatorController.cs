using Microsoft.AspNetCore.Mvc;
using CalculatorApi.Models;

namespace CalculatorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    [HttpGet("add/{a}/{b}")]
    public ActionResult<CalculationResponse> Add(decimal a, decimal b)
    {
        // Business rule: Don't accept values greater than 1 million
        if (Math.Abs(a) > 1_000_000 || Math.Abs(b) > 1_000_000)
        {
            return BadRequest(new CalculationResponse
            {
                Success = false,
                ErrorMessage = "Values cannot exceed 1 million",
                Operation = "Addition"
            });
        }

        var result = a + b;

        // Business rule: Result cannot exceed 1.8 million
        if (Math.Abs(result) > 1_800_000)
        {
            return BadRequest(new CalculationResponse
            {
                Success = false,
                ErrorMessage = "Result exceeds maximum limit of 1.8 million",
                Operation = "Addition"
            });
        }

        return Ok(new CalculationResponse
        {
            Result = result,
            Operation = "Addition",
            Success = true
        });
    }

    [HttpGet("divide/{dividend}/{divisor}")]
    public ActionResult<CalculationResponse> Divide(decimal dividend, decimal divisor)
    {
        // Business rule: Don't allow division by zero
        if (divisor == 0)
        {
            return BadRequest(new CalculationResponse
            {
                Success = false,
                ErrorMessage = "Division by zero is not allowed",
                Operation = "Division"
            });
        }

        var result = dividend / divisor;

        return Ok(new CalculationResponse
        {
            Result = Math.Round(result, 2),
            Operation = "Division",
            Success = true
        });
    }
}