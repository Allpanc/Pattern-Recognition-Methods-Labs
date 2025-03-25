using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using ScottPlot;

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

                // Матрица линейного преобразования
                var A = CalculateA(B);

                // Стандартный нормальный вектор, исходный шум
                var Y = CalculateY(N, random);

                // Генерация выборки X = A * Y + M
                // Результирующий нормально распределенный вектор
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