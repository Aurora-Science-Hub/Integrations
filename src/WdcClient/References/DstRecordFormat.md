# WDC Dst record format: 2026-11-04 snapshot

Source: [World Data Center for Geomagnetism, Kyoto — Dst format](https://wdc.kugi.kyoto-u.ac.jp/dstae/format/dstformat.html).

Retrieved: **2026-11-04 at 10:22 UTC**.

This Markdown snapshot preserves the field layout from the source page. Descriptions below are paraphrased.
Each record occupies **120 bytes**. Column numbers below start at one and include both endpoints.

| Columns | Fortran format | Meaning |
| --- | --- | --- |
| 1–3 | `A3` | Identifier: `DST` |
| 4–5 | `I2` | Year within the century |
| 6–7 | `I2` | Calendar month |
| 8 | `A1` | Index marker: `*` |
| 9–10 | `I2` | Calendar day |
| 11–12 | `A2` | Blank field, or `RR` on quick-look records |
| 13 | `A1` | Index value marker: `X` |
| 14 | `A1` | Revision code: `0` quick-look, `1` provisional, `2` final, `3` or higher corrected final. A blank is also permitted |
| 15–16 | `I2` | Century: `19` or blank denotes the 1900s. Records from 2000 use `20` |
| 17–20 | `I4` | Baseline component, measured in hundreds of nT |
| 21–116 | `24I4` | Twenty-four hourly components, each four characters wide and measured in nT. The sentinel `9999` indicates an unavailable measurement |
| 117–120 | `I4` | Daily average in nT. The sentinel `9999` also applies here |

Hourly components run chronologically from the day's first hour to its last hour.
The daily average is a separate field and must not become a twenty-fifth hourly record.

## Mapping used by WdcClient

The following are client mappings, separate from the source layout:

- WDC column 21 becomes offset `20` in a zero-based character span.
- Hour number `h`, counted from zero, starts at offset `20 + h * 4` and spans four characters.
- A present hourly component combines with the baseline as `baseline * 100 + hourlyComponent` in nT.
- The missing sentinel maps to `null` before the baseline is applied.
- Hourly timestamps mark the UTC interval start: `00:00` through `23:00`.
- The client ignores the daily average.

Retain this dated snapshot when the provider changes its website or format. Record a new snapshot when adapting the parser to a new layout.
