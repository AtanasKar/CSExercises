using System;

namespace Exercise_2;

internal static class Program
{
    public static void Main(string[] args)
    {
        var client = new TimeAndDateClient();
        try
        {
            var result = client.GetCurrentDayAndTime();

            Console.WriteLine($"Day: {result.Day}");
            Console.WriteLine($"Time: {result.Time}");
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }
}
