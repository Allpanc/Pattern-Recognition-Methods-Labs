using MathNet.Numerics.LinearAlgebra;

internal class FisherClassifier
{
    public static ClassifierParameters Train(ClassData class1, ClassData class2, int idx1, int idx2)
    {
        // Мат ожидания
        var M0 = class1.Mean;
        var M1 = class2.Mean;

        // Ковариационные матрицы
        var B0 = class1.Covariance;
        var B1 = class2.Covariance;

        // Весовой вектор по формуле 15
        var BCombined = (B0 + B1) * 0.5;
        var BInv = BCombined.Inverse();

        var weights = BInv * (M1 - M0);

        // Применение весовых коэф и нахождение дисперсии
        var proj0 = class1.Samples * weights;
        var proj1 = class2.Samples * weights;

        var sigma0Sq = Variance(proj0);
        var sigma1Sq = Variance(proj1);

        // Пороговое значение по формуле 16
        var weightedMeans = sigma1Sq * M0 + sigma0Sq * M1;
        var numerator = (M1 - M0).ToRowMatrix() * BInv * weightedMeans;
        var denominator = sigma1Sq + sigma0Sq;

        var threshold = -numerator[0] / denominator;

        return new ClassifierParameters(weights, threshold, (idx1, idx2));
    }

    public static int Predict(Vector<double> sample, List<ClassifierParameters> classifiers)
    {
        var votes = new int[3];

        foreach (var clf in classifiers)
        {
            var decision = sample.DotProduct(clf.Weights) + clf.Threshold;
            if (decision > 0)
                votes[clf.ClassIndices.Item2]++;
            else
                votes[clf.ClassIndices.Item1]++;
        }

        return Array.IndexOf(votes, votes.Max());
    }

    public static (double accuracy, double error) Evaluate(List<ClassifierParameters> classifiers,
        List<ClassData> testClasses)
    {
        var correct = 0;
        var total = 0;

        for (var i = 0; i < testClasses.Count; i++)
        {
            var samples = testClasses[i].Samples;
            for (var j = 0; j < samples.RowCount; j++)
            {
                var sample = samples.Row(j);
                var predicted = Predict(sample, classifiers);
                if (predicted == i) correct++;
                total++;
            }
        }

        var accuracy = (double)correct / total;
        var error = 1.0 - accuracy;
        return (accuracy, error);
    }

    private static double Variance(Vector<double> vector)
    {
        var mean = vector.Average();
        return vector.Select(v => (v - mean) * (v - mean)).Average();
    }
}