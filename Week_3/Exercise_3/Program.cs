using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

Console.OutputEncoding = System.Text.Encoding.UTF8;

const string Url = "https://www.mediapool.bg/";

try
{
    var html = await GetHtmlAsync(Url);
    var newsItems = ExtractNews(html);

    Console.WriteLine($"found news: {newsItems.Count}");
    Console.WriteLine();

    foreach (var item in newsItems)
    {
        if (ShouldExclude(item.Title))
        {
            continue;
        }

        Console.WriteLine($"Title: {item.Title}");
        Console.WriteLine($"Date and Time: {item.DateTime}");
        Console.WriteLine(new string('-', 60));
    }
}
catch (HttpRequestException ex)
{
    Console.Error.WriteLine($"Request error: {ex.Message}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected error: {ex.Message}");
}

static async Task<string> GetHtmlAsync(string url)
{
    using var client = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
        "AppleWebKit/537.36 (KHTML, like Gecko) " +
        "Chrome/130.0.0.0 Safari/537.36");

    using var response = await client.GetAsync(url);
    response.EnsureSuccessStatusCode();

    var html = await response.Content.ReadAsStringAsync();
    return html;
}

static List<NewsItem> ExtractNews(string html)
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

static bool ShouldExclude(string title)
{
    var lower = title.ToLowerInvariant();

    return lower.Contains("covid-19") ||
           lower.Contains("корона вирус") ||
           lower.Contains("пандемия");
}

static string StripTags(string html)
{
    return Regex.Replace(html, "<.*?>", string.Empty);
}

internal class NewsItem
{
    public string Title { get; set; } = string.Empty;
    public string DateTime { get; set; } = string.Empty;
}
