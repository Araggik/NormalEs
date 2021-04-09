using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es
{
    [Serializable]
    public class Rule
    {
        public string name;
        public List<Fact> prem;
        public List<Fact> concl;
        public string description;

        public Rule()
        {
            this.prem = new List<Fact>();
            this.concl = new List<Fact>();
        }

        public Rule(string name, List<Fact> prem, List<Fact> concl, string des)
        {
            this.name = name;
            this.prem = prem;
            this.concl = concl;
            this.description = des;
        }
    }
}
