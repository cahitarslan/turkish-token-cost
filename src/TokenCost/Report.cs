using System.Globalization;
using System.Text;

namespace TokenCost;

public static class Report
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static string Build(IReadOnlyList<CorpusResult> results, IReadOnlyList<ParallelCorpus> corpora, DateTime measuredAtUtc)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Turkish vs English token counts");
        sb.AppendLine();
        sb.AppendLine(Inv, $"Measured {measuredAtUtc:yyyy-MM-dd HH:mm} UTC. Ratio = Turkish tokens / English tokens for the same content.");
        sb.AppendLine();
        foreach (var corpus in corpora)
        {
            var rows = results.Where(r => r.Corpus == corpus.Name).ToList();
            if (rows.Count == 0) continue;
            var first = rows[0];
            sb.AppendLine(Inv, $"## {corpus.Name}");
            sb.AppendLine();
            sb.AppendLine(Inv, $"{corpus.Description} {corpus.Texts.Count} texts; English {first.English.Stats.Words:N0} words / {first.English.Stats.Characters:N0} characters, Turkish {first.Turkish.Stats.Words:N0} words / {first.Turkish.Stats.Characters:N0} characters.");
            sb.AppendLine();
            sb.AppendLine("| Tokenizer | English tokens | Turkish tokens | Ratio | Per-text median (min to max) | EN chars/token | TR chars/token | Extra cost for Turkish |");
            sb.AppendLine("|---|--:|--:|--:|--:|--:|--:|--:|");
            foreach (var r in rows.OrderByDescending(r => r.Ratio))
            {
                var spread = r.PerItem is { } p ? string.Create(Inv, $"{p.Median:F2} ({p.Min:F2} to {p.Max:F2})") : "-";
                sb.AppendLine(Inv,
                    $"| {r.Counter} | {r.English.Tokens:N0} | {r.Turkish.Tokens:N0} | {r.Ratio:F2} | {spread} | {r.English.CharsPerToken:F2} | {r.Turkish.CharsPerToken:F2} | {r.Ratio - 1:P0} |");
            }
            sb.AppendLine();
        }

        sb.AppendLine("## Cost of the same content");
        sb.AppendLine();
        sb.AppendLine("USD for one million English words of content and its Turkish translation, read as input, on the FLORES corpus ratios. Output tokens cost more per token but the ratio is the same.");
        sb.AppendLine();
        sb.AppendLine("| Model | English | Turkish | Difference |");
        sb.AppendLine("|---|--:|--:|--:|");
        foreach (var r in results.Where(r => r.Corpus == corpora[0].Name && Pricing.For(r.Counter) is not null))
        {
            var price = Pricing.For(r.Counter)!;
            var enTokens = r.English.TokensPerWord * 1_000_000;
            var trTokens = enTokens * r.Ratio;
            var en = (decimal)enTokens / 1_000_000m * price.InputPerMillion;
            var tr = (decimal)trTokens / 1_000_000m * price.InputPerMillion;
            sb.AppendLine(Inv, $"| {r.Counter} | {en:F2} | {tr:F2} | +{tr - en:F2} |");
        }
        return sb.ToString();
    }
}
