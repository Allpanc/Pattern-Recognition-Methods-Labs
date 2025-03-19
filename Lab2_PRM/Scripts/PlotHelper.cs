using ScottPlot;

namespace Lab2;

public static class PlotHelper
{
    public static void AddCoordinatesToPlot(Plot plt, double[] xValues, double[] yValues, Color color)
    {
        var scatter = plt.Add.Scatter(xValues, yValues);
        scatter.MarkerSize = 5;
        scatter.Color = color;
    }

    public static void SaveResultPlot(Plot plt, string filename, string title)
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