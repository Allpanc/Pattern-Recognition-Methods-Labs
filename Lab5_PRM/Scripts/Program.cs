// Генетический алгоритм — Вариант 8
// Полный вывод всех этапов алгоритма в консоль, как в Jupyter/ipynb и отчёте

class Program
{
    static void Main()
    {
        // Конфигурация: длина особи 8 бит, 10 особей, 50% вероятность мутации
        var config = new GeneticConfig(length: 8, size: 10, mutation: 0.5);
        var algorithm = new GeneticAlgorithm(config);

        // Шаг 1: Генерация начальной популяции
        List<List<int>> population = algorithm.GenerateInitialPopulation();
        algorithm.PrintPopulation(population, "Начальная популяция:");

        // Шаг 2: Оценка фитнесс-функции для каждой особи
        List<(List<int> Individual, int BitSum, double Fitness)> fitness = algorithm.EvaluateFitness(population);
        algorithm.PrintFitnessTable(fitness);

        // Шаг 3: Сортировка по фитнессу и сохранение лучшей особи (элитизм)
        List<(int Index, List<int> Individual, double Fitness)> sorted = fitness
           .Select((e, i) => (Index: i, e.Individual, e.Fitness))
           .OrderByDescending(x => x.Fitness)
           .ToList();

        List<int> elite = sorted[0].Individual;

        // Шаг 4: Назначение рангов и отбор пар
        Dictionary<int, int> ranks = algorithm.AssignRanks(sorted);
        List<(int, int)> pairs = algorithm.CreatePairs(ranks);
        algorithm.PrintPairs(pairs);

        // Шаг 5: Кроссовер
        List<List<int>> offspring = algorithm.Crossover(population, pairs);
        algorithm.PrintOffspring(pairs, population, offspring);

        // Шаг 6: Мутации
        List<List<int>> mutated = algorithm.MutateOffspring(offspring);
        algorithm.PrintMutations(offspring, mutated);

        // Шаг 7: Формирование новой популяции
        var newGeneration = new List<List<int>>
        {
            new(elite)
        }; // элитная особь

        foreach (List<int> child in mutated)
        {
            if (newGeneration.Count < config.PopulationSize)
            {
                newGeneration.Add(child);
            }
        }

        // Шаг 8: Вывод новой популяции
        algorithm.PrintPopulation(newGeneration, "Новая популяция после мутаций и элитизма:");
    }
}
