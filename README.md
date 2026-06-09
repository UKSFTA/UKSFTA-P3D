# P3D Debinarizer (Platinum Edition)

A professional-grade, cross-platform tool for converting Arma 3 **ODOL** (binarized) models to **MLOD** (editable) format.

## 🚀 Key Features

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
    *   Comprehensive error logging and success/failure summaries.
    *   Texture path validation against a defined mod root.

## 📦 Usage

### GUI Mode
Simply double-click the `debinarizer` binary (or run without arguments) to open the native file picker. You can hold `Ctrl` or `Shift` to select multiple files at once.

### CLI Mode
```bash
# Convert a single file
./debinarizer input.p3d

# Batch directory conversion with output folder
./debinarizer /path/to/models -out /path/to/output

# Export materials and skeletons for batch assets
./debinarizer /path/to/models -rvmat -skeleton

# Audit model performance and validate texture paths
./debinarizer input.p3d -audit-lods -validate -root P:\
```

## 📂 Project Structure

- `src/`: Core source code and utility modules.
- `tests/`: Unit and integration test suite.
- `docs/`: Technical specifications for ODOL versions.

## 📚 Technical Documentation

For in-depth technical specifications of the ODOL formats (v73-v75), see the documents in the `docs/` directory:
- [ODOL v73 Spec](docs/ODOL_v73.md)
- [ODOL v74 Spec](docs/ODOL_v74.md)
- [ODOL v75 Spec](docs/ODOL_v75.md)

---
*Maintained by the UKSFTA Development Team*
