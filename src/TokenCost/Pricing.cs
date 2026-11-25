namespace TokenCost;

/// <summary>List prices in USD per million tokens on 25 November 2026 (prompts up to 100K tokens).</summary>
public static class Pricing
{
    public sealed record Price(decimal InputPerMillion, decimal OutputPerMillion);

    private static readonly Dictionary<string, Price> Prices = new()
    {
        ["claude-haiku-5-5"] = new(0.10m, 0.50m),
        ["claude-sonnet-5-5"] = new(2m, 10m),
        ["claude-opus-5-5"] = new(4m, 20m),
        ["claude-fable-5-2"] = new(8m, 40m),
        ["claude-haiku-4-5"] = new(1m, 5m)
    };

    public static Price? For(string model) => Prices.GetValueOrDefault(model);
}
