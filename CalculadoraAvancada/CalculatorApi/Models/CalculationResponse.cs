namespace CalculatorApi.Models;

public class CalculationResponse
{
    public decimal Result { get; set; }
    public string Operation { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}