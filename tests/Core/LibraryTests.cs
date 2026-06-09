using System.IO;
using Xunit;
using BIS.P3D;
using BIS.P3D.Conversion;

namespace P3DDebinarizer.Tests.Core;

public class LibraryTests
{
    private static string GetTestP3dPath()
    {
        var current = Directory.GetCurrentDirectory();
        while (current != null && !Directory.Exists(Path.Combine(current, "test_p3ds")))
        {
            current = Directory.GetParent(current)?.FullName;
        }
        return Path.Combine(current ?? "", "test_p3ds");
    }

    [Theory]
    [MemberData(nameof(GetP3dFiles))]
    public void Parser_Contract_ValidP3D_ProducesValidModel(string fileName)
    {
        string fullPath = Path.Combine(GetTestP3dPath(), fileName);
        if (!File.Exists(fullPath))
        {
            throw new Xunit.SkipException("Test data file not found: " + fullPath);
        }

        using var fs = File.OpenRead(fullPath);
        
        var p3d = new P3D(fs);
        
        Assert.NotNull(p3d);
        Assert.True(p3d.LODs.Any(), "A valid P3D must have at least one LOD");
        Assert.All(p3d.LODs, lod => {
            Assert.True(lod.Resolution >= 0, "LOD resolution must be non-negative");
            Assert.NotNull(lod.Points);
        });
    }

    [Theory]
    [MemberData(nameof(GetP3dFiles))]
    public void Conversion_Contract_ODOLtoMLOD_ProducesValidStructure(string fileName)
    {
        string fullPath = Path.Combine(GetTestP3dPath(), fileName);
        if (!File.Exists(fullPath))
        {
            throw new Xunit.SkipException("Test data file not found: " + fullPath);
        }
        
        using var fs = File.OpenRead(fullPath);
        
        var p3d = new P3D(fs);
        if (p3d.ODOL != null)
        {
            var mlod = ODOL2MLOD.Convert(p3d.ODOL);
            
            Assert.NotNull(mlod);
            Assert.True(mlod.Lods.Any(), "Converted MLOD must have at least one LOD");
            Assert.All(mlod.Lods, lod => {
                Assert.NotNull(lod.Faces);
                Assert.NotNull(lod.Points);
            });
        }
    }

    public static IEnumerable<object[]> GetP3dFiles()
    {
        var path = GetTestP3dPath();
        if (!Directory.Exists(path)) yield break;

        foreach (var file in Directory.EnumerateFiles(path, "*.p3d"))
        {
            yield return new object[] { Path.GetFileName(file) };
        }
    }
}
