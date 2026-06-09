# UKSFTA P3D Debinarizer

A professional-grade, cross-platform debinarization toolkit for Arma 3. Converts binary **ODOL** models into editable **MLOD** format, providing tools for model forensics, audit, and manipulation within the UKSF Taskforce Alpha development pipeline.

## 🚀 Key Features

*   **Format Versatility**: Native support for ODOL v73, v74, and v75.
*   **Forensics & Auditing**:
    *   **Info**: Deep model diagnostics (mass, texture mapping, LOD data).
    *   **Audit**: Complexity auditing for high-performance simulation (missing PhysX, LOD counts).
    *   **Structure Discovery**: Binary structure map of ODOL chunks.
*   **Advanced Extraction**:
    *   **Material Extraction**: Converts embedded materials to standard `.rvmat` files.
    *   **Skeleton Export**: Generates `model.cfg` skeleton hierarchies for rigging.
    *   **Reference Validation**: Texture and material path audit against a defined mod root.
*   **Manipulation & Repair**:
    *   **Property Editor**: Batch update/delete model properties (`autocenter`, `mass`, etc.).
    *   **Sanity Fixes**: Automatic path normalization and trimming for engine compliance.
*   **Production UX**:
    *   Drag-and-drop support.
    *   Native multi-file GUI selection.
    *   PBO archive processing.

## 🛠 Infrastructure

- **Engine:** .NET 10.0 (C#)
- **Library:** `BIS.Core`, `BIS.P3D`, `BIS.PBO` (internal implementations).
- **Compliance:** Requires GPG signing for all commits.

## 📦 Usage

### GUI Mode
Double-click the `debinarizer` binary to open the native file picker. Supports multi-select via `Ctrl`/`Shift` clicking. Files will be converted in-place or into the specified output directory.

### CLI Mode
```bash
# Convert a single file
./debinarizer input.p3d

# Batch directory conversion
./debinarizer /path/to/models -out /path/to/output

# Extract materials and skeletons
./debinarizer /path/to/models -rvmat -skeleton

# Audit and validate
./debinarizer input.p3d -audit-lods -validate -root P:\
```

## 🔧 Command Reference

| Command | Description |
| :--- | :--- |
| `-out <dir>` | Specify target directory. |
| `-rvmat` | Extract embedded materials. |
| `-skeleton` | Export skeleton definition. |
| `-validate` | Check texture/material paths. |
| `-root <path>` | Root folder for path validation. |
| `-fix` | Apply path and name normalization. |
| `-info` | Show deep forensics. |
| `-map` | Map file structure. |

## ⚖ License

This project is licensed under the **Arma Public License - Share Alike (APL-SA)**.
