using ScottPlot;

internal static class PlotHelper
{
    public static void VisualizeFisher(List<ClassData> classes, List<ClassifierParameters> classifiers)
    {
        string filename = "fisher_plot";
        var plt = new Plot();
        
        // Отрисовка точек классов
        DrawPoints(classes, plt);

        // Отрисовка границ между классами
        double xMin = -10, xMax = 10;

        for (var i = 0; i < classifiers.Count; i++)
        {
            var individualPlot = new Plot();
            var clf = classifiers[i];
            var w = clf.Weights;
            var b = clf.Threshold;

            var y0 = (-b - w[0] * xMin) / w[1];
            var y1 = (-b - w[0] * xMax) / w[1];

            var line = plt.Add.Line(xMin, y0, xMax, y1);
            line.Color = Pallete.lineColors[i % Pallete.classColors.Length];
            line.LineWidth = 2;
            line.LegendText = $"Boundary {clf.ClassIndices.Item1 + 1} и {clf.ClassIndices.Item2 + 1}";
        }

        plt.Legend.Alignment = Alignment.UpperRight;

        SaveResultPlot(plt, filename, "Fisher classifier");
    }

    public static void VisualizeBayes(
        List<ClassData> classes,
        double[] classPriors,
        double xMin, double xMax, double yMin, double yMax,
        int gridSize = 100)
    {
        string filename = "bayes_plot";
        var plt = new Plot();

        DrawPoints(classes, plt);
        
        var xGrid = new double[gridSize];
        var yGrid = new double[gridSize];
        
        for (var i = 0; i < gridSize; i++)
        {
            xGrid[i] = xMin + i * (xMax - xMin) / (gridSize - 1);
            yGrid[i] = yMin + i * (yMax - yMin) / (gridSize - 1);
        }

        // Массивы точек по классам
        List<(double x, double y)>[] classPoints =
        {
            new(),
            new(),
            new()
        };

        // Классификация каждого пикселя
        for (var i = 0; i < gridSize; i++)
        {
            for (var j = 0; j < gridSize; j++)
            {
                var x = xGrid[i];
                var y = yGrid[j];

                var cls = BayesClassifier.ClassifyNaiveBayes(x, y, classes, classPriors);
                classPoints[cls].Add((x, y));
            }
        }

        // Отрисовка фона по классам
        for (var cls = 0; cls < classPoints.Length; cls++)
        {
            var points = classPoints[cls];
            
            if (points.Count == 0)
            {
                continue;
            }

            var xs = points.Select(p => p.x).ToArray();
            var ys = points.Select(p => p.y).ToArray();

            var scatter = plt.Add.Scatter(xs, ys);
            scatter.MarkerShape = MarkerShape.OpenSquare;
            scatter.MarkerSize = 3;
            scatter.Color = Pallete.backgroundColors[cls];
            scatter.LineWidth = 0;
            plt.MoveToBack(scatter);
        }
        
        SaveResultPlot(plt, filename, "Bayes classifier");
    }

    private static void DrawPoints(List<ClassData> classes, Plot plt)
    {
        for (var i = 0; i < classes.Count; i++)
        {
            var cls = classes[i];
            var xs = cls.Samples.Column(0).ToArray();
            var ys = cls.Samples.Column(1).ToArray();

            var colorIndex = i % Pallete.classColors.Length;
            AddCoordinatesToPlot(plt, xs, ys, Pallete.classColors[colorIndex], (colorIndex + 1).ToString());
        }
    }

    private static void AddCoordinatesToPlot(Plot plt, double[] xValues, double[] yValues, Color color,
        string legendText)
    {
        var scatter = plt.Add.Scatter(xValues, yValues);
        scatter.MarkerSize = 10;
        scatter.Color = color;
        scatter.LineWidth = 0;
        scatter.LegendText = legendText;
    }

    private static void SaveResultPlot(Plot plt, string filename, string title)
    {
        plt.Title(title);
        plt.XLabel("X");
        plt.YLabel("Y");
        SavePlotToPng(plt, filename);
    }

    private static void SavePlotToPng(Plot plt, string title)
    {
        plt.SavePng($"{PathProvider.ResourcesPath}\\{title}.png", 1200, 800);
    }
}