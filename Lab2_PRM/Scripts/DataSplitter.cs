namespace Lab2;

public class DataSplitter
{
    public static (double[] train, double[] test) SplitData(double[] dataset, int splitThreshold)
    {
        if (dataset.Length < splitThreshold)
        {
            throw new Exception("The dataset is too small");
        }
        
        var train = dataset.Take(splitThreshold).ToArray();
        var test = dataset.Skip(splitThreshold).ToArray();

        return (train, test);
    }
}