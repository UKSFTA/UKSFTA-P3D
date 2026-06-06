# P3D Debinarizer (Platinum Edition)

A high-performance, cross-platform tool for converting Arma 3 **ODOL** (binarized) models to **MLOD** (editable) format. This "Platinum Edition" has been extensively modernized to support the latest Arma 3 binary standards.

## 🚀 New Features (v1.2.0)

*   **ODOL v73/v74/v75 Support**: Full support for the latest Arma 3 model formats, including correct parsing of 4-byte animation flags and metadata blocks.
*   **Structure Discovery Map**: A powerful new diagnostic tool (`-map`) that generates a detailed report of the binary file structure, invaluable for debugging corrupt or unknown model versions.
*   **Self-Healing Parser**: Implements dynamic stream synchronization to automatically recover from version-specific binary shifts.
*   **Path Normalization**: Built-in bulk renaming tool (`-rename`) to migrate texture and material paths (e.g., from `a3\` to `z\project\`).
*   **Linux-x64 Native**: Fully compatible with Linux environments, powered by .NET 10.0.

## 📦 Usage

```bash
# Basic conversion
./debinarizer input.p3d output.p3d

# Recursive directory conversion
./debinarizer /path/to/models /path/to/output -r

# Generate Structure Discovery Map (for debugging)
./debinarizer input.p3d -map

# Convert and Rename Paths (e.g., re-basing a project)
./debinarizer input.p3d output.p3d -rename "P:\old\path" "z\new\path"
```

## 🔧 Technical Details

This tool uses a modernized `BisDll` core that includes:
- **LZO/LZSS Decompression**: Native C# implementation of Bohemia Interactive's compression algorithms.
- **Precision Floating Point**: Correct handling of `Vector3P` and compressed vector formats.
- **Dynamic LOD Discovery**: Heuristic algorithms to locate LOD address tables even when header data is variable-length.

## 🛠 Building from Source

Requirements: **.NET 10.0 SDK**

```bash
# Fast development build
./build.sh

# Production release (single-file, self-contained)
./build.sh --release
```

## 📚 Documentation

For detailed technical specifications of the ODOL v73 format discovered during development, see [docs/ODOL_v73_SPEC.md](docs/ODOL_v73_SPEC.md).

---
*Maintained by the UKSFTA Development Team*
