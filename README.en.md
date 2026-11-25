# turkish-token-cost

[Türkçe](README.md)

How many tokens do AI models count when the same content is written in Turkish, and how much more does it cost than English? This repository measures it.

Post: [Asking AI in Turkish costs 70 percent more](https://cahitarslan.com/en/blog/turkish-token-cost/)

## Result (25 November 2026)

FLORES-200 devtest: 1,012 sentences translated from English to Turkish by professional translators. Ratio is Turkish tokens divided by English tokens for the same content.

| Tokenizer | English tokens | Turkish tokens | Ratio | Extra for Turkish |
|---|--:|--:|--:|--:|
| Claude Haiku 4.5 | 29,894 | 58,028 | 1.94 | 94% |
| OpenAI cl100k_base (GPT-4) | 27,187 | 52,209 | 1.92 | 92% |
| Claude Haiku 5.5, Sonnet 5.5, Opus 5.5, Fable 5.2 | 42,927 | 72,950 | 1.70 | 70% |
| OpenAI o200k_base (GPT-4o and later) | 26,878 | 38,445 | 1.43 | 43% |

On 24 short business texts written in both languages for this test (support replies, e-mails, product and invoice text) the result points the same way: 73% for the new Claude models, 95% for Haiku 4.5, 41% for GPT-4o.

Claude's newer tokenizer also splits English into smaller pieces (4.45 characters per token on Haiku 4.5, 3.10 on the new models). So compare models by dollar cost, not by token count. Full table: [results/2026-11-25/report.md](results/2026-11-25/report.md).

## How it works

- `data/flores/`: FLORES-200 devtest English and Turkish files, aligned line by line.
- `data/business.json`: 24 short business texts in both languages.
- OpenAI tokenizers (`o200k_base`, `cl100k_base`) are counted offline and exactly with Microsoft.ML.Tokenizers.
- Anthropic does not publish its tokenizer. Claude counts come from Anthropic's free token-counting endpoint. The endpoint adds a few tokens for the message framing; that overhead is measured once with a one-letter message and subtracted.
- Each corpus is counted as one text (how a long prompt is billed). For the short texts every text is also counted on its own to show the spread of the ratio.

## Running it

Requires .NET 10.

```bash
dotnet test TokenCost.slnx
dotnet run --project src/TokenCost -- offline          # OpenAI tokenizers only, no key needed
ANTHROPIC_API_KEY=... dotnet run --project src/TokenCost -- run   # includes Claude; counting is free, nothing is generated
```

Results are written to `results/<date>/` as `results.json` and `report.md`.

## License

Code is MIT. FLORES-200 data is CC BY-SA 4.0 ([data/flores/README.md](data/flores/README.md)).
