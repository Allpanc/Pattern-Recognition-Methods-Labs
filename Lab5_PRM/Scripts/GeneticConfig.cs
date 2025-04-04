
// Класс конфигурации для хранения параметров ГА
public class GeneticConfig
{
    public int IndividualLength { get; } // длина хромосомы
    public int PopulationSize { get; } // количество особей
    public double MutationProbability { get; } // вероятность мутации
    public int NumberOfElites { get; }
    
    public string[] InitialPopulation => new string[]
    {
        "00100101",
        "01011011",
        "01101000",
        "10000010",
        "10110110",
        "00101011",
        "01011000",
        "01100101",
        "10001011",
        "10110010"
    };

    public GeneticConfig(int length, int size, double mutation, int numberOfElites)
    {
        IndividualLength = length;
        PopulationSize = size;
        MutationProbability = mutation;
        NumberOfElites = numberOfElites;
    }
}
