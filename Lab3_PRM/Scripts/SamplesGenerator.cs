using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

internal static class SamplesGenerator
{
    private static readonly Random random = new(2356);

    public static Matrix<double> GenerateSamples(Vector<double> mean, Matrix<double> cov, int count)
    {
        var samples = DenseMatrix.Create(count, mean.Count, (i, j) => 0.0);

        for (var i = 0; i < count; i++)
        {
            var sample = SampleMultivariateNormal(mean, cov);
            samples.SetRow(i, sample);
        }

        return samples;
    }

    private static Vector<double> SampleMultivariateNormal(Vector<double> mean, Matrix<double> cov)
    {
        var chol = cov.Cholesky().Factor;
        var stdNormal = Vector<double>.Build.Dense(mean.Count, _ => Normal.Sample(random, 0, 1));
        return mean + chol * stdNormal;
    }
}