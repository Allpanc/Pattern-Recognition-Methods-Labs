internal static class PathProvider
{
    public static string GeneratedPath => ResourcesPath + "\\Generated";
    public static string TestDatasetPath => DataPath + "\\svmdata3test.txt";
    public static string TrainDatasetPath => DataPath + "\\svmdata3.txt";
    private static string DataPath => ResourcesPath + "\\Data";
    
    private const string ResourcesPath = @"..\\..\\..\\Resources";
}