using Anthropic;
using Anthropic.Models.Messages;
using Microsoft.ML.Tokenizers;

namespace TokenCost;

/// <summary>Counts the tokens a model is billed for when it reads (or writes) a text.</summary>
public interface ITokenCounter
{
    /// <summary>Short, stable name used in result files, e.g. "claude-haiku-5-5" or "openai-o200k_base".</summary>
    string Name { get; }

    Task<long> CountAsync(string text, CancellationToken ct);
}

/// <summary>Exact, offline counts with OpenAI's published BPE vocabularies (cl100k_base: GPT-4; o200k_base: GPT-4o and later).</summary>
public sealed class TiktokenCounter(string encoding) : ITokenCounter
{
    private readonly Tokenizer _tokenizer = TiktokenTokenizer.CreateForEncoding(encoding);

    public string Name { get; } = $"openai-{encoding}";

    public Task<long> CountAsync(string text, CancellationToken ct) => Task.FromResult((long)_tokenizer.CountTokens(text));
}

/// <summary>
/// Anthropic does not publish its tokenizers, so counts come from the free token-counting endpoint.
/// The endpoint counts a whole user message, which adds a few framing tokens. That overhead is measured once
/// with a one-letter message and subtracted, so the result is the cost of the text itself.
/// </summary>
public sealed class AnthropicCounter(AnthropicClient client, string model) : ITokenCounter
{
    private long? _overhead;

    public string Name => model;

    public async Task<long> CountAsync(string text, CancellationToken ct)
    {
        _overhead ??= await RawAsync("a", ct) - 1;
        return await RawAsync(text, ct) - _overhead.Value;
    }

    private async Task<long> RawAsync(string text, CancellationToken ct)
    {
        var result = await client.Messages.CountTokens(new MessageCountTokensParams
        {
            Model = model,
            Messages = [new UserMessageParam { Content = text }]
        }, cancellationToken: ct);
        return result.InputTokens;
    }
}
