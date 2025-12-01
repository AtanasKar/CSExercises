using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

public class TimeAndDateClient
{
    private const string Url = "https://www.timeanddate.com/worldclock/bulgaria/sofia";

    public (string Day, string Time) GetCurrentDayAndTime()
    {
        var html = GetHtmlFromUrl(Url);

        var day = ExtractContentById(html, "ctdat");
        var time = ExtractContentById(html, "ct");

        return (day, time);
    }

    private static string GetHtmlFromUrl(string url)
    {
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                            "AppleWebKit/537.36 (KHTML, like Gecko) " +
                            "Chrome/130.0.0.0 Safari/537.36";

        using (var response = (HttpWebResponse)request.GetResponse())
        using (var stream = response.GetResponseStream())
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    private static string ExtractContentById(string html, string id)
    {
        var pattern = $"<span[^>]*id\\s*=\\s*\"?{id}\"?[^>]*>(?<value>.*?)</span>";
        var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (!match.Success)
        {
            throw new InvalidOperationException($"Could not find the element with id '{id}'.");
        }

        var value = match.Groups["value"].Value.Trim();
        return WebUtility.HtmlDecode(value);
    }
}
