using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics;
using Accord.Statistics.Kernels;

internal class Program
{
    private static void Main()
    {
        // Загружаем обучающую и тестовую выборки
        var trainList = Loader.LoadData(PathProvider.TrainDatasetPath);
        var testList = Loader.LoadData(PathProvider.TestDatasetPath);
        
        // Определяем размеры выборок
        var trainSize = trainList.Count;
        var testSize = testList.Count;

        // Создаем массивы признаков и меток классов
        var trainInputs = new double[trainSize, 2];
        var trainOutputs = new int[trainSize];

        var testInputs = new double[testSize, 2];
        var testOutputs = new int[testSize];
        
        // Словарь для преобразования строковых меток в числовые классы
        var labelMap = new Dictionary<string, int> { ["red"] = 0, ["green"] = 1 };

        // Заполняем обучающие данные: признаки и метки классов
        for (var i = 0; i < trainSize; i++)
        {
            trainInputs[i, 0] = trainList[i].X1;
            trainInputs[i, 1] = trainList[i].X2;
            trainOutputs[i] = labelMap[trainList[i].Color];
        }

        // Заполняем тестовые данные: признаки и метки классов
        for (var i = 0; i < testSize; i++)
        {
            testInputs[i, 0] = testList[i].X1;
            testInputs[i, 1] = testList[i].X2;
            testOutputs[i] = labelMap[testList[i].Color];
        }

        // Масштабируем признаки с помощью Z-преобразования (нормализация по столбцам)
        var means = trainInputs.Mean(0);                    // Среднее по каждому признаку
        var stdDevs = trainInputs.StandardDeviation();      // Стандартное отклонение

        trainInputs = trainInputs.ZScores(means, stdDevs);  // Масштабируем обучающую выборку
        testInputs = testInputs.ZScores(means, stdDevs);    // Масштабируем тестовую выборку теми же параметрами

        var variance = trainInputs.Variance(); // дисперсия признаков
        double gamma = 1.0 / (trainInputs.GetLength(1) * variance.Mean()); 
        
        // Настройка обучающего алгоритма SVM с гауссовским (RBF) ядром
        var teacher = new SequentialMinimalOptimization<Gaussian>
        {
            Kernel = new Gaussian(gamma),    // Параметр ширины ядра (σ)
            Complexity = 1.0               // Параметр регуляризации (C)
        };

        // Accord требует jagged-массивы, преобразуем из 2D
        var jaggedTrainInputs = trainInputs.ToJagged();

        // Обучаем модель SVM
        var svm = teacher.Learn(jaggedTrainInputs, trainOutputs);

        // Предсказываем классы на тестовой выборке
        var jaggedTestInputs = testInputs.ToJagged();
        var boolPredictions = svm.Decide(jaggedTestInputs);          // true/false
        var predictions = boolPredictions.Select(b => b ? 1 : 0).ToArray(); // Преобразуем в 0/1

        // Вычисляем точность классификации
        var accuracy = predictions.Zip(testOutputs, (p, t) => p == t ? 1.0 : 0.0).Average();
        Console.WriteLine($"Accuracy: {accuracy:P2}"); // Выводим точность в формате процентов

        // Визуализация результатов классификации
        PlotHelper.ShowClassificationPlot(testInputs, predictions, svm, means, stdDevs);

        // Визуализация матрицы ошибок
        PlotHelper.ShowConfusionMatrix(testOutputs, predictions, "svm_confusion_matrix");
    }
}
