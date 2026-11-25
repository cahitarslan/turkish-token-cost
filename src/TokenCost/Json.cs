using System.Text.Encodings.Web;
using System.Text.Json;

namespace TokenCost;

internal static class Json
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        // Keep Turkish characters readable in the result files
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}
