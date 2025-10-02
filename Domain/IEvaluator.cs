namespace Domain;

public interface IEvaluator
{
    public string Process(string input);
    public Task<string> ProcessAsync(string input);
}