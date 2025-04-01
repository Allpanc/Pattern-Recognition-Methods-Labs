internal static class BayesClassifier
{
    public static (double Efficiency, double Error) EvaluateClassifier(
        List<ClassData> testClasses, List<ClassData> trainClasses, double[] classPriors)
    {
        var correct = 0;
        var total = 0;

        for (var classIndex = 0; classIndex < testClasses.Count; classIndex++)
        {
            var samples = testClasses[classIndex].Samples;

            for (var i = 0; i < samples.RowCount; i++)
            {
                var x = samples[i, 0];
                var y = samples[i, 1];

                var predicted = ClassifyNaiveBayes(x, y, trainClasses, classPriors);

                if (predicted == classIndex)
                    correct++;

                total++;
            }
        }

        var efficiency = (double)correct / total;
        var error = 1.0 - efficiency;
        return (efficiency, error);
    }

    public static int ClassifyNaiveBayes(double x, double y, List<ClassData> classDataList, double[] classPriors)
    {
        var likelihoods = new double[classDataList.Count];

        for (var i = 0; i < classDataList.Count; i++)
        {
            var mean = classDataList[i].Mean.ToArray();
            var cov = classDataList[i].Covariance.ToArray();
            likelihoods[i] = DensityCalculator.CalculateDensity(x, y, mean, cov) * classPriors[i];
        }

        // Возвращаем индекс класса с максимальной апостериорной вероятностью
        return Array.IndexOf(likelihoods, likelihoods.Max());
    }
}