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
        
        var distributions = new (Vector<double> M, Matrix<double> B)[]
        {
            (
                Vector<double>.Build.DenseOfArray(new[] { xM1, yM1 }),
                Matrix<double>.Build.DenseOfArray(new[,] { {B1varX , B1covXY }, { B1covYX, B1varY } })
            ),
            (
                Vector<double>.Build.DenseOfArray(new[] { xM2 , yM2 }),
                Matrix<double>.Build.DenseOfArray(new[,] { { B2varX , B2covXY }, { B2covYX, B2varY } })
            ),
            (
                Vector<double>.Build.DenseOfArray(new[] { xM3, yM3 }),
                Matrix<double>.Build.DenseOfArray(new[,] { { B3varX, B3covXY }, { B3covYX, B3varY} })
            )
        };

        Color[] plotColors =
        {
            Color.FromColor(System.Drawing.Color.Goldenrod), 
            Color.FromColor(System.Drawing.Color.DarkViolet), 
            Color.FromColor(System.Drawing.Color.DarkOliveGreen)
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

                // Матрица линейного преобразования
                Matrix<double> A = RandomVectorSetsGenerator.CalculateA(B);

                // Стандартный нормальный вектор, исходный шум
                Matrix<double> Y = RandomVectorSetsGenerator.CalculateY(N, random);

                // Генерация выборки X = A * Y + M
                // Результирующий нормально распределенный вектор
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
        int[] trainLabels = Array.Empty<int>();
        int[] testLabels = Array.Empty<int>();

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
            trainLabels = trainLabels.Concat(Enumerable.Repeat(index, xiTrain.Length)).ToArray();
            testLabels = testLabels.Concat(Enumerable.Repeat(index, xiTest.Length)).ToArray();

            var scatter = plt.Add.Scatter(xiTest, yiTest, plotColors[index]);
            scatter.LineWidth = 0;
            scatter.MarkerSize = 10;
        }

        var (classMeans, classCovariances, classPriors) = TrainParametersCalculator.Train(xTrain, yTrain, trainLabels, 3);

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
            xTest, yTest,testLabels, classMeans, classCovariances, classPriors);

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