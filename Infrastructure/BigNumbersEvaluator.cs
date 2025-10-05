using Domain;
using Domain.Constants;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Infrastructure;
public class BigNumbersEvaluator : IEvaluator
{
    private readonly CancellationTokenSource cts = new();   // Added for backward compatibility and possible future implementation of Cancel button

    public async Task<string> ProcessAsync(string input)
    {
        return await Task.Run(() => ProcessInternal(input, cts.Token), cts.Token);
    }

    public string Process(string input) => ProcessInternal(input, cts.Token);


    private string ProcessInternal(string input, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        try
        {
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
                return $"Error: Invalid characters: {invalidCharacters}";

            var tokens = Tokenize(input);
            var postfix = ToPostfix(tokens);
            var result = EvaluatePostfix(postfix, ct);

            return result.ToString();
        }
        catch (OperationCanceledException)
        {
            return "Error: Operation canceled";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    private string GetInvalidCharacters(string input)
    {
        var invalidChars = Regex.Replace(input, Evaluators.ValidCharactersPattern, "");

        return invalidChars.Length > 0
            ? string.Join(" ", invalidChars.Distinct())
            : string.Empty;
    }

    private static IEnumerable<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        var number = "";

        foreach (var ch in expression)
        {
            if (char.IsDigit(ch))
            {
                number += ch;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(number))
                {
                    tokens.Add(number);
                    number = "";
                }

                if (!char.IsWhiteSpace(ch))
                    tokens.Add(ch.ToString());
            }
        }

        if (!string.IsNullOrWhiteSpace(number))
            tokens.Add(number);

        return tokens;
    }

    private static IEnumerable<string> ToPostfix(IEnumerable<string> tokens)
    {
        var output = new List<string>();
        var ops = new Stack<string>();

        foreach (var token in tokens)
        {
            if (BigInteger.TryParse(token, out _))
            {
                output.Add(token);
            }
            else if ("+-*/".Contains(token))
            {
                while (ops.Count > 0 && Precedence(ops.Peek()) >= Precedence(token))
                    output.Add(ops.Pop());

                ops.Push(token);
            }
            else if (token == "(")
            {
                ops.Push(token);
            }
            else if (token == ")")
            {
                while (ops.Count > 0 && ops.Peek() != "(")
                    output.Add(ops.Pop());

                if (ops.Count == 0)
                    throw new Exception("Mismatched parentheses");

                ops.Pop(); // remove "("
            }
        }

        while (ops.Count > 0)
            output.Add(ops.Pop());

        return output;

        int Precedence(string op) => op switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            _ => 0
        };
    }

    private static BigInteger EvaluatePostfix(IEnumerable<string> postfix, CancellationToken ct)
    {
        var stack = new Stack<BigInteger>();

        foreach (var token in postfix)
        {
            ct.ThrowIfCancellationRequested();

            if (BigInteger.TryParse(token, out var num))
            {
                stack.Push(num);
            }
            else
            {
                if (stack.Count < 2)
                    throw new Exception("Invalid expression");

                var b = stack.Pop();
                var a = stack.Pop();

                var result = token switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => b == 0 ? throw new DivideByZeroException() : a / b,
                    _ => throw new Exception($"Unknown operator {token}")
                };

                stack.Push(result);
            }
        }

        if (stack.Count != 1)
            throw new Exception("Invalid expression");

        return stack.Pop();
    }
}