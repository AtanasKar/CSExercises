using System.Text.Json;
using System.Xml.Linq;

public class TagWithConnection
{
    public string TagName { get; set; } = string.Empty;
    public Dictionary<string, string> Attributes { get; set; } = new();
    public string? Connection { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        XDocument doc = XDocument.Load("input-03.dae");
        Dictionary<string, XElement> tagsById = new();
        List<TagWithConnection> tagsWithConnections = new();

        foreach (var element in doc.Descendants())
        {
            var idAttr = element.Attribute("id");
            if (idAttr != null && !string.IsNullOrEmpty(idAttr.Value))
            {
                tagsById[idAttr.Value] = element;
            }
        }

        foreach (var element in doc.Descendants())
        {
            var tag = new TagWithConnection
            {
                TagName = element.Name.LocalName
            };

            foreach (var attr in element.Attributes())
            {
                string attrValue = attr.Value;
                if (attrValue.StartsWith("#"))
                {
                    string referenceId = attrValue.Substring(1);
                    if (tagsById.ContainsKey(referenceId))
                    {
                        tag.Connection = referenceId;
                    }
                }
                tag.Attributes[attr.Name.LocalName] = attrValue;
            }

            tagsWithConnections.Add(tag);
        }

        string json = JsonSerializer.Serialize(tagsWithConnections, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText("output.json", json);
    }
}

