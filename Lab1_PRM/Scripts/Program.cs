using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using ScottPlot;

internal class Program
{
    private static void Main()
    {
        var N = 200; // Количество генерируемых точек

        // Определяем три набора параметров

        var distributions = new (Vector<double> M, Matrix<double> B)[]
        {
            (
                Vector<double>.Build.DenseOfArray(new[] { 5.1, 3.7 }),
                Matrix<double>.Build.DenseOfArray(new[,] { { 4.1, 2.8 }, { 2.8, 4.4 } })
            ),
            (
                Vector<double>.Build.DenseOfArray(new[] { -4.2, 2.6 }),
                Matrix<double>.Build.DenseOfArray(new[,] { { 2.2, -0.6 }, { -1.1, 3.6 } })
            ),
            (
                Vector<double>.Build.DenseOfArray(new[] { 3.5, -3.5 }),
                Matrix<double>.Build.DenseOfArray(new[,] { { 2.6, 2.1 }, { 0.9, 4.1 } })
            )
        };
        
        Color[] plotColors =
        {
            Color.FromColor(System.Drawing.Color.Goldenrod), 
            Color.FromColor(System.Drawing.Color.DarkViolet), 
            Color.FromColor(System.Drawing.Color.DarkOliveGreen)
        };

        var commonPlot = new Plot();
        var random = new Random();

        for (var index = 0; index < distributions.Length; index++)
        {
            (Vector<double> M, Matrix<double> B) = distributions[index];
            try
            {
                var individualPlot = new Plot();
                // Проверяем, является ли матрица B положительно определённой
                ValidateMatrix(B);

                var A = CalculateA(B);

                var Y = CalculateY(N, random);

                // Генерация выборки X = A * Y + M
                var X = CalculateX(A, Y, N, M);

                var xValues = X.Row(0).ToArray();
                var yValues = X.Row(1).ToArray();

                ValidateCoordinateArrays(xValues, yValues);

                AddCoordinatesToPlot(individualPlot, xValues, yValues,  plotColors[index]);
                SaveResultPlot(individualPlot,$"Plot {index + 1}", $"График {index + 1}");

                AddCoordinatesToPlot(commonPlot, xValues, yValues, plotColors[index]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing the dataset: {ex.Message}");
            }
        }

        SaveResultPlot(commonPlot, "Common plot", "Три набора нормально распределенных векторов");
    }

    private static void ValidateMatrix(Matrix<double> B)
    {
        var eigen = B.Evd();
        var eigenValues = eigen.EigenValues;
        
        // Проверка собственных значений матрицы B на положительность
        if (eigenValues.Real().Any(v => v <= 0))
        {
            throw new Exception("Error: Matrix B is not positive definite.");
        }
    }

    private static void ValidateCoordinateArrays(double[] xValues, double[] yValues)
    {
        if (xValues.Length == 0 || yValues.Length == 0)
        {
            throw new Exception("Error: Empty data arrays.");
        }
    }

    private static Matrix<double> CalculateA(Matrix<double> B)
    {
        double R00 = B[0, 0];
        double R01 = B[0, 1];
        double R11 = B[1, 1];

        double A00 = Math.Sqrt(R00);
        double A01 = 0;
        double A10 = R01 / A00;
        double A11 = Math.Sqrt(R11 - A10 * A10);

        if (double.IsNaN(A00) || double.IsNaN(A10) || double.IsNaN(A11))
        {
            throw new Exception("Error: Invalid values in matrix A.");
        }

        var A = Matrix<double>.Build.DenseOfArray(new[,]
        {
            { A00, A01 },
            { A10, A11 }
        });
        
        return A;
    }

    private static Matrix<double> CalculateY(int N, Random random)
    {
        return Matrix<double>.Build.Dense(2, N, (i, j) => Normal.Sample(random, 0, 1));
    }

    private static Matrix<double> CalculateX(Matrix<double> A, Matrix<double> Y, int N, Vector<double> M)
    {
        var X = A * Y;
        
        // Из-за разницы типов данных, +M выполняется так
        for (var i = 0; i < N; i++)
        {
            X.SetColumn(i, X.Column(i) + M);
        }

        return X;
    }

    private static void AddCoordinatesToPlot(Plot plt, double[] xValues, double[] yValues, Color color)
    {
        var scatter = plt.Add.Scatter(xValues, yValues);
        scatter.MarkerSize = 10;
        scatter.Color = color;
        scatter.LineWidth = 0;
    }

    private static void SaveResultPlot(Plot plt, string filename, string title)
    {
        plt.Title(title);
        plt.XLabel("X");
        plt.YLabel("Y");
        SavePlotToPng(plt, filename);
    }

    private static void SavePlotToPng(Plot plt, string title)
    {
        plt.SavePng($"{PathProvider.ResourcesPath}\\{title}.png", 1200, 800);
    }
}