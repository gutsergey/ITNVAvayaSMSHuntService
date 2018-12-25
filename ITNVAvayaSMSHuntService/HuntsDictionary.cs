using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ITNVAvayaSMSHuntService
{
    public class Element
    {
        private string name;
        private string number;
        private string groupextension;
        private List<string> extensions;

        public Element()
        {
            this.extensions = new List<string>();
        }
        public Element(string name, string number, string groupextension, List<string> extensions)
        {
            this.name = name;
            this.number = number;
            this.groupextension = groupextension;
            this.extensions = new List<string>();

            extensions.ForEach((item) => this.extensions.Add(item));
        }

        public string Number { get { return number; } set { number = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Groupextension { get { return groupextension; } set { groupextension = value; } }

        public List<string> Extensions { get { return extensions; } }
    }

    public class HuntsDictionary
    {
        public Dictionary<string, Element> Dic;

        public HuntsDictionary()
        {
            Dic = new Dictionary<string, Element>();
        }


        public void BuildDic(KeyValuePair<string, Hunts.Element> k, List<string> extensions)
        {
            Element e = new Element(k.Value.name, k.Value.number, k.Value.groupextension, extensions);
            Dic.Add(k.Value.groupextension, e);

            
        }
    }
}
