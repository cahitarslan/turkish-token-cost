namespace TokenCost;

public sealed record LanguageCount(long Tokens, TextStats Stats)
{
    public double CharsPerToken => (double)Stats.Characters / Tokens;
    public double TokensPerWord => (double)Tokens / Stats.Words;
}

/// <summary>Token counts of one corpus under one tokenizer, in both languages.</summary>
public sealed record CorpusResult(
    string Corpus,
    string Counter,
    LanguageCount English,
    LanguageCount Turkish,
    ItemRatios? PerItem)
{
    /// <summary>How many Turkish tokens are needed for the same content that takes one English token.</summary>
    public double Ratio => (double)Turkish.Tokens / English.Tokens;
}

/// <summary>Spread of the Turkish/English ratio across individual texts (only computed when items are counted one by one).</summary>
public sealed record ItemRatios(double Min, double P25, double Median, double P75, double Max)
{
    public static ItemRatios From(IReadOnlyList<double> ratios)
    {
        if (ratios.Count == 0) throw new ArgumentException("No ratios.", nameof(ratios));
        var s = ratios.Order().ToArray();
        double At(double q) => s[(int)Math.Round(q * (s.Length - 1))];
        return new ItemRatios(s[0], At(0.25), At(0.5), At(0.75), s[^1]);
    }
}

public sealed class Measurement(IReadOnlyList<ITokenCounter> counters, int parallelism)
{
    /// <summary>
    /// Counts each corpus in both languages with every counter. The whole corpus is counted as one text
    /// (sentences joined by newlines), which is how a long prompt is billed. When <paramref name="perItemFor"/>
    /// accepts a corpus/counter pair, every text is also counted on its own to show how much the ratio varies.
    /// </summary>
    public async Task<List<CorpusResult>> RunAsync(
        IEnumerable<ParallelCorpus> corpora,
        Func<ParallelCorpus, ITokenCounter, bool> perItemFor,
        Action<string> log,
        CancellationToken ct)
    {
        var results = new List<CorpusResult>();
        foreach (var corpus in corpora)
        {
            var enText = corpus.Joined(Language.English);
            var trText = corpus.Joined(Language.Turkish);
            foreach (var counter in counters)
            {
                var en = new LanguageCount(await counter.CountAsync(enText, ct), TextStats.Of(enText));
                var tr = new LanguageCount(await counter.CountAsync(trText, ct), TextStats.Of(trText));
                var perItem = perItemFor(corpus, counter) ? ItemRatios.From(await PerItemRatiosAsync(corpus, counter, ct)) : null;
                var result = new CorpusResult(corpus.Name, counter.Name, en, tr, perItem);
                log($"{corpus.Name,-10} {counter.Name,-22} en {en.Tokens,8:N0}  tr {tr.Tokens,8:N0}  ratio {result.Ratio:F3}");
                results.Add(result);
            }
        }
        return results;
    }

    private async Task<double[]> PerItemRatiosAsync(ParallelCorpus corpus, ITokenCounter counter, CancellationToken ct)
    {
        var ratios = new double[corpus.Texts.Count];
        await Parallel.ForEachAsync(
            Enumerable.Range(0, corpus.Texts.Count),
            new ParallelOptions { MaxDegreeOfParallelism = parallelism, CancellationToken = ct },
            async (i, token) =>
            {
                var t = corpus.Texts[i];
                var en = await counter.CountAsync(t.English, token);
                var tr = await counter.CountAsync(t.Turkish, token);
                ratios[i] = (double)tr / en;
            });
        return ratios;
    }
}
