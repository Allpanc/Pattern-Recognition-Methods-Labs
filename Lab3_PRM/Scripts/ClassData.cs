using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

internal class ClassData
{
    public ClassData(Matrix<double> samples)
    {
        Samples = samples;
        Mean = samples.ColumnSums() / samples.RowCount;
        Covariance = CovarianceMatrix(samples);
    }

    public Matrix<double> Samples { get; }
    public Vector<double> Mean { get; }
    public Matrix<double> Covariance { get; }

    private Matrix<double> CovarianceMatrix(Matrix<double> data)
    {
        var mean = data.ColumnSums() / data.RowCount;
        var centered = data - DenseMatrix.Create(data.RowCount, data.ColumnCount, (i, j) => mean[j]);
        return centered.TransposeThisAndMultiply(centered) / (data.RowCount - 1);
    }
}