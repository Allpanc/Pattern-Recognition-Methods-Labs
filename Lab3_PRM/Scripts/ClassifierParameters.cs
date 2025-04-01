using MathNet.Numerics.LinearAlgebra;

internal class ClassifierParameters
{
    public ClassifierParameters(Vector<double> weights, double threshold, (int, int) indices)
    {
        Weights = weights;
        Threshold = threshold;
        ClassIndices = indices;
    }

    public Vector<double> Weights { get; }
    public double Threshold { get; }
    public (int, int) ClassIndices { get; }
}