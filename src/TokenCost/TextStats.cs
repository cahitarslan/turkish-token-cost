using System.Globalization;

namespace TokenCost;

public sealed record TextStats(long Characters, long Words, long Bytes)
{
    /// <summary>Characters are counted as text elements, so "ğ" is one character even if it is two code points.</summary>
    public static TextStats Of(string text)
    {
        var chars = 0L;
        var e = StringInfo.GetTextElementEnumerator(text);
        while (e.MoveNext()) chars++;
        var words = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).LongLength;
        return new TextStats(chars, words, System.Text.Encoding.UTF8.GetByteCount(text));
    }
}
