// See https://aka.ms/new-console-template for more information

using htmlSerializer;
using Newtonsoft.Json.Linq;
using System.Formats.Asn1;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Sources;

static void Print(Selector element, int depth)
{
    Console.WriteLine(new string(' ', depth * 2) + element.TagName + $"{element.Id} + {element.Classes?.First()}");
    if (element.Children != null)
    {


        Print(element.Children, depth + 1);

    }

}
Print(Selector.FromString("div#my.clas a #ddd.ff.qqq"), 0);


static void PrintTree(HtmlElement element, int depth)
{
    Console.WriteLine(new string(' ', depth * 2) + element.Name);
    if (element.Children != null)
    {

        foreach (var child in element.Children)
        {
            PrintTree(child, depth + 1);
        }
    }
    Console.WriteLine(new string('-', depth * 2) + element.Name);
}

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}


var html = await Load("https://learn.malkabruk.co.il/");

var cleanHtml = new Regex("\\s+").Replace(html, " ");

var htmlWords = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 1);

HtmlElement root = null, temp = null, newElement;

foreach (var line in htmlWords)
{
    var wordsLine = new Regex(" ").Split(line).Where(s => s.Length > 0);

    if (HtmlHelper.Instance.Tags.Contains(wordsLine.First()))
    {
        var tag = wordsLine.First();
        if (root == null)
        {
            root = new HtmlElement() { Name = tag, Parent = null };
            var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
            foreach (Match attribute in attributes)
            {
                if (attribute.Groups[1].Value == "id")
                    root.Id = attribute.Groups[2].Value;
                else if (attribute.Groups[1].Value == "class")
                    root.Classes = new List<string>(attribute.Groups[2].Value.Split(' '));
                else
                    root.Attributes = new List<string>(attribute.Groups[0].Value.Split(' '));
            }
            temp = root;
        }
        else
        {
            if (tag == "html")
            {
                continue;
            }

            newElement = new HtmlElement { Name = tag, Parent = temp };
            if (temp.Children == null)
                temp.Children = new List<HtmlElement> { newElement };
            else
                temp.Children.Add(newElement);
            var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
            foreach (Match attribute in attributes)
            {
                if (attribute.Groups[1].Value == "id")
                    newElement.Id = attribute.Groups[2].Value;
                else if (attribute.Groups[1].Value == "class")
                    newElement.Classes = new List<string>(attribute.Groups[2].Value.Split(' '));
                else
                    newElement.Attributes = new List<string>(attribute.Groups[0].Value.Split(' '));
            }
            if (!HtmlHelper.Instance.CloseTags.Contains(tag))
                temp = newElement;

        }
    }
    else if (Regex.IsMatch(wordsLine.First(), @"^/[^/\s]+") && HtmlHelper.Instance.Tags.Contains(wordsLine.First().Substring(1)))
    {
        if (wordsLine.First() != "/html")
            temp = temp.Parent;
    }
    else if (temp != null)
    {
        temp.InnerHtml = line;
    }

}
//PrintTree(head, 0);

Console.ReadLine();
