
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
    
    // Генерация заранее заданной популяции
    public List<List<int>> GetPredefinedPopulation(string[] populationStrings)
    {
        return populationStrings
            .Select(individual => individual.Select(bit => bit - '0').ToList())
            .ToList();
    }

    // Расчёт суммы битов и фитнесс-функции
    public List<(List<int> Individual, int BitSum, double Fitness)> EvaluateFitness(List<List<int>> population)
    {
        return population
           .Select(ind => (ind, ind.Sum(), ind.Sum() / (double)_config.PopulationSize))
           .ToList();
    }

    // Присваивание рангов: выше фитнес — выше номер ранга
    public Dictionary<int, int> AssignRanks(List<(int Index, List<int> Individual, double Fitness)> sorted)
    {
        var ranks = new Dictionary<int, int>();
        int currentRank = sorted.Count; // Начинаем с максимального ранга

        foreach (var item in sorted)
        {
            ranks[item.Index] = currentRank--;
        }

        return ranks;
    }

    // Создание пар для скрещивания на основе рангового отбора
    // Вероятность отбора особи пропорциональна её порядковому номеру (рангу), отсортированному по возрастанию приспособленности.
    // Формирование пар с вероятностью, пропорциональной рангу (ранговый отбор)
    public List<(int, int)> CreatePairsByRankProbability(Dictionary<int, int> ranks)
    {
        int totalRank = ranks.Values.Sum();
        int numPairs = (_config.PopulationSize - 2) / 2; // 2 элиты, остальные для пар

        List<int> SelectParents()
        {
            var cumulativeRanks = new List<(int index, int cumulative)>();
            int cumulativeSum = 0;

            foreach (var rankPair in ranks)
            {
                cumulativeSum += rankPair.Value;
                cumulativeRanks.Add((rankPair.Key, cumulativeSum));
            }

            List<int> selected = new();

            while (selected.Count < 2)
            {
                int randomValue = _random.Next(totalRank);
                int chosenIndex = cumulativeRanks.First(pair => randomValue < pair.cumulative).index;

                if (!selected.Contains(chosenIndex))
                    selected.Add(chosenIndex);
            }

            return selected;
        }

        var pairs = new List<(int, int)>();

        for (int i = 0; i < numPairs; i++)
        {
            var parents = SelectParents();
            pairs.Add((parents[0], parents[1]));
        }

        return pairs;
    }
    
    // Одноточечный кроссовер: Выбирается случайная точка разрыва, после чего родители обмениваются частями, создавая двух потомков
    public List<List<int>> Crossover(List<List<int>> population, List<(int, int)> pairs)
    {
        var offspring = new List<List<int>>();

        foreach ((int i1, int i2) in pairs)
        {
            List<int> p1 = population[i1];
            List<int> p2 = population[i2];
            int breakPoint = _random.Next(minValue: 1, _config.IndividualLength - 1);

            List<int> child1 = p1
               .Take(breakPoint)
               .Concat(p2.Skip(breakPoint))
               .ToList();

            List<int> child2 = p2
               .Take(breakPoint)
               .Concat(p1.Skip(breakPoint))
               .ToList();

            offspring.Add(child1);
            offspring.Add(child2);
        }

        return offspring;
    }

    // Мутация: один случайный бит инвертируется с заданной вероятностью
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
}
