# Arma 3 ODOL Format Technical Specification (v73-v75)

*Documented by Platinum DevOps Suite - June 6, 2026*

## 1. Introduction
This document defines the binary structure for Arma 3 ODOL (Object Data Layout) versions 73, 74, and 75. These versions introduce incremental metadata changes that require specific parsing logic to maintain stream alignment.

---

## 2. Global Header Structure
All versions share the same initial file header.

| Offset | Type | Description |
| :--- | :--- | :--- |
| `0x00` | `char[4]` | Signature ("ODOL") |
| `0x04` | `uint32` | Version (73, 74, or 75) |
| `0x08` | `uint32` | AppID |

---

## 3. Version-Specific Metadata Block
After the standard header (and `MuzzleFlash` string), newer versions include additional fields that must be read to keep the parser synchronized.

### v73 (Base v73+ Structure)
- `MuzzleFlash` (ASCIIZ)
- *Skip to LOD Table* (131-byte metadata/mystery block)

### v74
- `MuzzleFlash` (ASCIIZ)
- `Unknown_A` (uint32)
- `Unknown_B` (uint32)
- *Skip to LOD Table* (131-byte metadata/mystery block)

### v75
- `MuzzleFlash` (ASCIIZ)
- `Unknown_A` (uint32)
- `Unknown_B` (uint32)
- `PropertyMassDistribution` (float[4])
- `PropertyThermalSignature` (float)
- *Skip to LOD Table* (147-byte metadata/mystery block)

---

## 4. Metadata/Mystery Block Breakdown
The "Mystery Block" is a fixed-size section used for internal engine metadata. Its size increases to accommodate new fields in v75.

| Segment | v73/v74 | v75 | Description |
| :--- | :--- | :--- | :--- |
| Initial Padding | 1 byte | 1 byte | ASCIIZ terminator or padding |
| Float Sequence | 12 floats | 12 floats | World-space constants |
| Shadow/Padding | 16 bytes | 16 bytes | Reserved |
| Model Floats | 11 floats | 11 floats | Model-specific physics constants |
| New v75 Fields | - | 4 floats + 1 float | New thermal/mass distribution |
| Terminator | 1 byte | 1 byte | Block termination flag |

---

## 5. Synchronization Strategy
Because these fields are critical for parser alignment, a blind skip is risky.

**Recommended Implementation:**
1.  **Read Header** based on detected version.
2.  **Read ModelInfo** (Mass, Armor, etc.).
3.  **Read Animation Flags**.
4.  **Parse Version-Specific Fields** defined in Section 3.
5.  **Scan for LOD Table**: Do not rely on fixed-size skips. Implement a scanner that looks for the start of the LOD address table (a sequence of strictly increasing `uint32` offsets) to ensure the parser is perfectly aligned before attempting to read LOD data.
