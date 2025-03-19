namespace Lab2;

public static class Classifier
{
    public static (double Efficiency, double Error) EvaluateClassifier(
        double[] xTest, double[] yTest, int[] trueLabels,
        double[][] classMeans, double[][,] classCovariances, double[] classPriors)
    {
        var totalTestSamples = xTest.Length;
        var correctCount = 0;

        for (var i = 0; i < totalTestSamples; i++)
        {
            var trueClass = trueLabels[i];
            var predictedClass = ClassifyNaiveBayes(xTest[i], yTest[i], classMeans, classCovariances, classPriors);

            if (predictedClass == trueClass)
            {
                correctCount++;
            }
        }

        // Эффективность и ошибка классификации
        var efficiency = (double)correctCount / totalTestSamples;
        var error = 1.0 - efficiency;

        return (efficiency, error);
    }

    public static int ClassifyNaiveBayes(double x, double y, double[][] means, double[][,] covariances, double[] priors)
    {
        var l1 = DensityCalculator.CalculateDensity(x, y, means[0], covariances[0]);
        var l2 = DensityCalculator.CalculateDensity(x, y, means[1], covariances[1]);
        var l3 = DensityCalculator.CalculateDensity(x, y, means[2], covariances[2]);

        var lr12 = l1 * priors[0] / (l2 * priors[1]); // Отношение правдоподобия класс 1 к классу 2
        var lr13 = l1 * priors[0] / (l3 * priors[2]); // Отношение правдоподобия класс 1 к классу 3
        var lr23 = l2 * priors[1] / (l3 * priors[2]); // Отношение правдоподобия класс 2 к классу 3

        if (lr12 > 1 && lr13 > 1)
        {
            return 0; // Класс 1 выигрывает по отношению правдоподобия
        }

        if (lr12 < 1 && lr23 > 1)
        {
            return 1; // Класс 2 выигрывает по отношению правдоподобия
        }

        return 2; // Класс 3 выигрывает по отношению правдоподобия
    }
}