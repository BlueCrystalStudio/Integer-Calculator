using Infrastructure;

namespace Tests;
public class BigNumbersEvaluatorTests
{
    private readonly BigNumbersEvaluator _evaluator = new();

    // This is [Setup] equivalent in xUnit using Constructor
    public BigNumbersEvaluatorTests()
    {
        _evaluator = new ();
    }

    [Theory]
    [InlineData("100000 * 100000", "10000000000")]
    [InlineData("9223372036854775808 + 2", "9223372036854775810")]
    [InlineData("12345678901234567890 - 9876543210987654321", "2469135690246913569")]
    [InlineData("12345678901234567890 * 2", "24691357802469135780")]
    [InlineData("10000000000000000000 / 4", "2500000000000000000")]
    public void Process_ReturnsCorrectResult_ForBigNumbers(string input, string expectedResult)
    {
        // Act
        var result = _evaluator.Process(input);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("5 + 4", "9")]
    [InlineData("2 - 2", "0")]
    [InlineData("2 * 2", "4")]
    [InlineData("8 / 4", "2")]
    public void Process_ReturnsCorrectResult(string input, string expectedResult)
    {
        // Arrange

        // Act
        var result = _evaluator.Process(input);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("5 + 4", "9")]
    [InlineData("2 - 2", "0")]
    [InlineData("2 * 2", "4")]
    [InlineData("8 / 4", "2")]
    public async Task ProcessAsync_ReturnsCorrectResult(string input, string expectedResult)
    {
        // Arrange

        // Act
        var result = await _evaluator.ProcessAsync(input);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameResult_AsSyncVersion()
    {
        string input = "1 + 1 - 1 / 2 * 2";
        string expected = _evaluator.Process(input);

        string asyncResult = await _evaluator.ProcessAsync(input);

        Assert.Equal(expected, asyncResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("            ")]
    [InlineData(null)]
    public void Process_ReturnsEmpty_WhenInputIsNullOrWhitespace(string inputEmptyString)
    {
        Assert.Equal("", _evaluator.Process(inputEmptyString));
    }

    [Theory]
    [InlineData("a", "a")]
    [InlineData("5 + 4 + 3 + 2 + a", "a")]
    [InlineData("5 * Pi", "P i")]
    public void Process_ReturnsErrorString_WhenInputContainsInvalidCharacters(string input, string invalidChars)
    {
        // Act
        var result = _evaluator.Process(input);

        // Assert
        Assert.Contains("Error", result);
        Assert.Contains(invalidChars, result);
    }

    [Fact]
    public void Process_HandlesParenthesesCorrectly()
    {
        var result = _evaluator.Process("(2 + 3) * 4");
        Assert.Equal("20", result);
    }

    [Fact]
    public void Process_ReturnsError_OnDivisionByZero()
    {
        var result = _evaluator.Process("10 / 0");
        Assert.Equal("Error: Division by zero", result);
    }
}
