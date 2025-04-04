using System.Globalization;

internal static class Loader
{
    public static List<DataPoint> LoadData(string path)
    {
        var result = new List<DataPoint>();

        foreach (var line in File.ReadLines(path))
        {
            var parts = line.Split('\t');

            // Пропускаем заголовок или некорректные строки
            if (parts.Length < 3 || parts[0] == "X1")
            {
                continue;
            }

            // Создаем объект DataPoint из строки
            var dataPoint = new DataPoint
            {
                X1 = double.Parse(parts[1], CultureInfo.InvariantCulture),
                X2 = double.Parse(parts[2], CultureInfo.InvariantCulture),
                Color = parts[3].Trim().ToLower()
            };

            result.Add(dataPoint);
        }

        return result;
    }
}