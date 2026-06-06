# Arma 3 ODOL Format Specification - v75

*Documented by Platinum DevOps Suite - June 6, 2026*

## Overview
ODOL v75 expands the metadata header block with thermal and mass distribution properties, increasing the size of the "Mystery Block".

## Header Structure
- Signature: `ODOL`
- Version: `75`
- AppID: `uint32`
- MuzzleFlash: `ASCIIZ`
- **Unknown_A**: `uint32`
- **Unknown_B**: `uint32`
- **PropertyMassDistribution**: `float[4]`
- **PropertyThermalSignature**: `float`

## Mystery Block
- Size: 147 bytes

| Segment | Size |
| :--- | :--- |
| Padding | 1 byte |
| Float Sequence | 12 floats |
| Shadow Skip | 16 bytes |
| Model Floats | 11 floats |
| New v75 Fields | 4 floats + 1 float |
| Terminator | 1 byte |
