# Arma 3 ODOL Format Specification - v74

*Documented by Platinum DevOps Suite - June 6, 2026*

## Overview
ODOL v74 introduces two unknown `uint32` fields to the header block.

## Header Structure
- Signature: `ODOL`
- Version: `74`
- AppID: `uint32`
- MuzzleFlash: `ASCIIZ`
- **Unknown_A**: `uint32`
- **Unknown_B**: `uint32`

## Mystery Block
- Size: 131 bytes (Same as v73)

| Segment | Size |
| :--- | :--- |
| Padding | 1 byte |
| Float Sequence | 12 floats |
| Shadow Skip | 16 bytes |
| Model Floats | 11 floats |
| Terminator | 1 byte |
