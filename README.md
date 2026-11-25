**Türkçe** | [English](README.en.md)

# Türkçe sormak daha mı çok token tutuyor?

Aynı içeriğin İngilizcesi ve Türkçesi, yapay zeka modellerinin tokenizer'larıyla sayılıyor. Oran = Türkçe token / İngilizce token.

## Sonuç (2026-11-25)

| Tokenizer | FLORES-200 | İş metinleri |
|---|--:|--:|
| Claude Haiku 4.5 | 1,94 | 1,95 |
| OpenAI cl100k_base (GPT-4) | 1,92 | 1,92 |
| Claude 5.x (Haiku 5.5, Sonnet 5.5, Opus 5.5, Fable 5.2) | 1,70 | 1,73 |
| OpenAI o200k_base (GPT-4o ve sonrası) | 1,43 | 1,41 |

Aynı içerik Türkçe yazılınca Claude 5.x modellerinde yaklaşık %70 daha fazla token tutuyor. Ayrıntılar: [results/2026-11-25/report.md](results/2026-11-25/report.md).

## Veri

- `data/flores/`: FLORES-200 devtest, İngilizce ve Türkçe, 1012 cümle ([CC BY-SA 4.0](data/flores/README.md)).
- `data/business.json`: Bu test için iki dilde yazılmış 24 kısa iş metni (destek cevabı, e-posta, ürün ve fatura metni).

## Çalıştırma

```
dotnet test TokenCost.slnx
dotnet run --project src/TokenCost -- offline      # yalnızca OpenAI tokenizer'ları, API anahtarı gerekmez
dotnet run --project src/TokenCost -- run          # Anthropic'in ücretsiz token sayma servisi de, ANTHROPIC_API_KEY gerekir
```

Anthropic tokenizer'ını yayınlamadığı için Claude sayıları ücretsiz token sayma servisinden geliyor; mesaj üretilmiyor, ücret çıkmıyor.

## Lisans

Kod MIT. FLORES-200 verisi CC BY-SA 4.0 (Meta / NLLB Team).
