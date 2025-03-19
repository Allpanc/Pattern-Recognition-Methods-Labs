using MathNet.Numerics.LinearAlgebra;

namespace Lab2;

public static class DistributionParametersEstimator
{
    public static (double[] mean, double[,] covarianceMatrix) EstimateDistributionParameters(double[] x, double[] y)
    {
        // оценка математических ожиданий
        var meanX = DirectMeanEstimation(x);
        var meanY = DirectMeanEstimation(y);

        // оценка ковариационной матрицы
        var covMatrix = DirectCovarianceMatrixEstimation(x, y);

        // вектор математического ожидания
        double[] meanVector = { meanX, meanY };

        Console.WriteLine($"Оценка математического ожидания: ({meanX:F4}, {meanY:F4})");
        Console.WriteLine("Оценка ковариационной матрицы:");
        Console.WriteLine($"  [{covMatrix[0, 0]:F4}, {covMatrix[0, 1]:F4}]");
        Console.WriteLine($"  [{covMatrix[1, 0]:F4}, {covMatrix[1, 1]:F4}]");

        return (meanVector, covMatrix);
    }
    
    private static double DirectMeanEstimation(double[] sample)
    {
        double sum = 0;
        for (var i = 0; i < sample.Length; i++) sum += sample[i];
        return sum / sample.Length;
    }

    private static double[,] DirectCovarianceMatrixEstimation(double[] x, double[] y)
    {
        var n = x.Length;
        // средние значения для x и y
        double meanX = 0, meanY = 0;
        for (var i = 0; i < n; i++)
        {
            meanX += x[i];
            meanY += y[i];
        }

        meanX /= n;
        meanY /= n;

        // элементы ковариационной матрицы
        double covXX = 0; // дисперсия x
        double covYY = 0; // дисперсия y
        double covXY = 0; // ковариация между x и y

        for (var i = 0; i < n; i++)
        {
            var diffX = x[i] - meanX;
            var diffY = y[i] - meanY;

            covXX += diffX * diffX;
            covYY += diffY * diffY;
            covXY += diffX * diffY;
        }

        // делить на количество наблюдений
        covXX /= n;
        covYY /= n;
        covXY /= n;

        var covMatrix = new double[2, 2];
        covMatrix[0, 0] = covXX; // дисперсия x
        covMatrix[1, 1] = covYY; // дисперсия y
        covMatrix[0, 1] = covXY; // ковариация между x и y
        covMatrix[1, 0] = covXY;

        return covMatrix;
    }
    
    // Оценка параметров нормального распределения
    public static (Vector<double> m, Matrix<double> b) EstimateNormalDistributionParams(double[] x, double[] y)
    {
        //  Математического ожидания
        Vector<double> m = Vector<double>.Build.Dense(2);
        m[0] = x.Average();
        m[1] = y.Average();
    
        //  Ковариационной матрицы
        Matrix<double> b = Matrix<double>.Build.Dense(2, 2);
        for (int i = 0; i < x.Length; i++)
        {
            double dx = x[i] - m[0];
            double dy = y[i] - m[1];
        
            b[0, 0] += dx * dx;
            b[0, 1] += dx * dy;
            b[1, 0] += dx * dy;
            b[1, 1] += dy * dy;
        }
    
        b = b / x.Length;
        return (m, b);
    }
}