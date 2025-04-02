
// Основной класс, реализующий генетический алгоритм
public class GeneticAlgorithm
{
    readonly GeneticConfig _config;
    readonly Random _random = new();

    public GeneticAlgorithm(GeneticConfig config)
    {
        _config = config;
    }

    // Генерация случайной популяции
    public List<List<int>> GenerateInitialPopulation()
    {
        return Enumerable
           .Range(start: 0, _config.PopulationSize)
           .Select(
                _ => Enumerable
                   .Range(start: 0, _config.IndividualLength)
                   .Select(_ => _random.Next(2))
                   .ToList()
            )
           .ToList();
    }

    // Печать популяции с заголовком
    public void PrintPopulation(List<List<int>> population, string title)
    {
        Console.WriteLine("\n" + title);

        for (int i = 0; i < population.Count; i++)
        {
            Console.WriteLine($"Особь {i + 1}: {string.Join("", population[i])}");
        }
    }

    // Расчёт суммы битов и фитнесс-функции
    public List<(List<int> Individual, int BitSum, double Fitness)> EvaluateFitness(List<List<int>> population)
    {
        return population
           .Select(ind => (ind, ind.Sum(), ind.Sum() / (double)_config.PopulationSize))
           .ToList();
    }

    // Табличный вывод оценки приспособленности
    public void PrintFitnessTable(List<(List<int> Individual, int BitSum, double Fitness)> evaluated)
    {
        Console.WriteLine("\nОценка приспособленности:");
        Console.WriteLine("Особь | Хромосома  | Сумма бит | Фитнесс");
        Console.WriteLine("------|------------|-----------|---------");

        for (int i = 0; i < evaluated.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1,5} | {string.Join("", evaluated[i].Individual),10} | {evaluated[i].BitSum,9} | {evaluated[i].Fitness,7:0.000}"
            );
        }
    }

    // Присваивание рангов по убыванию фитнеса
    public Dictionary<int, int> AssignRanks(List<(int Index, List<int> Individual, double Fitness)> sorted)
    {
        var ranks = new Dictionary<int, int>();

        for (int i = 0; i < sorted.Count; i++)
        {
            ranks[sorted[i].Index] = _config.PopulationSize - i;
        }

        return ranks;
    }

    // Создание пар для скрещивания на основе рангового отбора
    public List<(int, int)> CreatePairs(Dictionary<int, int> ranks)
    {
        int totalRank = ranks.Values.Sum();
        int numPairs = (_config.PopulationSize - 1) / 2;

        int Select()
        {
            int r = _random.Next(totalRank);
            int sum = 0;

            foreach (KeyValuePair<int, int> kv in ranks)
            {
                sum += kv.Value;

                if (r < sum)
                {
                    return kv.Key;
                }
            }

            return ranks.Keys.First();
        }

        var pairs = new List<(int, int)>();

        for (int i = 0; i < numPairs; i++)
        {
            int a = Select();
            int b;

            do
            {
                b = Select();
            }
            while (a == b);

            pairs.Add((a, b));
        }

        return pairs;
    }

    // Печать сформированных пар
    public void PrintPairs(List<(int, int)> pairs)
    {
        Console.WriteLine("\nСформированные пары для кроссовера:");

        foreach ((int, int) pair in pairs)
        {
            Console.WriteLine($"({pair.Item1 + 1}, {pair.Item2 + 1})");
        }
    }

    // Одноточечный кроссовер
    public List<List<int>> Crossover(List<List<int>> population, List<(int, int)> pairs)
    {
        var offspring = new List<List<int>>();

        foreach ((int i1, int i2) in pairs)
        {
            List<int> p1 = population[i1];
            List<int> p2 = population[i2];
            int point = _random.Next(minValue: 1, _config.IndividualLength - 1);

            List<int> child1 = p1
               .Take(point)
               .Concat(p2.Skip(point))
               .ToList();

            List<int> child2 = p2
               .Take(point)
               .Concat(p1.Skip(point))
               .ToList();

            offspring.Add(child1);
            offspring.Add(child2);
        }

        return offspring;
    }

    // Вывод потомков после кроссовера
    public void PrintOffspring(List<(int, int)> pairs, List<List<int>> population, List<List<int>> offspring)
    {
        Console.WriteLine("\nРезультаты одноточечного кроссовера:");

        for (int i = 0; i < pairs.Count; i++)
        {
            int p1 = pairs[i].Item1;
            int p2 = pairs[i].Item2;

            Console.WriteLine(
                $"Пара {i + 1}: Особь {p1 + 1} ({string.Join("", population[p1])}), Особь {p2 + 1} ({string.Join("", population[p2])})"
            );

            Console.WriteLine($"  Потомок 1: {string.Join("", offspring[i * 2])}");
            Console.WriteLine($"  Потомок 2: {string.Join("", offspring[i * 2 + 1])}");
        }
    }

    // Мутация: один случайный бит инвертируется с вероятностью
    public List<List<int>> MutateOffspring(List<List<int>> offspring)
    {
        return offspring
           .Select(
                child =>
                {
                    if (_random.NextDouble() < _config.MutationProbability)
                    {
                        var mutated = new List<int>(child);
                        int index = _random.Next(_config.IndividualLength);
                        mutated[index] = 1 - mutated[index];
                        return mutated;
                    }

                    return child;
                }
            )
           .ToList();
    }

    // Вывод результатов мутаций
    public void PrintMutations(List<List<int>> before, List<List<int>> after)
    {
        Console.WriteLine("\nПрименение мутации:");

        for (int i = 0; i < before.Count; i++)
        {
            string from = string.Join("", before[i]);
            string to = string.Join("", after[i]);

            if (from != to)
            {
                Console.WriteLine($"Мутация {i + 1}: {from} → {to}");
            }
            else
            {
                Console.WriteLine($"Без изменений {i + 1}: {from}");
            }
        }
    }
}
