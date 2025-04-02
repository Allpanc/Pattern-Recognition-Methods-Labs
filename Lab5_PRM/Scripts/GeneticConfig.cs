
// Класс конфигурации для хранения параметров ГА
public class GeneticConfig
{
    public int IndividualLength { get; } // длина хромосомы
    public int PopulationSize { get; } // количество особей
    public double MutationProbability { get; } // вероятность мутации

    public GeneticConfig(int length, int size, double mutation)
    {
        IndividualLength = length;
        PopulationSize = size;
        MutationProbability = mutation;
    }
}
