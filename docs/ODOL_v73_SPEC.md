# Arma 3 ODOL v73+ Technical Specification

*Documented by Platinum DevOps Suite - June 6, 2026*

## Overview

This document details the binary structure of the Arma 3 Object Data Layout (ODOL) format, specifically focusing on the changes introduced in versions 73, 74, and 75. These findings were derived from forensic hex audits, dynamic structural mapping, and regression testing against 18 production assets.

## 1. File Header & Versioning

The ODOL file begins with a standard 4-byte signature followed by the version number.

| Offset | Type | Description | Notes |
| :--- | :--- | :--- | :--- |
| `0x00` | `char[4]` | Signature | Must be "ODOL" |
| `0x04` | `uint32` | Version | v73, v74, v75 observed |

### Version-Specific Header Fields

*   **v74+**: Two unknown `uint32` fields appear after `MuzzleFlash` and before `nLods`.
*   **v75+**: Further extension of the metadata header block, requiring a 16-byte offset shift compared to v73 to reach the LOD table.

## 2. ModelInfo Structure

The `ModelInfo` block contains global metadata.

*   **v75+ Changes**: Added `propertyMassDistribution` (float array, size=4) and `propertyThermalSignature` (float). These fields MUST be parsed to ensure correct stream alignment in v75 assets.

## 3. The "Mystery Shift" & Synchronization

Immediately following the `Animations` section (if present), or the `ModelInfo` block, v73+ models contain a substantial block of data.

*   **v73/v74**: Size is 131 bytes.
*   **v75**: Size is 147 bytes due to the addition of v75-specific metadata fields.

**Synchronization Strategy**:
The parser uses a "Dynamic Address Table Search" to verify this skip. It scans for a valid sequence of `uint32` offsets that point within the file bounds to confirm the start of the LOD table. This is robust across v73, v74, and v75.
