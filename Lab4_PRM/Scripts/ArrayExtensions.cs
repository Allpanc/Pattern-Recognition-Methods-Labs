internal static class ArrayExtensions
{
    public static double[][] ToJagged(this double[,] rect)
    {
        var rows = rect.GetLength(0);
        var cols = rect.GetLength(1);
        var jagged = new double[rows][];
        
        for (var i = 0; i < rows; i++)
        {
            jagged[i] = new double[cols];
            
            for (var j = 0; j < cols; j++)
            {
                jagged[i][j] = rect[i, j];
            }
        }

        return jagged;
    }
}