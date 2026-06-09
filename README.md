# P3D Debinarizer (Platinum Edition)

A professional-grade, cross-platform tool for converting Arma 3 **ODOL** (binarized) models to **MLOD** (editable) format.

## Key Features

*   **Format Versatility**: Native support for ODOL v73, v74, and v75.
*   **Professional Toolkit**:
    *   **Forensics & Audits**: Detailed model analysis, complexity auditing, and binary structure mapping.
    *   **Material Extraction**: Converts embedded materials to standard `.rvmat` files.
    *   **Skeleton Export**: Generates `model.cfg` skeleton hierarchies for rigging.
    *   **Sanity & Repair**: Automatic path normalization and property manipulation.
*   **Production UX**:
    *   Drag-and-drop support (via CLI argument pass-through).
    *   Native multi-file selection via GUI (Windows/Linux).
    *   Batch processing with dedicated output directories.
    *   Comprehensive error logging (`error.log`).

## Usage

### 1. GUI Mode (Quick Start)
Simply double-click the `debinarizer` binary to open the native file picker. 
*   **Multi-Select**: Hold `Ctrl` or `Shift` to select multiple `.p3d` or `.pbo` files at once.
*   The tool will automatically convert them and save them with the `_MLOD.p3d` suffix in the same directory.

### 2. CLI Mode (Advanced)

```bash
# Convert a single file
./debinarizer input.p3d

# Batch directory conversion with output folder
./debinarizer /path/to/models -out /path/to/output

# Extract materials and skeletons for batch assets
./debinarizer /path/to/models -rvmat -skeleton

# Audit model performance and validate texture paths
./debinarizer input.p3d -audit-lods -validate -root P:\
```

## Advanced Options

| Option | Description |
| :--- | :--- |
| `-out <dir>` | Output directory for processed files. |
| `-root <path>` | Local mod root for path validation. |
| `-validate` | Validate texture/material paths against mod root. |
| `-rvmat` | Extract embedded materials to `.rvmat`. |
| `-skeleton` | Export skeleton hierarchy to `.cfg`. |
| `-info` | Show deep model forensics. |
| `-audit-lods`| Audit complexity and PhysX compatibility. |
| `-r` | Process directories recursively. |

---
*Maintained by the UKSFTA Development Team*
