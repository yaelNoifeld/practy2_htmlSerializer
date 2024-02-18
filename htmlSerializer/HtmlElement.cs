using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htmlSerializer
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> q = new Queue<HtmlElement>();
            HtmlElement temp;
            q.Enqueue(this);
            while (q.Count > 0)
            {
                temp = q.Dequeue();
                foreach (HtmlElement el in temp.Children)
                {
                    q.Enqueue(el);
                    yield return el;
                }
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement temp = this;
            while (temp.Parent != null)
            {
                temp = this.Parent;
                yield return temp;
            }
        }
        private bool isMatch(HtmlElement element, Selector selector)
        {
            if ((selector.TagName != null && selector.TagName == element.Name) || selector.TagName == null)
            {
                if ((selector.Id != null && selector.Id == element.Id) || selector.Id == null)
                {
                    foreach (string str in selector.Classes)
                    {
                        if (!element.Classes.Contains(str)) ;
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
        public List<HtmlElement> FindElements(Selector selector)
        {
            HashSet<HtmlElement> res = new HashSet<HtmlElement>();
            FindElements(this, selector, res);
            foreach (HtmlElement el in this.Descendants())
            {
                FindElements(el, selector, res);
            }
            return res.ToList();
        }
        private bool FindElements(HtmlElement element, Selector selector, HashSet<HtmlElement> list)
        {
            if (element == null)
                return false;
            if (isMatch(element, selector))
            {
                if (selector.Children == null)
                {
                    if (selector.Parent == null)
                        list.Add(element);
                    return true;
                }
                foreach (HtmlElement child in element.Descendants())
                {
                    if (FindElements(child, selector.Children, list))
                    {
                        if (selector.Parent == null)
                            list.Add(element);
                        return true;
                    }          
                }
            }
            return false;
        }
    }
}
