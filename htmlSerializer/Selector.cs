using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace htmlSerializer
{
    internal class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Children { get; set; }

        public static Selector BuildSelector(string selectorString)
        {
            if (string.IsNullOrWhiteSpace(selectorString))
                throw new ArgumentException("Selector string cannot be null or empty.");
            Selector selector = new Selector();
            Selector root = null;
            Selector temp = null;
            List<string> parts = selectorString.Split(' ').ToList();
            foreach (string part in parts)
            {
                var attributes = new Regex("^(.*?)?(#.*?)?(\\..*?)?$").Matches(part);
                if (HtmlHelper.Instance.Tags.Contains(attributes[0].Groups[1].Value))
                    selector.TagName = attributes[0].Groups[1].Value;
                if (attributes[0].Groups[2].Value.StartsWith("#"))
                    selector.Id = attributes[0].Groups[2].Value.Substring(1);
                if (attributes[0].Groups[3].Value.StartsWith("."))
                {
                    selector.Classes = new List<string>();
                    selector.Classes = attributes[0].Groups[3].Value.Split('.').ToList();
                    selector.Classes.Remove("");
                }
                if (root == null)
                {
                    root = selector;
                    root.Parent = null;
                    temp = root;
                }
                else
                {
                    selector.Parent = temp;
                    temp.Children = selector;
                    temp = selector;
                }
                selector = new Selector();
            }
            return root;
        }
    }
}
