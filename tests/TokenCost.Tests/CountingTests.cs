using TokenCost;

namespace TokenCost.Tests;

public sealed class CountingTests
{
    [Fact]
    public void Text_stats_count_turkish_letters_once()
    {
        var stats = TextStats.Of("Çağrı ışığı");

        Assert.Equal(11, stats.Characters);
        Assert.Equal(2, stats.Words);
        Assert.True(stats.Bytes > stats.Characters); // ç, ğ, ı and ş take two bytes in UTF-8
    }

    [Fact]
    public async Task Offline_tokenizer_counts_plain_english_as_expected()
    {
        var counter = new TiktokenCounter("o200k_base");

        Assert.Equal(2, await counter.CountAsync("hello world", CancellationToken.None));
    }

    [Fact]
    public async Task Ratio_is_turkish_over_english()
    {
        var corpus = new ParallelCorpus("t", "test", [new ParallelText("1", "Thank you very much.", "Çok teşekkür ederim.")]);
        var counter = new FixedCounter(new() { ["Thank you very much."] = 5, ["Çok teşekkür ederim."] = 8 });

        var results = await new Measurement([counter], 1).RunAsync([corpus], (_, _) => true, _ => { }, CancellationToken.None);

        var r = Assert.Single(results);
        Assert.Equal(1.6, r.Ratio, precision: 6);
        Assert.Equal(1.6, r.PerItem!.Median, precision: 6);
    }

    [Fact]
    public void Item_ratio_spread_uses_sorted_values()
    {
        var spread = ItemRatios.From([1.5, 1.0, 2.0, 1.2, 1.8]);

        Assert.Equal(1.0, spread.Min);
        Assert.Equal(1.5, spread.Median);
        Assert.Equal(2.0, spread.Max);
    }

    private sealed class FixedCounter(Dictionary<string, long> counts) : ITokenCounter
    {
        public string Name => "fixed";

        public Task<long> CountAsync(string text, CancellationToken ct) => Task.FromResult(counts[text]);
    }
}
