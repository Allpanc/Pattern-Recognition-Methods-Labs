using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using ScottPlot;
using ScottPlot.Colormaps;
using ScottPlot.TickGenerators;

internal static class PlotHelper
{
    // Отображает scatter plot по предсказанным классам
    public static void ShowClassificationPlot(
        double[,] data, 
        int[] predicted,
        SupportVectorMachine<Gaussian> svm,
        double[] means,
        double[] stds)
    {
        var plt = new Plot();

        var redX = new List<double>();
        var redY = new List<double>();
        var greenX = new List<double>();
        var greenY = new List<double>();

        // Разделяем точки по предсказанным классам (0 = red, 1 = green)
        for (var i = 0; i < predicted.Length; i++)
        {
            if (predicted[i] == 0)
            {
                redX.Add(data[i, 0]);
                redY.Add(data[i, 1]);
            }
            else
            {
                greenX.Add(data[i, 0]);
                greenY.Add(data[i, 1]);
            }
        }

        // Добавляем точки на график
        AddCoordinatesToPlot(plt, redX.ToArray(), redY.ToArray(), Colors.Red, "Red predicted");
        AddCoordinatesToPlot(plt, greenX.ToArray(), greenY.ToArray(), Colors.Green, "Green predicted");

        // === Построение границы разделения ===
        int resolution = 300;
        double xMin = -2.5, xMax = 2.5;
        double yMin = -2.5, yMax = 2.5;

        double[] xs = Enumerable.Range(0, resolution)
            .Select(i => xMin + i * (xMax - xMin) / (resolution - 1))
            .ToArray();

        double[] ys = Enumerable.Range(0, resolution)
            .Select(i => yMin + i * (yMax - yMin) / (resolution - 1))
            .ToArray();

        double[,] z = new double[resolution, resolution];

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                double x1 = xs[j];
                double x2 = ys[i];

                // Применяем Z-преобразование (то же, что на обучении)
                double scaledX1 = (x1 - means[0]) / stds[0];
                double scaledX2 = (x2 - means[1]) / stds[1];

                z[i, j] = svm.Score(new[] { scaledX1, scaledX2 });
            }
        }

        // Добавляем линию границы
        var coords = new ScottPlot.Coordinates3d[resolution, resolution];
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                coords[i, j] = new ScottPlot.Coordinates3d(xs[j], ys[i], z[i, j]);
            }
        }

        var contour = plt.Add.ContourLines(coords, 1);
        contour.LineWidth = 2;
        contour.LineColor = Colors.Black;
        contour.LabelStyle = new LabelStyle
        {
            FontSize = 0
        };
        contour.ContourLineLevels = new double[] { 0 }; // decision boundary
        
        // Сохраняем график в файл
        SaveResultPlot(plt, "svm_classification_result", "SVM Classification Result");
    }

    // Создает и отображает confusion matrix
    public static void ShowConfusionMatrix(int[] actual, int[] predicted, string filename)
    {
        var plt = new Plot();

        // Заполняем матрицу ошибок 2x2: [истинный класс, предсказанный класс]
        var confusion = new int[2, 2];
        
        for (int i = 0; i < actual.Length; i++)
        {
            confusion[actual[i], predicted[i]]++;
        }

        var data = new double[2, 2];

        // Переворачиваем матрицу по вертикали для правильного визуального порядка
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                data[1 - i, j] = confusion[i, j];
            }
        }

        // Добавляем тепловую карту (Heatmap)
        var hm = plt.Add.Heatmap(data);
        hm.Colormap = new Blues();

        // Подписываем значения в ячейках
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                var value = data[i, j];
                var label = plt.Add.Text(value.ToString(), j, i);
                label.Alignment = Alignment.MiddleCenter;
                label.LabelFontSize = 20;
                label.LabelBold = true;
                label.LabelFontColor = value < 30 ? Colors.Black : Colors.White;
            }
        }

        // Добавляем цветовую шкалу
        var cb = plt.Add.ColorBar(hm);
        cb.Label = "Количество";

        // Подписи осей
        plt.Title("Матрица ошибок", size: 24);
        plt.XLabel("Предсказанный класс", size: 18);
        plt.YLabel("Истинный класс", size: 18);

        // Настраиваем метки осей вручную (0 — red, 1 — green)
        plt.Axes.Bottom.TickGenerator = new NumericManual(
            new[] { 0, 1d }, new[] { "red", "green" });
        plt.Axes.Left.TickGenerator = new NumericManual(
            new[] { 0, 1d }, new[] { "green", "red" }); // перевернуто для визуального соответствия

        // Устанавливаем квадратную сетку
        plt.Axes.SetLimits(-0.5, 1.5, -0.5, 1.5);
        
        // Сохраняем график
        SavePlotToPng(plt, filename);
    }

    // Вспомогательная функция для добавления точек на график
    private static void AddCoordinatesToPlot(Plot plt, double[] xValues, double[] yValues, Color color, string legendText)
    {
        var scatter = plt.Add.Scatter(xValues, yValues);
        scatter.MarkerSize = 5;
        scatter.Color = color;
        scatter.LineWidth = 0; // только точки, без линий
        scatter.LegendText = legendText;
    }

    // Вспомогательная функция для сохранения графика с подписями осей и заголовком
    private static void SaveResultPlot(Plot plt, string filename, string title)
    {
        plt.Title(title);
        plt.XLabel("X1");
        plt.YLabel("X2");
        SavePlotToPng(plt, filename);
    }

    // Сохраняет график в PNG-файл
    private static void SavePlotToPng(Plot plt, string filename)
    {
        var path = Path.Combine(PathProvider.GeneratedPath, $"{filename}.png");
        Directory.CreateDirectory(PathProvider.GeneratedPath);
        plt.SavePng(path, 1200, 800);
        Console.WriteLine($"Png file {filename} is saved to: {path}");
    }
}
