using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es
{
    public class SmartFact
    {
        public Fact fact;
        public int iparent;
        public List<int> kids;
        public int irule;
        public int itarget;

        public SmartFact(Fact f, int ip, List<int> k, int ir, int it)
        {
            this.fact = f;
            this.iparent = ip;
            this.kids = k;
            this.irule = ir;
            this.itarget = it;
        }
        public void ChangeP(int ip)
        {
            this.iparent = ip;
        }
    }
}
