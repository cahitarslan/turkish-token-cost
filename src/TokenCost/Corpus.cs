namespace TokenCost;

/// <summary>One text in two languages. Pairs are aligned: the Turkish text is a translation of the English one.</summary>
public sealed record ParallelText(string Id, string English, string Turkish);

/// <summary>A named set of parallel texts, for example FLORES-200 devtest or the business samples.</summary>
public sealed record ParallelCorpus(string Name, string Description, IReadOnlyList<ParallelText> Texts)
{
    public string Joined(Language lang) => string.Join('\n', Texts.Select(t => t.In(lang)));
}

public enum Language { English, Turkish }

public static class ParallelTextExtensions
{
    public static string In(this ParallelText text, Language lang) => lang == Language.English ? text.English : text.Turkish;
}

public static class CorpusLoader
{
    /// <summary>
    /// Loads two line-aligned files (one sentence per line, same order), as FLORES-200 ships them.
    /// Fails loudly on a line-count mismatch or an empty line: a silent misalignment would skew every ratio.
    /// </summary>
    public static ParallelCorpus LoadLineAligned(string name, string description, string englishPath, string turkishPath)
    {
        var en = File.ReadAllLines(englishPath);
        var tr = File.ReadAllLines(turkishPath);
        if (en.Length != tr.Length)
            throw new InvalidDataException($"{name}: {en.Length} English lines but {tr.Length} Turkish lines.");

        var texts = new List<ParallelText>(en.Length);
        for (var i = 0; i < en.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(en[i]) || string.IsNullOrWhiteSpace(tr[i]))
                throw new InvalidDataException($"{name}: line {i + 1} is empty in one of the languages.");
            texts.Add(new ParallelText($"{name}-{i + 1:D4}", en[i].Trim(), tr[i].Trim()));
        }
        return new ParallelCorpus(name, description, texts);
    }

    /// <summary>Loads a JSON array of { id, en, tr } objects.</summary>
    public static ParallelCorpus LoadJson(string name, string description, string path)
    {
        var items = System.Text.Json.JsonSerializer.Deserialize<List<JsonItem>>(File.ReadAllText(path), Json.Options)
                    ?? throw new InvalidDataException($"{path} is empty.");
        var texts = items.Select(i => string.IsNullOrWhiteSpace(i.En) || string.IsNullOrWhiteSpace(i.Tr)
                ? throw new InvalidDataException($"{name}: item {i.Id} is missing a language.")
                : new ParallelText(i.Id, i.En.Trim(), i.Tr.Trim()))
            .ToList();
        if (texts.Select(t => t.Id).Distinct().Count() != texts.Count)
            throw new InvalidDataException($"{name}: duplicate ids.");
        return new ParallelCorpus(name, description, texts);
    }

    private sealed record JsonItem(string Id, string En, string Tr);
}
