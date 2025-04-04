// Генетический алгоритм — Вариант 8

class Program
{
    static void Main()
    {
        // Конфигурация: длина особи 8 бит, 10 особей, 50% вероятность мутации
        var config = new GeneticConfig(length: 8, size: 10, mutation: 0.5, numberOfElites: 2);
        var algorithm = new GeneticAlgorithm(config);

        // Шаг 1: Генерация начальной популяции
        List<List<int>> population = algorithm.GetPredefinedPopulation(config.InitialPopulation);
        ConsoleHelper.PrintPopulation(population, "Initial population:");

        // Шаг 2: Оценка фитнесс-функции для каждой особи
        List<(List<int> Individual, int BitSum, double Fitness)> fitness = algorithm.EvaluateFitness(population);
        ConsoleHelper.PrintFitnessTable(fitness);

        // Шаг 3: Сортировка по фитнессу и сохранение лучшей особи (элитизм)
        List<(int Index, List<int> Individual, double Fitness)> sorted = fitness
           .Select((e, i) => (Index: i, e.Individual, e.Fitness))
           .OrderByDescending(x => x.Fitness)
           .ToList();

        List<List<int>> elites = new();
        
        for (int i = 0; i < config.NumberOfElites; i++)
        {
            elites.Add(sorted[i].Individual);
        }

        ConsoleHelper.PrintSortedByFitness(sorted);
        
        // Шаг 4: Назначение рангов и отбор пар
        Dictionary<int, int> ranks = algorithm.AssignRanks(sorted);
        //List<(int, int)> pairs = algorithm.CreatePairsByRankProbability(ranks);
        List<(int, int)> pairs = new List<(int, int)>()
        {
            (7, 4),
            (4, 2),
            (2, 4),
            (1, 0)
        };
        
        ConsoleHelper.PrintPairs(pairs);

        // Шаг 5: Кроссовер
        List<List<int>> offspring = algorithm.Crossover(population, pairs);
        ConsoleHelper.PrintOffspring(pairs, population, offspring);

        // Шаг 6: Мутации
        List<List<int>> mutated = algorithm.MutateOffspring(offspring);
        ConsoleHelper.PrintMutations(offspring, mutated);

        // Шаг 7: Формирование новой популяции
        var newGeneration = new List<List<int>>(); // элитная особь
        foreach (var elite in elites)
        {
            newGeneration.Add(elite);
        }

        foreach (List<int> child in mutated)
        {
            if (newGeneration.Count < config.PopulationSize)
            {
                newGeneration.Add(child);
            }
        }

        // Шаг 8: Вывод новой популяции
        ConsoleHelper.PrintPopulation(newGeneration, "New population");
    }
}
