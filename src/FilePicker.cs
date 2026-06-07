using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace P3DDebinarizer;

public static class FilePicker
{
    public static string[]? PickFiles()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return PickFilesWindows();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return PickFilesLinux();
        }
        return null;
    }

    private static string[]? PickFilesWindows()
    {
        var openFileName = new OpenFileName();
        openFileName.lStructSize = Marshal.SizeOf(openFileName);
        openFileName.lpstrFilter = "P3D Files\0*.p3d\0All Files\0*.*\0";
        char[] buffer = new char[4096];
        openFileName.lpstrFile = new string(buffer);
        openFileName.nMaxFile = 4096;
        openFileName.lpstrFileTitle = new string(new char[64]);
        openFileName.nMaxFileTitle = 64;
        openFileName.Flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000004 | 0x00000200; // OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST | OFN_HIDEREADONLY | OFN_ALLOWMULTISELECT

        if (GetOpenFileName(ref openFileName))
        {
            string[] parts = openFileName.lpstrFile.Split('\0', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                string dir = parts[0];
                return parts.Skip(1).Select(f => Path.Combine(dir, f)).ToArray();
            }
            return new string[] { openFileName.lpstrFile };
        }
        return null;
    }

    private static string[]? PickFilesLinux()
    {
        Console.WriteLine("DEBUG: Attempting to launch zenity...");
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "zenity",
                Arguments = "--file-selection --multiple --separator=\"|\" --title=\"Select P3D files\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            using var process = Process.Start(startInfo);
            if (process == null) { Console.WriteLine("DEBUG: Process null"); return null; }
            Console.WriteLine("DEBUG: Zenity started");
            string output = process.StandardOutput.ReadToEnd().Trim();
            string error = process.StandardError.ReadToEnd().Trim();
            process.WaitForExit();
            Console.WriteLine($"DEBUG: Zenity exited with {process.ExitCode}");
            if (!string.IsNullOrEmpty(error)) Console.WriteLine($"DEBUG: Zenity error: {error}");

            if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                return output.Split('|');
            }
            return null;
        }
        catch (Exception ex) { Console.WriteLine($"DEBUG: Exception {ex.Message}"); return null; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
    }

    [DllImport("comdlg32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref OpenFileName ofn);
}
