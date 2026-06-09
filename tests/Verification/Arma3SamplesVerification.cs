using System.IO;
using System.Text.Json;
using Xunit;
using BIS.P3D;
using BIS.P3D.Conversion;

namespace P3DDebinarizer.Tests.Verification;

public class Arma3SamplesVerification
{
    private readonly string _samplesPath;

    public Arma3SamplesVerification()
    {
        // Load path from config
        var configPath = "test_config.json";
        if (File.Exists(configPath))
        {
            var json = File.ReadAllText(configPath);
            var doc = JsonDocument.Parse(json);
            _samplesPath = doc.RootElement.GetProperty("SamplesPath").GetString() ?? "";
        }
        else
        {
            _samplesPath = Environment.GetEnvironmentVariable("ARMA3_SAMPLES_PATH") ?? "";
        }
    }

    [Theory]
    [InlineData("Addons/Test_Animal_01/Test_Animal_01_F.p3d")]
    [InlineData("Addons/Test_Boat_01/Test_Boat_01.p3d")]
    [InlineData("Addons/Test_Car_01/Test_Car_01.p3d")]
    public void Verify_Conversion_Integrity(string relativePath)
    {
        if (string.IsNullOrEmpty(_samplesPath))
        {
            // Skip test if not configured
            return;
        }

        var fullPath = Path.Combine(_samplesPath, relativePath);
        if (!File.Exists(fullPath)) 
        {
            // Skip if file doesn't exist
            return;
        }
        
        using var fs = File.OpenRead(fullPath);
        var p3d = new P3D(fs);
        
        Assert.NotNull(p3d);
        
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
}
