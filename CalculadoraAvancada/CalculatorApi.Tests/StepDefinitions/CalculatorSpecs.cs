using Microsoft.AspNetCore.Mvc.Testing;
using TechTalk.SpecFlow;
using Xunit;
using CalculatorApi.Models;
using System.Text.Json;

namespace CalculatorApi.Tests.StepDefinitions;

[Binding]
public class CalculatorSteps : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    // Test state variables
    private CalculationResponse? _actualResult;
    private int _actualStatusCode;
    private string _actualContent = string.Empty;

    public CalculatorSteps(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Given(@"the calculator system is available")]
    public async Task GivenTheCalculatorSystemIsAvailable()
    {
        // Arrange
        var testEndpoint = "/api/calculator/add/1/1";
        
        // Act
        var response = await _client.GetAsync(testEndpoint);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.BadRequest);
        Console.WriteLine("Calculator system is available and responding");
    }

    [When(@"I add (.*) and (.*)")]
    public async Task WhenIAdd(decimal firstNumber, decimal secondNumber)
    {
        // Arrange
        var endpoint = $"/api/calculator/add/{firstNumber}/{secondNumber}";
        
        // Act
        var response = await _client.GetAsync(endpoint);
        _actualStatusCode = (int)response.StatusCode;
        _actualContent = await response.Content.ReadAsStringAsync();
        
        // Parse response
        _actualResult = JsonSerializer.Deserialize<CalculationResponse>(_actualContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        // Assert (logging for debugging)
        Console.WriteLine($"API Response: {_actualContent}");
        Console.WriteLine($"Status Code: {response.StatusCode}");
    }

    [When(@"I try to add (.*) and (.*)")]
    public async Task WhenITryToAdd(decimal firstNumber, decimal secondNumber)
    {
        await WhenIAdd(firstNumber, secondNumber);
    }

    [When(@"I divide (.*) by (.*)")]
    public async Task WhenIDivide(decimal dividend, decimal divisor)
    {
        // Arrange
        var endpoint = $"/api/calculator/divide/{dividend}/{divisor}";
        
        // Act
        var response = await _client.GetAsync(endpoint);
        _actualStatusCode = (int)response.StatusCode;
        _actualContent = await response.Content.ReadAsStringAsync();
        
        // Parse response
        _actualResult = JsonSerializer.Deserialize<CalculationResponse>(_actualContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        // Assert (logging for debugging)
        Console.WriteLine($"API Response: {_actualContent}");
        Console.WriteLine($"Status Code: {response.StatusCode}");
    }

    [When(@"I try to divide (.*) by (.*)")]
    public async Task WhenITryToDivide(decimal dividend, decimal divisor)
    {
        await WhenIDivide(dividend, divisor);
    }

    [Then(@"the result should be (.*)")]
    public void ThenTheResultShouldBe(decimal expectedResult)
    {
        // Arrange
        var errorMessage = $"Expected result: {expectedResult}, but got: {_actualResult?.Result}";
        
        // Act & Assert
        Assert.NotNull(_actualResult);
        Assert.Equal(expectedResult, _actualResult.Result);
        
        Console.WriteLine($"Expected: {expectedResult}, Actual: {_actualResult.Result}");
    }

    [Then(@"the operation should be successful")]
    public void ThenTheOperationShouldBeSuccessful()
    {
        // Arrange
        var expectedStatusCode = 200;
        var expectedSuccess = true;
        
        // Act & Assert
        Assert.Equal(expectedStatusCode, _actualStatusCode);
        Assert.NotNull(_actualResult);
        Assert.Equal(expectedSuccess, _actualResult.Success);
        
        Console.WriteLine("Operation completed successfully");
    }

    [Then(@"the operation should fail")]
    public void ThenTheOperationShouldFail()
    {
        // Arrange
        var expectedStatusCode = 400;
        var expectedSuccess = false;
        
        // Act & Assert
        Assert.Equal(expectedStatusCode, _actualStatusCode);
        Assert.NotNull(_actualResult);
        Assert.Equal(expectedSuccess, _actualResult.Success);
        
        Console.WriteLine("Operation failed as expected");
    }

    [Then(@"I should receive the message ""(.*)""")]
    public void ThenIShouldReceiveTheMessage(string expectedMessage)
    {
        // Arrange
        var errorContext = $"Expected message: '{expectedMessage}', but got: '{_actualResult?.ErrorMessage}'";
        
        // Act & Assert
        Assert.NotNull(_actualResult);
        Assert.NotNull(_actualResult.ErrorMessage);
        Assert.Equal(expectedMessage, _actualResult.ErrorMessage);
        
        Console.WriteLine($"Error message received: {_actualResult.ErrorMessage}");
    }
}