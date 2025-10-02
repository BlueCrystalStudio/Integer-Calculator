using Domain;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Infrastructure;
public class Evaluator : IEvaluator
{
    public string Process(string input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            // Validate
            if (!Regex.IsMatch(input, @"^[0-9+\-*/\s()]+$"))
                return "Error: Invalid characters";

            var result = new DataTable().Compute(input, "");
            int intResult = Convert.ToInt32(Math.Floor(Convert.ToDouble(result)));

            return intResult.ToString();
        }
        catch (DivideByZeroException)
        {
            return "Error: Division by zero";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public async Task<string> ProcessAsync(string input) => await Task.Run(() => Process(input));
}
