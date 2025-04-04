
internal static class ConsoleHelper
{
    // Печать популяции с заголовком
    public static void PrintPopulation(List<List<int>> population, string title)
    {
        Console.WriteLine("\n" + title);

        for (int i = 0; i < population.Count; i++)
        {
            Console.WriteLine($"Individual {i + 1}: {string.Join("", population[i])}");
        }
    }
    
    // Печать сформированных пар
    public static void PrintPairs(List<(int, int)> pairs)
    {
        Console.WriteLine("\nFormed pairs for crossover:");

        foreach ((int, int) pair in pairs)
        {
            Console.WriteLine($"({pair.Item1 + 1}, {pair.Item2 + 1})");
        }
    }
    
    // Табличный вывод оценки приспособленности
    public static void PrintFitnessTable(List<(List<int> Individual, int BitSum, double Fitness)> evaluated)
    {
        Console.WriteLine("\nFitness table:");
        Console.WriteLine("Individual | Chromosome  | Bit sum | Fitness");
        Console.WriteLine("------|------------|-----------|---------");

        for (int i = 0; i < evaluated.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1,5} | {string.Join("", evaluated[i].Individual),10} | {evaluated[i].BitSum,9} | {evaluated[i].Fitness,7:0.000}"
            );
        }
    }
    
    // Вывод отсортированных по убыванию фитнеса особей
    public static void PrintSortedByFitness(List<(int Index, List<int> Individual, double Fitness)> sorted)
    {
        Console.WriteLine("\nSorted individuals by fitness (descending):");
        Console.WriteLine("Rank | Original # | Chromosome  | Fitness");
        Console.WriteLine("-----|------------|-------------|--------");

        for (int i = 0; i < sorted.Count; i++)
        {
            var (index, individual, fitness) = sorted[i];
            Console.WriteLine($"{sorted.Count - i,4} | {index + 1,10} | {string.Join("", individual),11} | {fitness,6:0.000}");
        }
    }
    
    // Вывод потомков после кроссовера
    public static void PrintOffspring(List<(int, int)> pairs, List<List<int>> population, List<List<int>> offspring)
    {
        Console.WriteLine("\nSingle point crossover results:");

        for (int i = 0; i < pairs.Count; i++)
        {
            int p1 = pairs[i].Item1;
            int p2 = pairs[i].Item2;

            Console.WriteLine(
                $"Pair {i + 1}: Individual {p1 + 1} ({string.Join("", population[p1])}), Individual {p2 + 1} ({string.Join("", population[p2])})"
            );

            Console.WriteLine($"  Descendant 1: {string.Join("", offspring[i * 2])}");
            Console.WriteLine($"  Descendant 2: {string.Join("", offspring[i * 2 + 1])}");
        }
    }
    
    // Вывод результатов мутаций
    public static void PrintMutations(List<List<int>> before, List<List<int>> after)
    {
        Console.WriteLine("\nApplying mutation:");

        for (int i = 0; i < before.Count; i++)
        {
            string from = string.Join("", before[i]);
            string to = string.Join("", after[i]);

            if (from != to)
            {
                Console.WriteLine($"Mutation {i + 1}: {from} => {to}");
            }
            else
            {
                Console.WriteLine($"Unchanged {i + 1}: {from}");
            }
        }
    }
}