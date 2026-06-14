# UKSFTA P3D Debinarizer

[![Build](https://github.com/UKSFTA/UKSFTA-P3D/actions/workflows/build.yml/badge.svg)](https://github.com/UKSFTA/UKSFTA-P3D/actions/workflows/build.yml)
[![Test](https://github.com/UKSFTA/UKSFTA-P3D/actions/workflows/test.yml/badge.svg)](https://github.com/UKSFTA/UKSFTA-P3D/actions/workflows/test.yml)
[![Lint](https://github.com/UKSFTA/UKSFTA-P3D/actions/workflows/lint.yml/badge.svg)](https://github.com/UKSFTA/UKSFTA-P3D/actions/workflows/lint.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-linux%20%7C%20windows-lightgrey)]()

Cross-platform debinarization toolkit for Arma 3. Converts binary **ODOL** models
(v73–v75) into editable **MLOD** format, with tools for forensics, auditing, and
batch processing.

Built on the [UKSFTA-BIS](https://github.com/UKSFTA/UKSFTA-BIS) format library.

## 🚀 Features

* **ODOL → MLOD conversion** — Full support for v73, v74, v75
* **Forensics & auditing** — Mass, texture mapping, LOD analysis, PhysX checks
* **Material extraction** — Embedded materials → standard `.rvmat` files
* **Skeleton export** — Generates `model.cfg` for rigging
* **Reference validation** — Texture and material path audit against a mod root
* **Property editing** — Batch update/delete model properties (`autocenter`, `mass`, etc.)
* **Sanity fixes** — Automatic path normalisation for engine compliance
* **GUI + CLI modes** — Native file picker with drag-and-drop, plus full CLI
* **PBO processing** — Opens PBO archives directly via the library

## 🛠 Setup

### Prerequisites
- .NET 10.0 SDK
- Git (for submodule)

### Clone & Build
```bash
git clone --recurse-submodules https://github.com/UKSFTA/UKSFTA-P3D.git
cd UKSFTA-P3D

# Or if already cloned:
git submodule update --init --recursive

# Build
./build.sh
# Or directly:
dotnet build P3DDebinarizer.sln
```

### Test
```bash
./dev.sh test
# Or:
dotnet test
```

## 📦 Usage

### GUI Mode
Launch the binary and select files via the native file picker. Supports
multi-select via Ctrl/Shift clicking and drag-and-drop.

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
| `-out <dir>` | Specify target directory |
| `-rvmat` | Extract embedded materials |
| `-skeleton` | Export skeleton definition |
| `-validate` | Check texture/material paths |
| `-root <path>` | Root folder for path validation |
| `-fix` | Apply path and name normalisation |
| `-info` | Show deep forensics |
| `-map` | Map file structure |
| `-recursive` | Process subdirectories |
| `-verbose` | Verbose output |

## 📚 Documentation

Detailed documentation for the P3D format and debinarisation process is
available in the `docs/` directory (a Git submodule pointing to the
[repository wiki](https://github.com/UKSFTA/UKSFTA-P3D/wiki)):

| Page | Description |
|---|---|
| [[P3D Format Overview]] | ODOL vs MLOD, model anatomy, LOD types, format history |
| [[ODOL Binary Format]] | Full byte-level specification for v73, v74, v75 |
| [[MLOD Text Format]] | Editable MLOD format structure and block reference |
| [[How Debinarisation Works]] | End-to-end conversion pipeline and data mapping |
| [[Architecture]] | Application structure and library dependencies |
| [[Verification]] | Test setup and acceptance testing with Arma 3 Samples |

## 🏗 Architecture

The application depends on three projects from the
[UKSFTA-BIS](https://github.com/UKSFTA/UKSFTA-BIS) format library
(via a Git submodule at `libs/UKSFTA-BIS/`):

| Library | Role |
|---|---|
| **BIS.Core** | Stream utilities, compression, config parsing, math types |
| **BIS.P3D** | ODOL/MLOD model parsing, skeleton, animations |
| **BIS.PBO** | PBO archive reading and file extraction |

Library documentation is available on the
[UKSFTA-BIS wiki](https://github.com/UKSFTA/UKSFTA-BIS/wiki).

The `docs/` directory is a Git submodule pointing to the
[project wiki](https://github.com/UKSFTA/UKSFTA-P3D/wiki) — update it with
`git submodule update --init docs`.

## 🙏 Acknowledgements

This project builds upon work from two upstream forks:

- **[rpgshooter/P3D-Debinarizer-Arma-3](https://github.com/rpgshooter/P3D-Debinarizer-Arma-3)** — The fork this repository was originally derived from.
- **[ScripyZz/P3D-Debinarizer-Arma-3](https://github.com/ScripyZz/P3D-Debinarizer-Arma-3)** — The original upstream project.
The code has been heavily modified since; all `<Authors>` metadata in `.csproj`
files now reflects **UKSFTA** as the current maintainer.

The [UKSFTA-BIS](https://github.com/UKSFTA/UKSFTA-BIS) library submodule builds
upon work from [jetelain/bis-file-formats](https://github.com/jetelain/bis-file-formats)
and [Braini01/bis-file-formats](https://github.com/Braini01/bis-file-formats).

## ⚖ License

This project is licensed under the **Arma Public License - Share Alike (APL-SA)**.
The UKSFTA-BIS library submodule is licensed under **MIT**.
