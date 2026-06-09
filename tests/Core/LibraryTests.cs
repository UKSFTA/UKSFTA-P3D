using Xunit;
using BisDll.Common.Math;
using BisDll.Model;
using BisDll.Stream;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace P3DDebinarizer.Tests.Core;

public class LibraryTests
{
    private static string GetTestP3dPath()
    {
        // Navigate up to find test_p3ds folder
        var current = Directory.GetCurrentDirectory();
        while (current != null && !Directory.Exists(Path.Combine(current, "test_p3ds")))
        {
            current = Directory.GetParent(current)?.FullName;
        }
        return Path.Combine(current ?? "", "test_p3ds");
    }

    [Fact]
    public void Vector3P_Equality_Works()
    {
        var v1 = new Vector3P(1.0f, 2.0f, 3.0f);
        var v2 = new Vector3P(1.01f, 2.01f, 3.01f); // Within 0.05 tolerance
        var v3 = new Vector3P(1.1f, 2.0f, 3.0f);

        Assert.Equal(v1, v2);
        Assert.NotEqual(v1, v3);
    }

    [Theory]
    [MemberData(nameof(GetP3dFiles))]
    public void Parser_CanReadP3D_WithoutCrashing(string fileName)
    {
        string fullPath = Path.Combine(GetTestP3dPath(), fileName);

        // This is the core 'Success' test - can we load it?
        var exception = Record.Exception(() =>
        {
            using var fs = File.OpenRead(fullPath);
            BisDll.Model.P3D.GetInstance(fs);
        });

        Assert.Null(exception);
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
