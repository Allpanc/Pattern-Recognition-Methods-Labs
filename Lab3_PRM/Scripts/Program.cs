using MathNet.Numerics.LinearAlgebra.Double;

internal class Program
{
    private static void Main()
    {
        var xM1 = 6.1;
        var yM1 = 4.5;
        var B1varX = 3.1;
        var B1varY = 3.4;
        var B1covXY = 2.8;
        var B1covYX = 2.8;

        var xM2 = -4.2;
        var yM2 = 4.2;
        var B2varX = 2.2;
        var B2varY = 2.6;
        var B2covXY = -1.1;
        var B2covYX = -1.1;

        var xM3 = 2.5;
        var yM3 = -4;
        var B3varX = 2.6;
        var B3varY = 3.1;
        var B3covXY = 2.1;
        var B3covYX = 2.1;

        var M1 = Vector.Build.DenseOfArray(new[] { xM1, yM1 });
        var B1 = DenseMatrix.OfArray(new[,] { { B1varX, B1covXY }, { B1covYX, B1varY } });

        var M2 = Vector.Build.DenseOfArray(new[] { xM2, yM2 });
        var B2 = DenseMatrix.OfArray(new[,] { { B2varX, B2covXY }, { B2covYX, B2varY } });

        var M3 = Vector.Build.DenseOfArray(new[] { xM3, yM3 });
        var B3 = DenseMatrix.OfArray(new[,] { { B3varX, B3covXY }, { B3covYX, B3varY } });

        var train1 = new ClassData(SamplesGenerator.GenerateSamples(M1, B1, 150));
        var train2 = new ClassData(SamplesGenerator.GenerateSamples(M2, B2, 150));
        var train3 = new ClassData(SamplesGenerator.GenerateSamples(M3, B3, 150));

        var test1 = new ClassData(SamplesGenerator.GenerateSamples(M1, B1, 50));
        var test2 = new ClassData(SamplesGenerator.GenerateSamples(M2, B2, 50));
        var test3 = new ClassData(SamplesGenerator.GenerateSamples(M3, B3, 50));

        var clf12 = FisherClassifier.Train(train1, train2, 0, 1);
        var clf13 = FisherClassifier.Train(train1, train3, 0, 2);
        var clf23 = FisherClassifier.Train(train2, train3, 1, 2);

        var classifiers = new List<ClassifierParameters> { clf12, clf13, clf23 };
        var testSet = new List<ClassData> { test1, test2, test3 };

        var (fisherAccuracy, fisherError) = FisherClassifier.Evaluate(classifiers, testSet);

        foreach (ClassifierParameters clf in classifiers)
        {
            int idx1 = clf.ClassIndices.Item1;
            int idx2 = clf.ClassIndices.Item2;

            var w = clf.Weights;
            double threshold = clf.Threshold;

            Console.WriteLine($"Classifier between class {idx1 + 1} and class {idx2 + 1}:");

            Console.WriteLine($"  Weight vector: [{w[0]:F4}, {w[1]:F4}]");
            Console.WriteLine($"  Weight vector norm: {w.L2Norm():F4}");
            Console.WriteLine($"  Threshold: {threshold:F4}");
            Console.WriteLine();
        }
        
        Console.WriteLine($"Fisher effectiveness: {fisherAccuracy * 100:0.00}%");
        Console.WriteLine($"Fisher error: {fisherError * 100:0.00}%");

        PlotHelper.VisualizeFisher(testSet, classifiers);

        // Bayes
        var trainClasses = new List<ClassData> { train1, train2, train3 };
        double[] priors = { 1.0 / 3, 1.0 / 3, 1.0 / 3 }; // Равномерное априорное распределение

        var (bayesAccuracy, bayesError) = BayesClassifier.EvaluateClassifier(testSet, trainClasses, priors);

        Console.WriteLine($"Bayes effectiveness: {bayesAccuracy:P2}");
        Console.WriteLine($"Bayes error: {bayesError:P2}");
        
        PlotHelper.VisualizeBayes(
            classes: testSet,
            classPriors: new[] { 1.0 / 3, 1.0 / 3, 1.0 / 3 },
            xMin: -10, xMax: 10,
            yMin: -35, yMax: 35
        );
    }
}