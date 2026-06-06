# P3D Debinarizer (Platinum Edition)

A high-performance, cross-platform tool for converting Arma 3 **ODOL** (binarized) models to **MLOD** (editable) format.

## 🚀 New Features (v1.2.0)

*   **ODOL v73/v74/v75 Support**: Full support for the latest Arma 3 model formats.
*   **Production-Grade UX**: Drag-and-drop support, native file picker, and automatic batch output handling (`<FILENAME>_MLOD.p3d`).
*   **Batch Processing & Logging**: Dedicated output directories (`-out`), error logging (`error.log`), and success/failure summaries.
*   **Structure Discovery Map**: A powerful new diagnostic tool (`-map`).
*   **Path Normalization**: Built-in bulk renaming tool (`-rename`).

## 📦 Usage

```bash
# GUI File Picker (Double-click executable)
./debinarizer

# Basic conversion
./debinarizer input.p3d output.p3d

# Batch directory conversion with output folder
./debinarizer /path/to/models -out /path/to/output

# Generate Structure Discovery Map (for debugging)
./debinarizer input.p3d -map
```

## 📂 Project Structure

- `src/`: Source code.
- `tests/`: Unit tests.
- `test_p3ds/`: Test model files.

## 📚 Documentation

For detailed technical specifications of the ODOL formats (v73/v74/v75), see [docs/ODOL_v73_SPEC.md](docs/ODOL_v73_SPEC.md).

---
*Maintained by the UKSFTA Development Team*
