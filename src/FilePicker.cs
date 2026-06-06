using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace P3DDebinarizer;

public static class FilePicker
{
    public static string? PickFile()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return PickFileWindows();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return PickFileLinux();
        }
        return null;
    }

    private static string? PickFileWindows()
    {
        // Minimal P/Invoke for GetOpenFileName
        var openFileName = new OpenFileName();
        openFileName.lStructSize = Marshal.SizeOf(openFileName);
        openFileName.lpstrFilter = "P3D Files\0*.p3d\0All Files\0*.*\0";
        openFileName.lpstrFile = new string(new char[256]);
        openFileName.nMaxFile = 256;
        openFileName.lpstrFileTitle = new string(new char[64]);
        openFileName.nMaxFileTitle = 64;
        openFileName.Flags = 0x00080000 | 0x00001000 | 0x00000800; // OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST

        if (GetOpenFileName(ref openFileName))
        {
            return openFileName.lpstrFile;
        }
        return null;
    }

    private static string? PickFileLinux()
    {
        // Try Zenity as a lightweight, near-native GUI dependency on Linux
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "zenity",
                Arguments = "--file-selection --title=\"Select P3D file\"",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using var process = Process.Start(startInfo);
            return process?.StandardOutput.ReadToEnd().Trim();
        }
        catch
        {
            Console.WriteLine("Zenity not found. Please provide a file path as an argument.");
            return null;
        }
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
