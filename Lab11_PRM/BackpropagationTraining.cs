class BackpropagationTraining
{
    static Func<double, double> firstLayerActivationFunc = S => S >= T ? 1 : 0;
    static Func<double, double> secondLayerActivationFunc = S => Math.Tanh(S/k);
    static Func<double, double> secondLayerFirstDerivative = Y => (1 - Y * Y);
    
    const double T = 0.6;
    const double k = 2.0;
    const double eta = 0.7;
    const int itetations = 1;
    
    static void Main()
    {
        // Обучающая выборка: X1, X2, D
        double[,] inputs =
        {
            {0, 1, 0},
            {1, 0, 0},
            {1, 1, 0}
        };

        // Инициализация весов
        double w11 = 0.4, w12 = 0.5;
        double w21 = 0.6, w22 = 0.7;
        double w1 = 0.8, w2 = 0.9;

        for (int iteration = 1; iteration <= itetations; iteration++)
        {
            Console.WriteLine($"\n=== Iteration {iteration} ===\n");

            for (int i = 0; i < inputs.GetLength(0); i++)
            {
                double x1 = inputs[i, 0];
                double x2 = inputs[i, 1];
                double D = inputs[i, 2];

                // Прямой проход
                double S1 = x1 * w11 + x2 * w21;
                double S2 = x1 * w12 + x2 * w22;
                double Y1 = firstLayerActivationFunc(S1);
                double Y2 = firstLayerActivationFunc(S2);
                double S3 = Y1 * w1 + Y2 * w2;
                double Y = secondLayerActivationFunc(S3);

                // Ошибка и дельта
                double error = D - Y;
                double delta2 = error * secondLayerFirstDerivative(Y);

                // Дельты скрытого слоя
                double delta1_1 = delta2 * w1;
                double delta2_1 = delta2 * w2;
                
                // Обновление весов выходного слоя
                w1 += eta * delta2 * Y1;
                w2 += eta * delta2 * Y2;
                
                // Обновление весов скрытого слоя
                w11 += eta * delta1_1 * x1;
                w12 += eta * delta2_1 * x1;
                w21 += eta * delta1_1 * x2;
                w22 += eta * delta2_1 * x2;

                // Квадратичная ошибка
                double loss = Math.Pow(error, 2);
                
                Console.WriteLine($"Training Sample {i + 1}: X = {{{x1}, {x2}}}, Target D = {D}");
                Console.WriteLine($"  Hidden neuron outputs: Y1 = {Y1}, Y2 = {Y2}");
                Console.WriteLine($"  Network output: Y = {Y:F4}");
                Console.WriteLine($"  Error = {error:F4}, Loss = {loss:F4}");
                Console.WriteLine($"  Updated Weights:");
                Console.WriteLine($"    w1  = {w1:F4}, w2  = {w2:F4}");
                Console.WriteLine($"    w11 = {w11:F4}, w12 = {w12:F4}");
                Console.WriteLine($"    w21 = {w21:F4}, w22 = {w22:F4}\n");
            }
        }

        Console.WriteLine("=== TRAINING COMPLETE ===");
        Console.WriteLine($"Final weights:");
        Console.WriteLine($"  w1  = {w1:F4}, w2  = {w2:F4}");
        Console.WriteLine($"  w11 = {w11:F4}, w12 = {w12:F4}");
        Console.WriteLine($"  w21 = {w21:F4}, w22 = {w22:F4}");
    }
}
