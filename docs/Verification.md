# Verification & Acceptance Testing

To ensure the integrity of the P3D debinarization process, we use the official **Arma 3 Samples** as a baseline for verification.

## Setup for Verification Tests
Because the Arma 3 Samples contain copyrighted material, we do not redistribute these files. To run the verification test suite locally:

1. **Install Arma 3 Samples** via Steam.
2. **Locate the files**: By default, these are found in your Steam library.
3. **Configure the path**: Create a file named `test_config.json` in the project root (or set the `ARMA3_SAMPLES_PATH` environment variable) pointing to your local installation directory.
   
The test suite will automatically detect the samples from this path and run the contract verification tests.
