using System;
using System.IO;

namespace RetroPath.Tests.TestUtils;

public class Paths
{
    private static string RuntimeDir => Directory.GetCurrentDirectory();
    
    public static string TestProjectDir => RuntimeDir[..RuntimeDir.IndexOf("bin", StringComparison.Ordinal)];
    public static string TestDataDir => Path.Combine(TestProjectDir, "data");
}