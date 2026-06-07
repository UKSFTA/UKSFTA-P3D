#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BisDll;
using BisDll.Stream;
using BisDll.Model;
using BisDll.Model.ODOL;
using BisDll.Model.MLOD;

namespace P3DDebinarizer;

internal sealed class Program
{
    private static bool _showInfo;
    private static bool _recursive;
    private static bool _showMap;
    private static bool _auditLods;
    private static bool _verbose;
    private static bool _exportRvmat;
    private static string? _oldPath;
    private static string? _newPath;

    [STAThread]
    private static int Main(string[] args)
    {
        List<string> inputs = new List<string>();
        string? outputDir = null;

        if (args.Length == 0)
        {
            string[]? pickedFiles = FilePicker.PickFiles();
            if (pickedFiles == null || pickedFiles.Length == 0)
            {
                Console.WriteLine("No files selected.");
                return 0;
            }
            inputs.AddRange(pickedFiles);
        }
        else if (args.Contains("--help", StringComparer.OrdinalIgnoreCase) || args.Contains("-h", StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("P3D Debinarizer - Arma 3 P3D to MLOD Converter");
            Console.WriteLine("Usage: debinarizer <input1> <input2> ... [options]");
            Console.WriteLine("\nArguments:");
            Console.WriteLine("  <input>           Paths to .p3d files or directories.");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  -out <dir>        Output directory for batch processing (optional).");
            Console.WriteLine("  -info             Show basic information about the P3D file.");
            Console.WriteLine("  -map              Show structure discovery map.");
            Console.WriteLine("  -rvmat            Export embedded materials to .rvmat files.");
            Console.WriteLine("  -audit-lods       Perform a performance audit on the LODs.");
            Console.WriteLine("  -v, --verbose     Enable verbose output.");
            Console.WriteLine("  -r, --recursive   Search for files recursively.");
            Console.WriteLine("  -rename <old> <new>  Remap texture paths.");
            Console.WriteLine("  -h, --help        Show this help message.");
            return 0;
        }
        else
        {
            // Parse arguments, excluding options
            _showInfo = args.Contains("-info", StringComparer.OrdinalIgnoreCase);
            _showMap = args.Contains("-map", StringComparer.OrdinalIgnoreCase);
            _exportRvmat = args.Contains("-rvmat", StringComparer.OrdinalIgnoreCase);
            _auditLods = args.Contains("-audit-lods", StringComparer.OrdinalIgnoreCase);
            _verbose = args.Contains("-v", StringComparer.OrdinalIgnoreCase) || args.Contains("--verbose", StringComparer.OrdinalIgnoreCase);
            _recursive = args.Contains("-r", StringComparer.OrdinalIgnoreCase) || args.Contains("--recursive", StringComparer.OrdinalIgnoreCase);

            // Handle -out and -rename options before collecting files
            int outIdx = Array.FindIndex(args, a => a.Equals("-out", StringComparison.OrdinalIgnoreCase));
            if (outIdx != -1 && args.Length > outIdx + 1)
            {
                outputDir = args[outIdx + 1];
                if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            }

            int renameIdx = Array.FindIndex(args, a => a.Equals("-rename", StringComparison.OrdinalIgnoreCase));
            if (renameIdx != -1 && args.Length > renameIdx + 2)
            {
                _oldPath = args[renameIdx + 1];
                _newPath = args[renameIdx + 2];
            }

            // Collect input files/dirs
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith('-'))
                {
                    if (args[i].Equals("-out", StringComparison.OrdinalIgnoreCase) || 
                        args[i].Equals("-rename", StringComparison.OrdinalIgnoreCase)) i++; // Skip option and next arg
                    continue;
                }
                inputs.Add(args[i]);
            }
        }

        int success = 0;
        int failure = 0;

