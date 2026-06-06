# Arma 3 ODOL Format Specification - v75

*Documented by Platinum DevOps Suite - June 6, 2026*

## Overview
ODOL v75 adds specific thermal and mass distribution properties.

## 1. File Header
| Offset | Type | Description |
| :--- | :--- | :--- |
| `0x00` | `char[4]` | Signature ("ODOL") |
| `0x04` | `uint32` | Version (75) |
| `0x08` | `uint32` | AppID |
| `0x0C` | `ASCIIZ` | MuzzleFlash String |
| `0xXX` | `uint32` | Unknown_A |
| `0xXX` | `uint32` | Unknown_B |
| `0xXX` | `float[4]` | PropertyMassDistribution |
| `0xXX` | `float` | PropertyThermalSignature |

## 2. Mystery/Metadata Block
- **Size**: 147 bytes

| Structure | Data Type | Notes |
| :--- | :--- | :--- |
| Padding | `byte` | Often `0x00` |
| World Floats | `float[12]` | Physics constants |
| Shadow Reserved| `byte[16]` | Reserved engine space |
| Model Floats | `float[11]` | Model-specific physics |
| Thermal/Mass Data | `float[5]` | v75-specific metadata |
| Terminator | `byte` | Block termination |

## 3. Animation Section
Identical to v73.

## 4. LOD Address Table
Identical to v73.
