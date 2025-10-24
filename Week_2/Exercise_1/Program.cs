using System.Text.Json;

public struct Coordinate
{
    public float lat { get; set; }
    public float lng { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            string inputFile = "input-01.txt";
            string content = File.ReadAllText(inputFile);

            string[] coordinatePairs = content.Split(';', StringSplitOptions.RemoveEmptyEntries);

            Coordinate[] coordinates = new Coordinate[coordinatePairs.Length];

            for (int i = 0; i < coordinatePairs.Length; i++)
            {
                string[] latLng = coordinatePairs[i].Split(',');
                if (latLng.Length == 2)
                {
                    coordinates[i] = new Coordinate
                    {
                        lat = float.Parse(latLng[0].Trim()),
                        lng = float.Parse(latLng[1].Trim())
                    };
                }
            }

            string json = JsonSerializer.Serialize(coordinates, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            string outputFile = "output.json";
            File.WriteAllText(outputFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}