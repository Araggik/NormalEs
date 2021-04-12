using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Es
{
    public partial class Form6 : Form
    {
        public void CreateTree(TreeNode curnode, SmartFact curfact)
        {
            if(curfact.irule <0)
            {
                curnode.Nodes.Add("Запрошена");
            }
            else
            {
                curnode.Nodes.Add(list_rule[curfact.irule].description);
                foreach (var el in list_facts)
                {
                    if (el.iparent == curfact.irule)
                    {
                        TreeNode newnode = new TreeNode("Цель: " + el.fact.variable.name + " = " + el.fact.value);
                        CreateTree(newnode, el);
                        curnode.Nodes.Add(newnode);
                    }
                }
            }                        
        }

        public List<SmartFact> list_facts;

        public List<Rule> list_rule;

        public Form6()
        {
            InitializeComponent();
        }

        private void Form6_Shown(object sender, EventArgs e)
        {
            SmartFact main_goal = list_facts[list_facts.Count - 1];
            TreeNode head = new TreeNode("Цель: "+main_goal.fact.variable.name+" = "+main_goal.fact.value);
            CreateTree(head,main_goal);
            treeView1.Nodes.Add(head);
        }
    }
}
