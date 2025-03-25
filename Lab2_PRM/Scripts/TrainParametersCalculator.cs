namespace Lab2;

public static class TrainParametersCalculator
{
    /// Входные параметры:
    /// обучающая выборка по X и Y,
    /// метки классов,
    /// количество классов
    /// Возвращаемые значения:
    /// математические ожидания (оценки),
    /// ковариационные матрицы (оценки),
    /// априорные вероятности каждого класса
    
    public static (double[][] ClassMeans, double[][,] ClassCovariances, double[] ClassPriors)
        Train(double[] trainDataX, double[] trainDataY, int[] trainLabels, int numberOfClasses)
    {
        var totalSamples = trainDataX.Length;

        // Количество точек в каждом классе
        var classSampleCounts = GetClassSampleCounts(trainLabels, numberOfClasses, totalSamples);
        
        // Оценки математических ожиданий
        var classMeans = GetClassMeans(trainLabels, numberOfClasses, totalSamples, classSampleCounts, trainDataX, trainDataY);

        // Оценки ковариационных матриц
        var classCovariances = GetClassCovariances(trainLabels, numberOfClasses, totalSamples, classSampleCounts, trainDataX,
            trainDataY, classMeans);
        
        // Априорные вероятности
        var classPriors = GetClassPriors(numberOfClasses, classSampleCounts, totalSamples);

        return (classMeans, classCovariances, classPriors);
    }

    private static int[] GetClassSampleCounts(int[] trainLabels, int numberOfClasses, int totalSamples)
    {
        var classSampleCounts = new int[numberOfClasses];

        for (var i = 0; i < totalSamples; i++)
        {
            var classLabel = trainLabels[i];

            classSampleCounts[classLabel]++;
        }

        return classSampleCounts;
    }

    private static double[][] GetClassMeans(int[] labels, int numClasses, int totalSamples, int[] classSampleCounts,
        double[] trainDataX, double[] trainDataY)
    {
        var classMeans = new double[numClasses][];
        
        // Оценка математических ожиданий для каждого класса
        for (var c = 0; c < numClasses; c++)
        {
            classMeans[c] = new double[2]; // для x и y
            classMeans[c][0] = 0; // x
            classMeans[c][1] = 0; // y
        }

        // Суммируются значения координат для каждого класса
        for (var i = 0; i < totalSamples; i++)
        {
            var classLabel = labels[i];
            classMeans[classLabel][0] += trainDataX[i]; // x
            classMeans[classLabel][1] += trainDataY[i]; // y
        }

        // Деление на количество точек в каждом классе
        for (var c = 0; c < numClasses; c++)
        {
            if (classSampleCounts[c] > 0)
            {
                classMeans[c][0] /= classSampleCounts[c]; // x
                classMeans[c][1] /= classSampleCounts[c]; // y
            }
        }

        return classMeans;
    }

    private static double[][,] GetClassCovariances(int[] labels, int numClasses, int totalSamples,
        int[] classSampleCounts, double[] trainDataX, double[] trainDataY, double[][] classMeans)
    {
        var classCovariances = new double[numClasses][,];
        
        // Оценка ковариационных матриц для каждого класса
        for (var c = 0; c < numClasses; c++)
        {
            classCovariances[c] = new double[2, 2];
            classCovariances[c][0, 0] = 0; // xx
            classCovariances[c][0, 1] = 0; // xy
            classCovariances[c][1, 0] = 0; // yx
            classCovariances[c][1, 1] = 0; // yy
        }

        // Суммируются произведения отклонений от средних
        for (var i = 0; i < totalSamples; i++)
        {
            var classLabel = labels[i];

            var dx = trainDataX[i] - classMeans[classLabel][0];
            var dy = trainDataY[i] - classMeans[classLabel][1];

            classCovariances[classLabel][0, 0] += dx * dx;
            classCovariances[classLabel][0, 1] += dx * dy;
            classCovariances[classLabel][1, 0] += dx * dy;
            classCovariances[classLabel][1, 1] += dy * dy;
        }

        // Деление на количество точек в каждом классе
        for (var c = 0; c < numClasses; c++)
        {
            classCovariances[c][0, 0] /= classSampleCounts[c]; // xx
            classCovariances[c][0, 1] /= classSampleCounts[c]; // xy
            classCovariances[c][1, 0] /= classSampleCounts[c]; // yx
            classCovariances[c][1, 1] /= classSampleCounts[c]; // yy
        }

        return classCovariances;
    }

    private static double[] GetClassPriors(int numClasses, int[] classSampleCounts, int totalSamples)
    {
        var classPriors = new double[numClasses];

        for (var c = 0; c < numClasses; c++)
        {
            classPriors[c] = (double)classSampleCounts[c] / totalSamples;
        }

        return classPriors;
    }
}