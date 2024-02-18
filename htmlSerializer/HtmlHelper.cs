using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;



namespace htmlSerializer
{
    internal class HtmlHelper
    {
        private readonly static HtmlHelper _instance=new HtmlHelper();
        public static HtmlHelper Instance=> _instance; 

        public List<string> Tags { get; set; }
        public List<string> CloseTags { get; set; }

        private HtmlHelper()
        {
            Tags = new List<string>();
            CloseTags = new List<string>();
            var contentAllTags = File.ReadAllText("AllTags.json");
            var contentAllTagsJson = JsonConvert.DeserializeObject<List<string>>(contentAllTags);
            foreach (var tag in contentAllTagsJson)
            {
                Tags.Add(tag);
            }
            var contentSelfClosingTags = File.ReadAllText("SelfClosingTags.json");
            var contentSelfClosingTagsJson= JsonConvert.DeserializeObject<List<string>>(contentSelfClosingTags);
            foreach(var tag in contentSelfClosingTagsJson)
            {
                CloseTags.Add(tag);
            }
        }
    }
}
