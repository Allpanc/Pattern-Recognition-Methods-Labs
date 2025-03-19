using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;

namespace Lab2;

public class RandomVectorSetsGenerator
{
    public static Matrix<double> CalculateA(Matrix<double> B)
    {
        var R00 = B[0, 0];
        var R01 = B[0, 1];
        var R11 = B[1, 1];

        var A00 = Math.Sqrt(R00);
        double A01 = 0;
        var A10 = R01 / A00;
        var A11 = Math.Sqrt(R11 - A10 * A10);

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

    public static Matrix<double> CalculateY(int N, Random random)
    {
        return Matrix<double>.Build.Dense(2, N, (i, j) => Normal.Sample(random, 0, 1));
    }

    public static Matrix<double> CalculateX(Matrix<double> A, Matrix<double> Y, int N, Vector<double> M)
    {
        var X = A * Y;

        // Из-за разницы типов данных, +M выполняется так
        for (var i = 0; i < N; i++) X.SetColumn(i, X.Column(i) + M);

        return X;
    }
}