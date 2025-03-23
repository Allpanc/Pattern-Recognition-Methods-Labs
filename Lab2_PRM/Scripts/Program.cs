using MathNet.Numerics.LinearAlgebra;
using ScottPlot;
using ScottPlot.Plottables;

namespace Lab2;

internal class Program
{
    private static void Main()
    {
        var N = 200; // Количество генерируемых точек

        // Определяем три набора параметров

        var distributions = new (Vector<double> M, Matrix<double> B)[]
        {
            (
                Vector<double>.Build.DenseOfArray(new[] { 5.1, 3.5 }),
                Matrix<double>.Build.DenseOfArray(new[,] { { 4.1, 2.8 }, { 2.8, 4.4 } })
            ),
            (
                Vector<double>.Build.DenseOfArray(new[] { -4.2, 2.2 }),
                Matrix<double>.Build.DenseOfArray(new[,] { { 2.2, -0.6 }, { -1.1, 3.6 } })
            ),
            (
                Vector<double>.Build.DenseOfArray(new[] { 2.5, -2.5 }),
                Matrix<double>.Build.DenseOfArray(new[,] { { 2.6, 2.1 }, { 0.9, 4.1 } })
            )
        };

        Color[] plotColors =
        {
            Color.FromColor(System.Drawing.Color.Gold),
            Color.FromColor(System.Drawing.Color.DarkViolet),
            Color.FromColor(System.Drawing.Color.SaddleBrown)
        };
        
        Color[] plotBgColors =
        {
            Color.FromColor(System.Drawing.Color.PaleGoldenrod),
            Color.FromColor(System.Drawing.Color.Plum),
            Color.FromColor(System.Drawing.Color.RosyBrown)
        };

        var commonPlot = new Plot();
        var random = new Random();

        List<(double[] xValues, double[] yValues)> samples = new();
        
        for (var index = 0; index < distributions.Length; index++)
        {
            var (M, B) = distributions[index];
            try
            {
                var individualPlot = new Plot();
                // Проверяем, является ли матрица B положительно определённой
                Validator.ValidateMatrix(B);

                Matrix<double> A = RandomVectorSetsGenerator.CalculateA(B);

                Matrix<double> Y = RandomVectorSetsGenerator.CalculateY(N, random);

                // Генерация выборки X = A * Y + M
                Matrix<double> X = RandomVectorSetsGenerator.CalculateX(A, Y, N, M);

                double[]? xValues = X.Row(0).ToArray();
                double[]? yValues = X.Row(1).ToArray();
                
                samples.Add((xValues, yValues));

                Validator.ValidateCoordinateArrays(xValues, yValues);

                PlotHelper.AddCoordinatesToPlot(individualPlot, xValues, yValues, plotColors[index]);
                PlotHelper.SaveResultPlot(individualPlot, $"Plot {index + 1}", $"График {index + 1}");

                PlotHelper.AddCoordinatesToPlot(commonPlot, xValues, yValues, plotColors[index]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing the dataset: {ex.Message}");
            }
        }

        PlotHelper.SaveResultPlot(commonPlot, "Common plot", "Три набора нормально распределенных векторов");
        
        double[] xTrain = Array.Empty<double>();
        double[] yTrain = Array.Empty<double>();
        double[] xTest = Array.Empty<double>();
        double[] yTest = Array.Empty<double>();
        int[] labels = Array.Empty<int>();
        int[] trueLabels = Array.Empty<int>();

        var plt = new Plot();
        
        for (var index = 0; index < samples.Count; index++)
        {
            (double[] xValues, double[] yValues) sample = samples[index];
            var (xiTrain, xiTest) = DataSplitter.SplitData(sample.xValues, 150);
            var (yiTrain, yiTest) = DataSplitter.SplitData(sample.yValues, 150);

            xTrain = xTrain.Concat(xiTrain).ToArray();
            xTest = xTest.Concat(xiTest).ToArray();
            yTrain = yTrain.Concat(yiTrain).ToArray();
            yTest = yTest.Concat(yiTest).ToArray();
            labels = labels.Concat(Enumerable.Repeat(index, xiTrain.Length)).ToArray();
            trueLabels = trueLabels.Concat(Enumerable.Repeat(index, xiTest.Length)).ToArray();

            var scatter = plt.Add.Scatter(xiTest, yiTest, plotColors[index]);
            scatter.LineWidth = 0;
            scatter.MarkerSize = 10;
        }

        var (classMeans, classCovariances, classPriors) = TrainParametersCalculator.Train(xTrain, yTrain, labels, 3);

        Console.WriteLine("ClassMeans:");
        
        foreach (double[] classMean in classMeans)
        {
            Console.WriteLine($"{classMean[0]}, {classMean[1]}");
        }
        
        Console.WriteLine("ClassCovariances:");
        
        foreach (double[,] classCovariance in classCovariances)
        {
            Console.WriteLine($"{classCovariance[0,0]}, {classCovariance[0,1]}, {classCovariance[1,0]}, {classCovariance[1,1]}");
        }
        
        Console.WriteLine("ClassPriors:");
        
        foreach (double classPrior in classPriors)
        {
            Console.WriteLine(classPrior);
        }
        
        var (efficiency, error) = Classifier.EvaluateClassifier(
            xTest, yTest,trueLabels, classMeans, classCovariances, classPriors);

        // Выводим результаты
        Console.WriteLine($"Classification efficiency: {efficiency:P2}");
        Console.WriteLine($"Classification error: {error:P2}");
        Console.WriteLine($"Correctly classified: {efficiency * 150:F0} of 150");
        Console.WriteLine($"Incorrectly classified: {error * 150:F0} of 150");

        // сетка для визуализации границ принятия решений
        int gridSize = 100;
        double xMin = xTest.Min() - 2;
        double xMax = xTest.Max() + 2;
        double yMin = yTest.Min() - 2;
        double yMax = yTest.Max() + 2;

        double[] xGrid = new double[gridSize];
        double[] yGrid = new double[gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            xGrid[i] = xMin + i * (xMax - xMin) / (gridSize - 1);
            yGrid[i] = yMin + i * (yMax - yMin) / (gridSize - 1);
        }

        // границы принятия решений
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                double x = xGrid[i];
                double y = yGrid[j];
                int cls = Classifier.ClassifyNaiveBayes(x, y, classMeans, classCovariances, classPriors);
                
                if (cls == 0)
                {
                    var marker = plt.Add.Scatter(new[] { x }, new[] { y }, plotBgColors[0]);
                    marker.MarkerShape = MarkerShape.OpenSquare;
                    plt.MoveToBack(marker);
                }
                else if (cls == 1)
                {
                    var marker = plt.Add.Scatter(new[] { x }, new[] { y }, plotBgColors[1]);
                    marker.MarkerShape = MarkerShape.OpenSquare;
                    plt.MoveToBack(marker);
                }
                else
                {
                    var marker = plt.Add.Scatter(new[] { x }, new[] { y }, plotBgColors[2]);
                    marker.MarkerShape = MarkerShape.OpenSquare;
                    plt.MoveToBack(marker);
                }
            }
        }

        // Устанавливаем границы графика явно (фиксированные)
        plt.Axes.SetLimits(xMin, xMax, yMin, yMax);

        PlotHelper.SaveResultPlot(plt, "Result", "Result");
    }
}