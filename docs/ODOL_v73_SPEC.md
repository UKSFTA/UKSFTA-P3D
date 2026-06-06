# Arma 3 ODOL v73+ Technical Specification

*Documented by Platinum DevOps Suite - February 14, 2026*

## Overview

This document details the binary structure of the Arma 3 Object Data Layout (ODOL) format, specifically focusing on the changes introduced in version 73 and later (up to v75). These findings were derived from forensic hex audits, dynamic structural mapping, and regression testing against 18 production assets.

Prior to this documentation, the structure of v73+ models was largely undocumented in the open-source community, leading to widespread tool incompatibilities.

## 1. File Header & Versioning

The ODOL file begins with a standard 4-byte signature followed by the version number.

| Offset | Type | Description | Notes |
| :--- | :--- | :--- | :--- |
| `0x00` | `char[4]` | Signature | Must be "ODOL" |
| `0x04` | `uint32` | Version | v73, v74, v75 observed |

### Version-Specific Header Fields

*   **v59+**: `AppID` (uint32) follows the version.
*   **v58+**: `MuzzleFlash` (asciiz string) follows AppID.
*   **v74+**: Two unknown `uint32` fields appear after MuzzleFlash and before `nLods`.

## 2. ModelInfo Structure (v73 Changes)

The `ModelInfo` block contains global metadata about the model (mass, armor, skeleton, etc.). Version 73 introduces significant changes to field types and layout.

### 2.1 boolean -> int32 Transitions

In older versions (v6x), several flags were stored as single bytes (`bool`). In v73+, some of these have been promoted to 4-byte integers or are followed by padding.

*   **`hasAnims` Flag**:
    *   **Pre-v73**: `bool` (1 byte).
    *   **v73+**: `int32` (4 bytes).
    *   *Critical:* Reading this as a boolean causes a 3-byte misalignment that corrupts the entire subsequent stream.

### 2.2 New Fields

*   **`AICovers`**: A new boolean flag appearing after `CanBeOccluded`.
*   **`propertyLodDensityCoef`**: `float` (v70+).
*   **`propertyDrawImportance`**: `float` (v71+).
*   **`propertyExplosionShielding`**: `float` (v72+).

### 2.3 The "Mystery Shift" (Metadata Block)

Immediately following the `Animations` section (if present), or the `ModelInfo` block (if no animations), v73+ models contain a substantial block of unknown data that must be skipped to reach the LOD Address Table.

*   **Size**: 131 bytes (fixed).
*   **Structure**:
    1.  **String**: An empty or short ASCIIZ string.
    2.  **Floats**: A sequence of 12 `float` values (often 0.0 or small constants).
    3.  **Shadow Skip**: A 16-byte block (often `00` or `FF`).
    4.  **Model Floats**: A sequence of 11 `float` values.
    5.  **Terminator**: A single byte (often `0` or `1`).

**Synchronization Strategy**:
The parser implementation uses a "Dynamic Address Table Search" to verify this skip. It scans for a valid sequence of `uint32` offsets that point within the file bounds to confirm the start of the LOD table.

## 3. Skeleton & Bone Counts

*   **Bone Count Encoding**:
    *   Despite evidence of variable-length integer encoding (`CompactInteger`) in other parts of the engine, the `Skeleton` bone count in `ODOL` v73 remains a standard `int32`.
    *   *Correction:* Initial hypotheses about `CompactInteger` usage here were incorrect; the stream misalignment was actually due to the 4-byte `hasAnims` flag.

## 4. Animations Section

The `Animations` block is conditional, depending on the `hasAnims` flag.

*   **Structure**:
    *   `AnimationClass[]`: Array of animation definitions.
    *   `nAnimLODs`: `int32` count of animation LODs.
    *   `Bones2Anims`: Nested arrays mapping bones to animations.
    *   `Anims2Bones`: Nested arrays mapping animations to bones.
    *   **v73 Change**: The `AnimType` enum values appear consistent, but the parsing loop must strictly adhere to the `nAnimLODs` count.

## 5. LOD Address Table

This table dictates the file offsets for each Level of Detail (LOD) stored in the file.

*   **Layout**:
    1.  `StartAddresses`: `uint32[nLods]`
    2.  `EndAddresses`: `uint32[nLods]`
    3.  `PermanentFlags`: `bool[nLods]`

**Critical Synchronization Point**: Because of the variable-length nature of the preceding `ModelInfo` and `Animations` blocks, it is vital to "re-align" the stream before reading this table. The debinarizer implements a heuristic scanner that looks for monotonically increasing offsets to lock onto this table.

## 6. Compression & Arrays

*   **LZO Compression**: Standard for `ReadCompressedArray`.
*   **Condensed Arrays**: v73 continues to use condensed arrays (where a flag indicates if all elements are identical), but the count field remains a standard `int32`.

## 7. Migration Guide for Tool Developers

If you are updating a P3D tool for v73+ support:

1.  **Update `hasAnims`**: Switch from `ReadBoolean()` to `ReadInt32() != 0`.
2.  **Implement the Skip**: Add a 131-byte skip (or dynamic search) after the `ModelInfo`/`Animations` block.
3.  **Verify `ModelInfo`**: Ensure `propertyLodDensityCoef` and `propertyDrawImportance` are read.
4.  **Remove Obsolete Fields**: Ensure `pivotsNameObsolete` is **NOT** read for v73+.

---

*Generated by Gemini CLI - 2026-02-14*
