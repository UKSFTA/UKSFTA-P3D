using System;
using System.IO;
using System.Linq;
using BisDll.Model.ODOL;

namespace P3DDebinarizer;

public static class MaterialSerializer
{
    public static void ExportMaterial(EmbeddedMaterial material, string outputDirectory)
    {
        string fileName = material.materialName;
        if (string.IsNullOrEmpty(fileName)) return;
        
        // Ensure the filename ends with .rvmat
        if (!fileName.EndsWith(".rvmat", StringComparison.OrdinalIgnoreCase))
            fileName += ".rvmat";

        string filePath = Path.Combine(outputDirectory, fileName);
        
        // Ensure directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? outputDirectory);

        // Simple serialization based on known RVMAT structure
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("class _ARMA_DLL {");
            writer.WriteLine("    class Surface {");
            writer.WriteLine($"        material = \"{material.materialName}\";");
            writer.WriteLine("    };");
            writer.WriteLine("};");
            // Note: A full RVMAT serializer would need to map all fields in EmbeddedMaterial
            // This is a minimal implementation to generate a valid file.
        }
    }
}
