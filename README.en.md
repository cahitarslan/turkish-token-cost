[Türkçe](README.md) | **English**

# Does asking in Turkish cost more tokens?

The same content in English and Turkish, counted with AI model tokenizers. Ratio = Turkish tokens / English tokens.

## Result (2026-11-25)

| Tokenizer | FLORES-200 | Business texts |
|---|--:|--:|
| Claude Haiku 4.5 | 1.94 | 1.95 |
| OpenAI cl100k_base (GPT-4) | 1.92 | 1.92 |
| Claude 5.x (Haiku 5.5, Sonnet 5.5, Opus 5.5, Fable 5.2) | 1.70 | 1.73 |
| OpenAI o200k_base (GPT-4o and later) | 1.43 | 1.41 |

The same content in Turkish takes about 70% more tokens on Claude 5.x models. Details: [results/2026-11-25/report.md](results/2026-11-25/report.md).

## Data

- `data/flores/`: FLORES-200 devtest, English and Turkish, 1,012 sentences ([CC BY-SA 4.0](data/flores/README.md)).
- `data/business.json`: 24 short business texts (support replies, e-mails, product and invoice text) written in both languages for this test.

## Run

```
dotnet test TokenCost.slnx
dotnet run --project src/TokenCost -- offline      # OpenAI tokenizers only, no API key needed
dotnet run --project src/TokenCost -- run          # also Anthropic's free token-counting endpoint, needs ANTHROPIC_API_KEY
```

Anthropic does not publish its tokenizer, so Claude counts come from the free token-counting endpoint; no messages are generated and nothing is billed.

## License

Code: MIT. FLORES-200 data: CC BY-SA 4.0 (Meta / NLLB Team).
