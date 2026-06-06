# Arma 3 ODOL Format Specification - v73

*Documented by Platinum DevOps Suite - June 6, 2026*

## Overview
ODOL v73 is the baseline for the modern model formats used in Arma 3.

## Header Structure
- Signature: `ODOL`
- Version: `73`
- AppID: `uint32`
- MuzzleFlash: `ASCIIZ`

## Mystery Block
- Size: 131 bytes

| Segment | Size |
| :--- | :--- |
| Padding | 1 byte |
| Float Sequence | 12 floats |
| Shadow Skip | 16 bytes |
| Model Floats | 11 floats |
| Terminator | 1 byte |
