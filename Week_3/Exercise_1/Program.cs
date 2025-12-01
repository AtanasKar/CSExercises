using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Exercise_1;

internal static class Program
{
    private static readonly HttpClient HttpClient;

    static Program()
    {
        HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/130.0.0.0 Safari/537.36");

        HttpClient.DefaultRequestHeaders.Accept.ParseAdd("text/plain");
    }

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Enter IPv4 address (emty line to exit):");

        while (true)
        {
            Console.Write("IP: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                break;
            }

            if (!IPAddress.TryParse(input, out var address) ||
                address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Console.WriteLine("Invalid IPv4 address. Try again.");
                continue;
            }

            try
            {
                var country = await GetCountryByIpAsync(input.Trim());
                Console.WriteLine($"Country: {country}");
            }
            catch (TaskCanceledException)
            {
                Console.Error.WriteLine("Time out (waiting time exceeded).");
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Request error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }

    private static async Task<string> GetCountryByIpAsync(string ip)
    {
        var ipApiUrl = $"https://ipapi.co/{ip}/country/";

        using var response = await HttpClient.GetAsync(ipApiUrl);
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new InvalidOperationException(
                "Error 429 (Too Many Requests).");
        }

        response.EnsureSuccessStatusCode();

        var content = (await response.Content.ReadAsStringAsync()).Trim();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("No information received.");
        }

        return content;
    }
}
