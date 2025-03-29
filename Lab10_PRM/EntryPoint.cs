class EntryPoint
{
    class Weights
    {
        public double w11;
        public double w12;
        public double w21;
        public double w22;
    }

    static double t = 0.8;
    const double k = 1;
    const int numberOfIterations = 1;
    const double learningCoef = 0.7;
    const int x1 = 1;
    const int x2 = 0;
    const int d1 = 0;
    const int d2 = 0;

    static Func<double, double> activation1 = s => s >= t ? 1 : 0;
    static Func<double, double> activation2 = s => 1 / (1 + Math.Exp(-k * s));

    static void Main()
    {
        Console.WriteLine($"Input: ({x1};{x2})");
        Console.WriteLine($"Target: ({d1};{d2})");

        var initialWeights = new Weights
        {
            w11 = 0.4,
            w12 = 0.5,
            w21 = 0.6,
            w22 = 0.7
        };
        
        List<Weights> weights = new List<Weights>
        {
            initialWeights
        };
        
        PrintWeights(initialWeights);

        for (int i = 0; i < numberOfIterations; i++)
        {
            Console.WriteLine($"Iteration: {i + 1}");
            
            var (e1,e2) = CalculateIterationParameters(weights[i]);

            var newWeights = new Weights
            {
                w11 = weights[i].w11 - learningCoef * e1 * x1,
                w12 = weights[i].w12 - learningCoef * e2 * x1,
                w21 = weights[i].w21 - learningCoef * e1 * x2,
                w22 = weights[i].w22 - learningCoef * e2 * x2
            };
            
            t -= learningCoef * e1;
            
            weights.Add(newWeights);
            
            PrintWeights(newWeights);
            
            Console.WriteLine();
        }
    }

    private static (double,double) CalculateIterationParameters(Weights weights)
    {
        double S1 = x1 * weights.w11 + x2 * weights.w21;
        double S2 = x1 * weights.w12 + x2 * weights.w22;
        double Y1 = activation1(S1);
        double Y2 = activation2(S2);
            
        Console.WriteLine($"\n{nameof(S1)}: {S1}" +
                          $"\n{nameof(S2)}: {S2}" +
                          $"\n{nameof(Y1)}: {Y1}" +
                          $"\n{nameof(Y2)}: {Y2}");

        double e1 = d1 - Y1;
        double e2 = d2 - Y2;
        double e = Math.Pow(e1, 2) + Math.Pow(e2, 2);
            
        Console.WriteLine($"\nErrors: \n{nameof(e1)}: {e1} \n{nameof(e2)}: {e2} \n{nameof(e)}: {e}");
        
        return (e1,e2);
    }

    static void PrintWeights(Weights weights)
    {
        Console.WriteLine($"\nWeights:" +
                          $"\n{nameof(weights.w11)}: {weights.w11} " +
                          $"\n{nameof(weights.w12)}: {weights.w12} " +
                          $"\n{nameof(weights.w21)}: {weights.w21} " +
                          $"\n{nameof(weights.w22)}: {weights.w22}\n");
    }
}