using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es
{
    [Serializable]
    public class Variable
    {
        public string name;
        public Domain domain;
        public int type;
        public string quest;

        public Variable(string name, Domain domain, int type, string quest)
        {
            this.name = name;
            this.domain = domain;
            this.type = type;
            this.quest = quest;
        }
    }
}
