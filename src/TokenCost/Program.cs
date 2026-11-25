using System.Text.Json;
using Anthropic;
using TokenCost;

// Usage:
//   dotnet run --project src/TokenCost -- offline                       OpenAI tokenizers only, no API key needed
//   dotnet run --project src/TokenCost -- run [--models a,b] [--parallel 4] [--out results/<date>]
// "run" also uses Anthropic's free token-counting endpoint, so it needs ANTHROPIC_API_KEY but costs nothing.
var root = FindRoot();
var argsList = args.ToList();
var command = argsList.FirstOrDefault() ?? "help";
string Opt(string name, string fallback) => argsList.IndexOf($"--{name}") is var i and >= 0 && i + 1 < argsList.Count ? argsList[i + 1] : fallback;

if (command is not ("offline" or "run"))
{
    Console.WriteLine("Commands: offline | run [--models a,b] [--parallel 4] [--out results/<date>]");
    return command == "help" ? 0 : 2;
}

var corpora = new List<ParallelCorpus>
{
    CorpusLoader.LoadLineAligned(
        "flores",
        "FLORES-200 devtest: Wikipedia-style sentences translated by professional translators (CC BY-SA 4.0).",
        Path.Combine(root, "data", "flores", "eng_Latn.devtest"),
        Path.Combine(root, "data", "flores", "tur_Latn.devtest")),
    CorpusLoader.LoadJson(
        "business",
        "Short business texts (support replies, e-mails, product and invoice text) written in both languages for this test.",
        Path.Combine(root, "data", "business.json"))
};

var counters = new List<ITokenCounter> { new TiktokenCounter("o200k_base"), new TiktokenCounter("cl100k_base") };
if (command == "run")
{
    if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")))
    {
        Console.Error.WriteLine("Set ANTHROPIC_API_KEY first (token counting is free, but needs a key).");
        return 2;
    }
    var client = new AnthropicClient();
    var models = Opt("models", "claude-haiku-5-5,claude-sonnet-5-5,claude-opus-5-5,claude-fable-5-2,claude-haiku-4-5")
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    counters.InsertRange(0, models.Select(m => (ITokenCounter)new AnthropicCounter(client, m)));
}

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var measuredAt = DateTime.UtcNow;
// Per-text spread: always for the small business set; for FLORES only with the offline tokenizers (1,012 texts x 2 calls per model otherwise)
var results = await new Measurement(counters, int.Parse(Opt("parallel", "4"))).RunAsync(
    corpora,
    (corpus, counter) => corpus.Name == "business" || counter is TiktokenCounter,
    Console.WriteLine,
    cts.Token);

var outDir = Path.Combine(root, Opt("out", Path.Combine("results", measuredAt.ToString("yyyy-MM-dd"))));
Directory.CreateDirectory(outDir);
File.WriteAllText(Path.Combine(outDir, "results.json"), JsonSerializer.Serialize(new { measuredAt, results }, Json.Options));
File.WriteAllText(Path.Combine(outDir, "report.md"), Report.Build(results, corpora, measuredAt));
Console.WriteLine($"Wrote {Path.GetRelativePath(root, outDir)}/results.json and report.md");
return 0;

static string FindRoot()
{
    for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        if (File.Exists(Path.Combine(dir.FullName, "TokenCost.slnx"))) return dir.FullName;
    throw new DirectoryNotFoundException("Run from inside the repository (TokenCost.slnx not found).");
}
