# Turkish vs English token counts

Measured 2026-11-25 20:04 UTC. Ratio = Turkish tokens / English tokens for the same content.

## flores

FLORES-200 devtest: Wikipedia-style sentences translated by professional translators (CC BY-SA 4.0). 1012 texts; English 21,901 words / 132,977 characters, Turkish 17,609 words / 136,772 characters.

| Tokenizer | English tokens | Turkish tokens | Ratio | Per-text median (min to max) | EN chars/token | TR chars/token | Extra cost for Turkish |
|---|--:|--:|--:|--:|--:|--:|--:|
| claude-haiku-4-5 | 29,894 | 58,028 | 1.94 | - | 4.45 | 2.36 | 94 % |
| openai-cl100k_base | 27,187 | 52,209 | 1.92 | 1.91 (1.00 to 3.57) | 4.89 | 2.62 | 92 % |
| claude-haiku-5-5 | 42,927 | 72,950 | 1.70 | - | 3.10 | 1.87 | 70 % |
| claude-sonnet-5-5 | 42,927 | 72,950 | 1.70 | - | 3.10 | 1.87 | 70 % |
| claude-opus-5-5 | 42,927 | 72,950 | 1.70 | - | 3.10 | 1.87 | 70 % |
| claude-fable-5-2 | 42,927 | 72,950 | 1.70 | - | 3.10 | 1.87 | 70 % |
| openai-o200k_base | 26,878 | 38,445 | 1.43 | 1.42 (0.82 to 2.45) | 4.95 | 3.56 | 43 % |

## business

Short business texts (support replies, e-mails, product and invoice text) written in both languages for this test. 24 texts; English 479 words / 2,752 characters, Turkish 369 words / 2,757 characters.

| Tokenizer | English tokens | Turkish tokens | Ratio | Per-text median (min to max) | EN chars/token | TR chars/token | Extra cost for Turkish |
|---|--:|--:|--:|--:|--:|--:|--:|
| claude-haiku-4-5 | 621 | 1,209 | 1.95 | 2.06 (1.56 to 2.33) | 4.43 | 2.28 | 95 % |
| openai-cl100k_base | 562 | 1,079 | 1.92 | 1.95 (1.60 to 2.30) | 4.90 | 2.56 | 92 % |
| claude-haiku-5-5 | 859 | 1,488 | 1.73 | 1.79 (1.36 to 2.57) | 3.20 | 1.85 | 73 % |
| claude-sonnet-5-5 | 859 | 1,488 | 1.73 | 1.79 (1.36 to 2.57) | 3.20 | 1.85 | 73 % |
| claude-opus-5-5 | 859 | 1,488 | 1.73 | 1.79 (1.36 to 2.57) | 3.20 | 1.85 | 73 % |
| claude-fable-5-2 | 859 | 1,488 | 1.73 | 1.79 (1.36 to 2.57) | 3.20 | 1.85 | 73 % |
| openai-o200k_base | 559 | 786 | 1.41 | 1.38 (1.15 to 1.95) | 4.92 | 3.51 | 41 % |

## Cost of the same content

USD for one million English words of content and its Turkish translation, read as input, on the FLORES corpus ratios. Output tokens cost more per token but the ratio is the same.

| Model | English | Turkish | Difference |
|---|--:|--:|--:|
| claude-haiku-5-5 | 0.20 | 0.33 | +0.14 |
| claude-sonnet-5-5 | 3.92 | 6.66 | +2.74 |
| claude-opus-5-5 | 7.84 | 13.32 | +5.48 |
| claude-fable-5-2 | 15.68 | 26.65 | +10.97 |
| claude-haiku-4-5 | 1.36 | 2.65 | +1.28 |
