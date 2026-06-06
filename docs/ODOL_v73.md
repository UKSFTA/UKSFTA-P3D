# Arma 3 ODOL Format Specification - v73

*Documented by Platinum DevOps Suite - June 6, 2026*

## Overview
ODOL v73 is the baseline for modern Arma 3 model formats.

## 1. File Header
| Offset | Type | Description |
| :--- | :--- | :--- |
| `0x00` | `char[4]` | Signature ("ODOL") |
| `0x04` | `uint32` | Version (73) |
| `0x08` | `uint32` | AppID (e.g., Arma 3) |
| `0x0C` | `ASCIIZ` | MuzzleFlash String |

## 2. Mystery/Metadata Block
Immediately follows the MuzzleFlash string. 
- **Size**: 131 bytes

| Structure | Data Type | Notes |
| :--- | :--- | :--- |
| Padding | `byte` | Often `0x00` |
| World Floats | `float[12]` | Physics constants |
| Shadow Reserved| `byte[16]` | Reserved engine space |
| Model Floats | `float[11]` | Model-specific physics |
| Terminator | `byte` | Block termination |

## 3. Animation Section
Conditional based on `hasAnims` flag (int32).
- `nAnims` (int32)
- `AnimationClass[]`: 
    - `Name` (ASCIIZ)
    - `Type` (AnimType enum)
    - `Data` (Variable depending on AnimType)
- `Bones2Anims` (int[][])
- `Anims2Bones` (int[][])

## 4. LOD Address Table
- Located via heuristic scan for `uint32` offsets.
- `LODCount` (int32)
- `StartAddresses` (uint32[LODCount])
- `EndAddresses` (uint32[LODCount])
- `PermanentFlags` (bool[LODCount])

## Parsing Notes
- Strings are ASCIIZ (null-terminated).
- Compression is LZO/LZSS for array data.
- Endianness is Little-Endian.