        foreach (var input in inputs)
        {
            if (Directory.Exists(input))
            {
                var option = _recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                foreach (var file in Directory.EnumerateFiles(input, "*.p3d", option))
                {
                    string? outputPath = null;
                    if (outputDir != null)
                    {
                        outputPath = Path.Combine(outputDir, Path.GetFileName(file));
                    }
                    if (ProcessFile(file, outputPath)) success++;
                    else failure++;
                }
            }
            else if (File.Exists(input))
            {
                if (ProcessFile(input, null)) success++;
                else failure++;
            }
            else
            {
                Console.WriteLine($"[Error] Input path not found: {input}");
                failure++;
            }
        }

        Console.WriteLine($"\nSummary: {success} succeeded, {failure} failed.");
        return 0;
    }

    private static bool ProcessFile(string inputPath, string? outputPath)
    {
        if (outputPath == null)
        {
            string directory = Path.GetDirectoryName(inputPath) ?? "";
            string fileName = Path.GetFileNameWithoutExtension(inputPath);
            outputPath = Path.Combine(directory, $"{fileName}_MLOD.p3d");
        }

        try
        {
            using var stream = File.OpenRead(inputPath);
            var binaryReader = new BinaryReaderEx(stream);
            // binaryReader.Verbose = _verbose; // Temporarily commented out as it causes unexpected behavior elsewhere in the code
            try
            {
                var p3d = P3D.GetInstance(stream); // Pass stream, not binaryReader instance if needed
                if (p3d == null)
                {
                    Console.WriteLine($" [Warning] {inputPath}: Unsupported or unknown P3D format.");
                    return false;
                }

                if (_showInfo) DumpInfo(p3d, inputPath);
                if (_auditLods) AuditLods(p3d, inputPath);
                if (_showMap && p3d is ODOL odolMap) DumpStructureMap(binaryReader, odolMap);

                if (outputPath != null && p3d is ODOL odol)
                {
                    var mlod = BisDll.Model.Conversion.ODOL2MLOD(odol);

                    if (_oldPath != null && _newPath != null)
                    {
                        Console.WriteLine($" [*] Remapping paths: {_oldPath} -> {_newPath}");
                        foreach (var lod in mlod.LODs)
                        {
                            if (lod.Textures != null)
                            {
                                for (int j = 0; j < lod.Textures.Length; j++)
                                {
                                    if (lod.Textures[j].Contains(_oldPath, StringComparison.OrdinalIgnoreCase))
                                    {
                                        lod.Textures[j] = lod.Textures[j].Replace(_oldPath, _newPath, StringComparison.OrdinalIgnoreCase);
                                    }
                                }
                            }
                        }
                    }

                    mlod.writeToFile(outputPath, true);
                    Console.WriteLine($"[Success] {inputPath} -> {outputPath}");
                    
                    if (_exportRvmat && p3d is ODOL odolData && odolData.LODs != null)
                    {
                        var materialDir = Path.Combine(Path.GetDirectoryName(outputPath) ?? "", "materials");
                        foreach (var lod in odolData.LODs)
                        {
                            if (lod.Materials != null)
                            {
                                foreach (var mat in lod.Materials)
                                {
                                    MaterialSerializer.ExportMaterial(mat, materialDir);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                if (_verbose)
                {
                    Console.WriteLine("\n[Read Coverage Map on Failure]");
                    foreach (var c in binaryReader.Coverage)
                    {
                        Console.WriteLine($"  {c.Start:X8} - {c.End:X8} | {c.Label}");
                    }
                }
                if (_showMap) DumpStructureMap(binaryReader, null);
                if (_verbose) Console.WriteLine($"[Debug] Exception detail: {ex}");
                throw;
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText("error.log", $"[{DateTime.Now}] Error processing {inputPath}: {ex.Message}\n");
            Console.WriteLine($" [Error] {inputPath}: {ex.Message}");
            return false;
        }
    }

    private static void AuditLods(P3D p3d, string path)
    {
        Console.WriteLine($"\n[Performance Audit] {Path.GetFileName(path)}");
        bool hasGeometry = false;
        bool hasShadow = false;
        int visualLods = 0;

        if (p3d.LODs == null)
        {
            Console.WriteLine("  [!] No LOD data found.");
            return;
        }

        foreach (var lod in p3d.LODs)
        {
            if (lod == null) continue;
            float res = lod.Resolution;
            if (res == 1E+13f) hasGeometry = true;
            if (res >= 10000f && res < 20000f) hasShadow = true;
            if (res < 10000f) visualLods++;

            string name = lod.Name ?? res.ToString("F1");
            int points = lod.Points?.Length ?? 0;
            Console.WriteLine($"  - LOD {name,-15} | Vertices: {points,6}");
        }

        if (!hasGeometry) Console.WriteLine("  [!] MISSING GEOMETRY LOD (Server performance risk)");
        if (!hasShadow) Console.WriteLine("  [!] MISSING SHADOW VOLUME (Client performance risk)");
        if (visualLods < 2) Console.WriteLine("  [!] LOW LOD COUNT (Optimization risk)");
        Console.WriteLine("--------------------------------------------------\n");
    }

    private static void DumpInfo(P3D p3d, string path)
    {
        Console.WriteLine($"File: {Path.GetFileName(path)} (v{p3d.Version})");
        Console.WriteLine($"  Mass: {p3d.Mass:F2}");

        if (p3d.LODs == null)
        {
            Console.WriteLine("  LODs: 0");
            return;
        }

        Console.WriteLine($"  LODs: {p3d.LODs.Length}");
        var allTextures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var allSelections = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var allProxies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var lod in p3d.LODs)
        {
            if (lod == null) continue;
            string name = lod.Name ?? lod.Resolution.ToString("F1");
            int points = lod.Points?.Length ?? 0;
            int texturesCount = lod.Textures?.Length ?? 0;
            Console.WriteLine($"    - {name}: {points} pts, {texturesCount} textures");

            if (lod.Textures != null)
            {
                foreach (var t in lod.Textures) if (!string.IsNullOrWhiteSpace(t)) allTextures.Add(t);
            }
            try
            {
                if (lod.Selections != null)
                {
                    foreach (var s in lod.Selections) if (!string.IsNullOrWhiteSpace(s)) allSelections.Add(s);
                }
            }
            catch { }
            try
            {
                if (lod.Proxies != null)
                {
                    foreach (var p in lod.Proxies) if (!string.IsNullOrWhiteSpace(p)) allProxies.Add(p);
                }
            }
            catch { }
        }

        if (allTextures.Count > 0)
        {
            Console.WriteLine("\n  [VFS Links]");
            foreach (var t in allTextures.OrderBy(x => x)) Console.WriteLine($"    - {t}");
        }

        if (allSelections.Count > 0)
        {
            Console.WriteLine("\n  [Named Selections]");
            foreach (var s in allSelections.OrderBy(x => x)) Console.WriteLine($"    - {s}");
        }

        if (allProxies.Count > 0)
        {
            Console.WriteLine("\n  [Proxies]");
            foreach (var p in allProxies.OrderBy(x => x)) Console.WriteLine($"    - {p}");
        }
    }

    private static void DumpStructureMap(BinaryReaderEx reader, ODOL? odol)
    {
        Console.WriteLine("\n[Structure Discovery Map]");
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine($"{"Offset (Hex)",-12} | {"Size",-8} | {"Label"}");
        Console.WriteLine("--------------------------------------------------");

        var sorted = reader.Coverage.OrderBy(c => c.Start).ToList();
        long lastEnd = 0;

        foreach (var (start, end, label) in sorted)
        {
            if (start > lastEnd)
            {
                Console.WriteLine($"{lastEnd:X8}     | {(start - lastEnd),-8} | [GAP / UNKNOWN]");
            }
            Console.WriteLine($"{start:X8}     | {(end - start),-8} | {label}");
            lastEnd = Math.Max(lastEnd, end);
        }

        long fileSize = reader.BaseStream.Length;
        if (lastEnd < fileSize)
        {
            Console.WriteLine($"{lastEnd:X8}     | {(fileSize - lastEnd),-8} | [GAP / REMAINING]");
        }
        Console.WriteLine("--------------------------------------------------\n");
    }
}
