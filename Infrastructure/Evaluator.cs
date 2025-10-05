using Domain;
using Domain.Constants;
using System.Data;
using System.Text.RegularExpressions;

namespace Infrastructure;
public class Evaluator : IEvaluator
{
    public string Process(string input)
    {
        try
        {
            // Check
            if (string.IsNullOrWhiteSpace(input))
                return "";

            input = input.Replace(" ", "");
            if (input.Contains("/0"))
            {
                return "Error: Division by zero";
            }

            // Validate
            var invalidCharacters = GetInvalidCharacters(input);
            if (!string.IsNullOrEmpty(invalidCharacters))
            {
                return $"Error: Invalid characters: {invalidCharacters}";
            }

            var result = new DataTable().Compute(input, "");
            long longResult = Convert.ToInt64(Math.Floor(Convert.ToDouble(result)));

            return longResult.ToString();
        }
        catch (OverflowException)
        {
            // rethrow for higher level handling.
            // From my research this is correct approach and not anti-pattern, but you have to disable User-unhandled Errors.
            throw;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    //                                                                  ¯\_(ツ)_/¯
    public async Task<string> ProcessAsync(string input) => await Task.Run(() => Process(input));

    private string GetInvalidCharacters(string input)
    {
        var invalidChars = Regex.Replace(input, Evaluators.ValidCharactersPattern, "");
        if (invalidChars.Length > 0)
        {
            // remove duplicates for readability
            var invalidCharacters = string.Join(" ", invalidChars.Distinct());
            return invalidCharacters;
        }
        else
        {
            return string.Empty;
        }
    }
}
