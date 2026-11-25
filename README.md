# turkish-token-cost

[English](README.en.md)

Aynı içerik Türkçe yazıldığında yapay zeka modelleri kaç token sayıyor, İngilizceye göre ne kadar fazla ödeniyor? Bu repo bunu ölçüyor.

Yazı: [Türkçe sormak yapay zekada yüzde 70 daha pahalı](https://cahitarslan.com/blog/turkce-token-maliyeti/)

## Sonuç (25 Kasım 2026)

FLORES-200 devtest: profesyonel çevirmenlerin İngilizceden Türkçeye çevirdiği 1.012 cümle. Oran, aynı içeriğin Türkçe token sayısının İngilizce token sayısına bölümü.

| Tokenizer | İngilizce token | Türkçe token | Oran | Türkçenin fazlası |
|---|--:|--:|--:|--:|
| Claude Haiku 4.5 | 29.894 | 58.028 | 1,94 | %94 |
| OpenAI cl100k_base (GPT-4) | 27.187 | 52.209 | 1,92 | %92 |
| Claude Haiku 5.5, Sonnet 5.5, Opus 5.5, Fable 5.2 | 42.927 | 72.950 | 1,70 | %70 |
| OpenAI o200k_base (GPT-4o ve sonrası) | 26.878 | 38.445 | 1,43 | %43 |

Bu test için iki dilde yazılan 24 kısa iş metninde (destek cevabı, e-posta, ürün ve fatura metni) sonuç aynı yönde: yeni Claude modellerinde %73, Haiku 4.5'te %95, GPT-4o'da %41.

Claude'un yeni tokenizer'ı İngilizceyi de daha küçük parçalara bölüyor (Haiku 4.5'te token başına 4,45 karakter, yeni modellerde 3,10). Bu yüzden modelleri karşılaştırırken token sayısına değil dolar maliyetine bakmak gerekiyor. Ayrıntılı tablo: [results/2026-11-25/report.md](results/2026-11-25/report.md).

## Nasıl çalışıyor

- `data/flores/`: FLORES-200 devtest İngilizce ve Türkçe dosyaları, satır satır hizalı.
- `data/business.json`: 24 kısa iş metni, iki dilde.
- OpenAI tokenizer'ları (`o200k_base`, `cl100k_base`) Microsoft.ML.Tokenizers ile tamamen yerel ve kesin sayılıyor.
- Anthropic tokenizer'ını yayınlamıyor. Claude sayıları Anthropic'in ücretsiz token sayma servisinden geliyor. Servis mesajın çerçevesi için birkaç token ekliyor; bu ek tek harflik bir mesajla bir kez ölçülüp çıkarılıyor.
- Bütün derlem tek metin olarak sayılıyor (uzun bir istem böyle faturalanır). Kısa metinlerde ayrıca her metin tek tek sayılıp oranın dağılımı veriliyor.

## Çalıştırma

.NET 10 gerekir.

```bash
dotnet test TokenCost.slnx
dotnet run --project src/TokenCost -- offline          # yalnız OpenAI tokenizer'ları, anahtar gerekmez
ANTHROPIC_API_KEY=... dotnet run --project src/TokenCost -- run   # Claude dahil; sayım ücretsiz, cevap üretilmez
```

Sonuçlar `results/<tarih>/` altına `results.json` ve `report.md` olarak yazılır.

## Lisans

Kod MIT. FLORES-200 verisi CC BY-SA 4.0 ([data/flores/README.md](data/flores/README.md)).
