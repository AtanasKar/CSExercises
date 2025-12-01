using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace MenuApp;

internal class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Directory.CreateDirectory(DataDirectory);

        while (true)
        {
            Console.WriteLine("===== МЕНЮ =====");
            Console.WriteLine("1) Проверка на IP (за държава) - задача 1");
            Console.WriteLine("2) Текущ ден и час в София       - задача 2");
            Console.WriteLine("3) Новини от Mediapool           - задача 3");
            Console.WriteLine("0) Изход");
            Console.Write("Изберете опция: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    await RunIpLookupAsync();
                    break;
                case "2":
                    await RunTimeAndDateAsync();
                    break;
                case "3":
                    await RunNewsAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Невалидна опция. Опитайте отново.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private static string DataDirectory =>
        Path.Combine(AppContext.BaseDirectory, "Data");

    // ===== ЗАДАЧА 1 – IP към държава (ipapi.co) =====

    private static async Task RunIpLookupAsync()
    {
        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/130.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Accept.ParseAdd("text/plain");

        Console.Write("Въведете IPv4 адрес (празен ред за връщане в менюто): ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        if (!IPAddress.TryParse(input, out var address) ||
            address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
        {
            Console.WriteLine("Невалиден IPv4 адрес.");
            return;
        }

        var ip = input.Trim();
        var url = $"https://ipapi.co/{ip}/country/";

        try
        {
            using var response = await client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Console.WriteLine("Сървърът върна 429 (Too Many Requests). Изчакайте и опитайте пак.");
                return;
            }

            response.EnsureSuccessStatusCode();
            var content = (await response.Content.ReadAsStringAsync()).Trim();

            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Не беше върната информация за държавата.");
            }
            else
            {
                Console.WriteLine($"Държава (код): {content}");

                var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss};{ip};{content}";
                var path = Path.Combine(DataDirectory, "ip_results.csv");
                await File.AppendAllLinesAsync(path, new[] { line }, Encoding.UTF8);
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Заявката отне твърде дълго (изтече времето за изчакване).");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Грешка при заявката: {ex.Message}");
        }
    }

    // ===== ЗАДАЧА 2 – Текущ ден и час в София (timeanddate.com) =====

    private static async Task RunTimeAndDateAsync()
    {
        try
        {
            var client = new TimeAndDateClientForMenu();
            var (day, time) = await client.GetCurrentDayAndTimeAsync();

            Console.WriteLine($"Ден:  {day}");
            Console.WriteLine($"Час: {time}");

            var path = Path.Combine(DataDirectory, "time_results.txt");
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Ден: {day}, Час: {time}";
            await File.AppendAllLinesAsync(path, new[] { line }, Encoding.UTF8);
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException)
        {
            Console.WriteLine($"Грешка: {ex.Message}");
        }
    }

    // ===== ЗАДАЧА 3 – Новини от Mediapool (без Covid-19/корона/пандемия) =====

    private static async Task RunNewsAsync()
    {
        const string url = "https://www.mediapool.bg/";

        try
        {
            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/130.0.0.0 Safari/537.36");

            var html = await client.GetStringAsync(url);
            var newsItems = ExtractNews(html);

            Console.WriteLine($"Намерени новини: {newsItems.Count}");
            Console.WriteLine();

            var filtered = new List<NewsItem>();

            foreach (var item in newsItems)
            {
                if (ShouldExclude(item.Title))
                {
                    continue;
                }

                filtered.Add(item);

                Console.WriteLine($"Заглавие: {item.Title}");
                Console.WriteLine($"Дата и час: {item.DateTime}");
                Console.WriteLine(new string('-', 60));
            }

            // запис в CSV файл
            var path = Path.Combine(DataDirectory, "news_results.csv");
            var lines = new List<string> { "Title;DateTime" };
            lines.AddRange(filtered.Select(n =>
                $"{n.Title.Replace(';', ',')};{n.DateTime.Replace(';', ',')}"));

            await File.WriteAllLinesAsync(path, lines, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Грешка при зареждане на новините: {ex.Message}");
        }
    }

    private static List<NewsItem> ExtractNews(string html)
    {
        var result = new List<NewsItem>();

        var titlePattern = new Regex(
            "<h3[^>]*>(?<title>.*?)</h3>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        foreach (Match titleMatch in titlePattern.Matches(html))
        {
            var startIndex = titleMatch.Index;
            var searchLength = Math.Min(2000, html.Length - startIndex);
            var nearbyHtml = html.Substring(startIndex, searchLength);

            var nearbyText = StripTags(nearbyHtml);
            var dateMatch = Regex.Match(
                nearbyText,
                @"\d{1,2}\.\d{1,2}\.\d{4}[^0-9]{0,10}\d{1,2}:\d{2}",
                RegexOptions.Singleline);

            var rawTitle = StripTags(titleMatch.Groups["title"].Value).Trim();
            var rawDate = dateMatch.Success
                ? dateMatch.Value.Trim()
                : "няма дата";

            if (string.IsNullOrWhiteSpace(rawTitle))
            {
                continue;
            }

            result.Add(new NewsItem
            {
                Title = WebUtility.HtmlDecode(rawTitle),
                DateTime = WebUtility.HtmlDecode(rawDate)
            });
        }

        return result;
    }

    private static bool ShouldExclude(string title)
    {
        var lower = title.ToLowerInvariant();

        return lower.Contains("covid-19") ||
               lower.Contains("корона вирус") ||
               lower.Contains("пандемия");
    }

    private static string StripTags(string html)
    {
        return Regex.Replace(html, "<.*?>", string.Empty);
    }
}

internal class TimeAndDateClientForMenu
{
    private const string Url = "https://www.timeanddate.com/worldclock/bulgaria/sofia";

    public async Task<(string Day, string Time)> GetCurrentDayAndTimeAsync()
    {
        var html = await GetHtmlFromUrlAsync(Url);

        var day = ExtractContentById(html, "ctdat");
        var time = ExtractContentById(html, "ct");

        return (day, time);
    }

    private static async Task<string> GetHtmlFromUrlAsync(string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/130.0.0.0 Safari/537.36");

        using var client = new HttpClient();
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    private static string ExtractContentById(string html, string id)
    {
        var pattern = $"<span[^>]*id\\s*=\\s*\"?{id}\"?[^>]*>(?<value>.*?)</span>";
        var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (!match.Success)
        {
            throw new InvalidOperationException($"Не беше намерен елемент с id '{id}'.");
        }

        var value = match.Groups["value"].Value.Trim();
        return WebUtility.HtmlDecode(value);
    }
}

internal class NewsItem
{
    public string Title { get; set; } = string.Empty;
    public string DateTime { get; set; } = string.Empty;
}
