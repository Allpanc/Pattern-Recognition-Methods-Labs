using MathNet.Numerics.LinearAlgebra;

namespace Lab2;

public static class Validator
{
    public static void ValidateMatrix(Matrix<double> B)
    {
        var eigen = B.Evd();
        var eigenValues = eigen.EigenValues;

        // Проверка собственных значений матрицы B на положительность
        if (eigenValues.Real().Any(v => v <= 0))
        {
            throw new Exception("Error: Matrix B is not positive definite.");
        }
    }

    public static void ValidateCoordinateArrays(double[] xValues, double[] yValues)
    {
        if (xValues.Length == 0 || yValues.Length == 0)
        {
            throw new Exception("Error: Empty data arrays.");
        }
    }
}