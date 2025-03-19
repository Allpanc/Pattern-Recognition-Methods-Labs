namespace Lab2;

public static class DensityCalculator
{
    public static double CalculateDensity(double x, double y, double[] classMeans, double[,] covariance)
    {
        var meanX = classMeans[0];
        var meanY = classMeans[1];

        // Определитель ковариационной матрицы
        var determinant = GetDeterminant(covariance);

        // Обратная ковариационная матрица
        var invCovariance = GetInvertedCovariance(covariance, determinant);

        // Отклонения от среднего
        var dx = x - meanX;
        var dy = y - meanY;

        var quad =  GetQuad(dx, invCovariance, dy);

        var factor = 1.0 / (2.0 * Math.PI * Math.Sqrt(determinant));

        //  Плотность вероятности
        var density = factor * Math.Exp(-0.5 * quad);

        return density;
    }

    private static double GetDeterminant(double[,] covariance)
    {
        return covariance[0, 0] * covariance[1, 1] - covariance[0, 1] * covariance[1, 0];
    }

    private static double[,] GetInvertedCovariance(double[,] covariance, double determinant)
    {
        var invCovariance = new double[2, 2];
        
        invCovariance[0, 0] = covariance[1, 1] / determinant;
        invCovariance[0, 1] = -covariance[0, 1] / determinant;
        invCovariance[1, 0] = -covariance[1, 0] / determinant;
        invCovariance[1, 1] = covariance[0, 0] / determinant;
        return invCovariance;
    }

    private static double GetQuad(double dx, double[,] invCovariance, double dy)
    {
        return dx * (invCovariance[0, 0] * dx + invCovariance[0, 1] * dy) +
               dy * (invCovariance[1, 0] * dx + invCovariance[1, 1] * dy);
    }
}