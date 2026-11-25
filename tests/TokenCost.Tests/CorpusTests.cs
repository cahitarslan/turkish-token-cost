using TokenCost;

namespace TokenCost.Tests;

public sealed class CorpusTests : IDisposable
{
    private readonly string _dir = Directory.CreateTempSubdirectory("tokencost-").FullName;

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    private string Write(string name, params string[] lines)
    {
        var path = Path.Combine(_dir, name);
        File.WriteAllLines(path, lines);
        return path;
    }

    [Fact]
    public void Line_aligned_files_become_pairs_in_order()
    {
        var corpus = CorpusLoader.LoadLineAligned("t", "test", Write("en", "One.", "Two."), Write("tr", "Bir.", "İki."));

        Assert.Equal(2, corpus.Texts.Count);
        Assert.Equal("İki.", corpus.Texts[1].Turkish);
        Assert.Equal("One.\nTwo.", corpus.Joined(Language.English));
    }

    [Fact]
    public void Line_count_mismatch_is_rejected()
    {
        Assert.Throws<InvalidDataException>(() =>
            CorpusLoader.LoadLineAligned("t", "test", Write("en", "One.", "Two."), Write("tr", "Bir.")));
    }

    [Fact]
    public void Empty_line_is_rejected()
    {
        Assert.Throws<InvalidDataException>(() =>
            CorpusLoader.LoadLineAligned("t", "test", Write("en", "One.", "Two."), Write("tr", "Bir.", " ")));
    }

    [Fact]
    public void Business_set_in_the_repository_loads()
    {
        var path = Path.Combine(RepoRoot(), "data", "business.json");
        var corpus = CorpusLoader.LoadJson("business", "test", path);

        Assert.True(corpus.Texts.Count >= 20);
    }

    internal static string RepoRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "TokenCost.slnx"))) return dir.FullName;
        throw new DirectoryNotFoundException("TokenCost.slnx not found.");
    }
}
