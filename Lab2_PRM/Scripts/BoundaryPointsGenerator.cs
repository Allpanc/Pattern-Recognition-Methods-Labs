namespace Lab2;

public static class BoundaryPointsGenerator
{
    public static List<(double, double)> GenerateBoundaryPointsAnalytical(int classI, int classJ,
        double[][] means, double[][,] covs, double[] priors,
        double xMin, double xMax, double step)
    {
        var boundaryPoints = new List<(double, double)>();

        for (var x = xMin; x <= xMax; x += step)
        {
            var (y1, y2) = CalculateBoundaryY(x, classI, classJ, means, covs, priors);

            if (!double.IsNaN(y1))
            {
                boundaryPoints.Add((x, y1));
            }
        }

        return boundaryPoints;
    }

    private static (double, double?) CalculateBoundaryY(double x, int classI, int classJ,
        double[][] means, double[][,] covs, double[] priors)
    {
        // Параметры дискриминантных функций
        var meanI = means[classI];
        var meanJ = means[classJ];
        var covI = covs[classI];
        var covJ = covs[classJ];
        var priorI = priors[classI];
        var priorJ = priors[classJ];

        // Определители
        var detI = GetDeterminant(covI);
        var detJ = GetDeterminant(covJ);

        // Обратные матрицы
        var covInvI = GetInvertedCovariance(covI, detI);
        var covInvJ = GetInvertedCovariance(covJ, detJ);

        //  Коэффициенты квадратичного уравнения
        var A = 0.5 * (covInvJ[0, 0] - covInvI[0, 0]);
        
        var B = 0.5 * (covInvJ[1, 1] - covInvI[1, 1]);
        
        var C = covInvJ[0, 1] - covInvI[0, 1];
        
        var D = covInvI[0, 0] * meanI[0] - covInvJ[0, 0] * meanJ[0] +
            covInvI[0, 1] * meanI[1] - covInvJ[0, 1] * meanJ[1];
        
        var E = covInvI[1, 1] * meanI[1] - covInvJ[1, 1] * meanJ[1] +
            covInvI[0, 1] * meanI[0] - covInvJ[0, 1] * meanJ[0];

        var F = Math.Log(priorI) - Math.Log(priorJ) - 0.5 * Math.Log(detI) + 0.5 * Math.Log(detJ);
        
        F += 0.5 * (meanI[0] * meanI[0] * covInvI[0, 0] + 2 * meanI[0] * meanI[1] * covInvI[0, 1] +
                    meanI[1] * meanI[1] * covInvI[1, 1]);
        
        F -= 0.5 * (meanJ[0] * meanJ[0] * covInvJ[0, 0] + 2 * meanJ[0] * meanJ[1] * covInvJ[0, 1] +
                    meanJ[1] * meanJ[1] * covInvJ[1, 1]);

        // Решение квадратного уравнения относительно y
        if (Math.Abs(B) < 1e-10)
        {
            // Особый случай: B ≈ 0
            if (Math.Abs(C * x + E) < 1e-10)
            {
                return (double.NaN, null); // Нет решения или вертикальная граница
            }

            return (-1 * (A * x * x + D * x + F) / (C * x + E), null);
        }

        // Стандартный случай
        var discriminant = Math.Pow(C * x + E, 2) - 4 * B * (A * x * x + D * x + F);

        if (discriminant < 0)
        {
            return (double.NaN, null); // Нет действительных решений
        }

        var y1 = (-1 * (C * x + E) + Math.Sqrt(discriminant)) / (2 * B);
        var y2 = (-1 * (C * x + E) - Math.Sqrt(discriminant)) / (2 * B);

        return (y1, y2);
    }

    private static double GetDeterminant(double[,] covariance)
    {
        var determinant = covariance[0, 0] * covariance[1, 1] - covariance[0, 1] * covariance[1, 0];
        
        if (Math.Abs(determinant) < 1e-10)
        {
            determinant = 1e-10;
        }
        
        return determinant;
    }

    private static double[,] GetInvertedCovariance(double[,] covariance, double determinant)
    {
        var invertedCovariance = new double[2, 2];
        
        invertedCovariance[0, 0] = covariance[1, 1] / determinant;
        invertedCovariance[0, 1] = -covariance[0, 1] / determinant;
        invertedCovariance[1, 0] = -covariance[1, 0] / determinant;
        invertedCovariance[1, 1] = covariance[0, 0] / determinant;
        
        return invertedCovariance;
    }
}