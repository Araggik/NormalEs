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
    public partial class Form5 : Form
    {
        public ExpertSys Es;

        public List<SmartFact> checked_facts;

        //МЛВ
        public void Conclusion()
        {
            string name_target = comboBox1.Text;
            //comboBox1.Enabled = false;
            int len = Es.variables.Count;
            bool notfind_target = true;
            Variable target;
            int curvar = 0;
            while(notfind_target)
            {
                if(name_target==Es.variables[curvar].name)
                {
                    notfind_target = false;
                    target = Es.variables[curvar];
                }
                curvar++;
            }
            //Список проверенных фактов
            //checked_facts = new List<SmartFact>();
            //Список использованных правил, при проверке текущего факта
            List<int> used_rules = new List<int>();
            //Стек непроверенных правил
            Stack<Fact> unchecked_facts= new Stack<Fact>();
            //Перем. для определения найдена ли цель
            bool notvalue = true;
            //Перем. для определения прошел ли полный перебор
            bool notall = true;
            //Перем. для определения закончилось ли взаимодествие с пользователем
            bool notstop = true;
            int currule = 0;
            //Текущее начальное правило при обратном выводе
            int rule_target = 0;
            //Индекс последнего правила
            int lastrule = Es.rules.Count - 1;
            while(notvalue && notall && notstop)
            {
                //Есть ли в заключении правила наша цель
                int id_used_fact_target =Es.rules[rule_target].concl.FindIndex(
                    delegate (Fact x)
                    { 
                        return x.variable.name == name_target; 
                    });
                if(id_used_fact_target !=-1)
                {
                    used_rules.Add(rule_target);
                    int count_prem = Es.rules[rule_target].prem.Count;
                    for(int i=count_prem-1;i>=0;i--)
                    {
                        //Добавление первых посылок в стек непроверенных правил
                        unchecked_facts.Push(Es.rules[rule_target].prem[i]);
                    }
                    //Переменная для полного перебора для текущего правила в вершине
                    bool notcheck_all = true;
                    int del_rule = -1;
                    while (notcheck_all && unchecked_facts.Count != 0 && notstop)
                    {
                        //Достаем следующее проверяемое правило, основное действие в цикле
                        Fact curused_fact = unchecked_facts.Peek();

                        int count_check = checked_facts.Count;
                        //Есть ли уже проверенный факт с такой же переменной
                        bool ischecked = false;
                        int j;
                        for (j = 0; j < count_check && !ischecked; j++)
                        {
                            if (checked_facts[j].fact.variable.name == curused_fact.variable.name)
                            {
                                ischecked = true;
                            }
                        }
                        j--;
                        //Доказали ли мы факт через уже проверенные факты (смотрим на последнее правило в списке использованных правил)
                        bool isproof = false;
                        List<int> list = new List<int>();
                        bool isconcl = false;
                        for(int i=0;i < Es.rules[used_rules[used_rules.Count - 1]].concl.Count && !isconcl; i++)
                        {
                            if (Es.rules[used_rules[used_rules.Count - 1]].concl[i].variable.name == curused_fact.variable.name)
                            {
                                if (Es.rules[used_rules[used_rules.Count - 1]].concl[i].value == curused_fact.value)
                                {
                                    isconcl = true;
                                }
                            }
                        }
                        if (isconcl)
                        {
                            for (int i = 0; i < Es.rules[used_rules[used_rules.Count - 1]].prem.Count && !isproof; i++)
                            {
                                for (int t = 0; t < checked_facts.Count && !isproof; t++)
                                {
                                    if (Es.rules[used_rules[used_rules.Count - 1]].prem[i].variable.name == checked_facts[t].fact.variable.name)
                                    {
                                        if (Es.rules[used_rules[used_rules.Count - 1]].prem[i].value == checked_facts[t].fact.value)
                                        {
                                            list.Add(checked_facts[t].irule);
                                            isproof = true;
                                        }
                                    }
                                }
                                isproof = !isproof;
                            }
                        }
                        else
                        {
                            isproof = true;
                        }
                        if(!isproof)
                        {
                            SmartFact newfact = new SmartFact(curused_fact, used_rules[used_rules.Count - 2], list, used_rules[used_rules.Count - 1], id_used_fact_target);
                            checked_facts.Add(newfact);
                            unchecked_facts.Pop();
                            used_rules.RemoveAt(used_rules.Count - 1);
                        }
                        else if(ischecked) //Есть ли уже проверенный факт с такой же переменной
                        {
                            //Если значения переменных совпадает
                            if(checked_facts[j].fact.value == curused_fact.value)
                            {
                                checked_facts[j].ChangeP(used_rules[used_rules.Count - 1]);
                                unchecked_facts.Pop();
                            }
                            else //Если значения переменных не совпадает (то надо будет вернуться к вершине родителю т е удалить последнее правило и доказываемые с ним факты)
                            {
                                del_rule = used_rules[used_rules.Count - 1];
                                int count_prem_facts = Es.rules[del_rule].prem.Count;
                                int c = checked_facts.Count;
                                int count_checked_prem_facts = 0;
                                for (int i = 0; i < c; i++)
                                {
                                    if (checked_facts[i].iparent == del_rule)
                                    {
                                        count_checked_prem_facts++;
                                        checked_facts[i].ChangeP(-1);
                                    }
                                }
                                int del_uncheck_facts = count_prem_facts - count_checked_prem_facts;
                                if (del_uncheck_facts > unchecked_facts.Count)
                                {
                                    del_uncheck_facts = unchecked_facts.Count;
                                }
                                for (int i = 0; i < del_uncheck_facts; i++)
                                {
                                    unchecked_facts.Pop();
                                }
                                used_rules.RemoveAt(used_rules.Count - 1);
                            }
                        }
                        else if (curused_fact.variable.type == 1) //Если запращиваемая переменная то открываем новую форму
                        {
                            Form8 form8 = new Form8();
                            foreach (string a in curused_fact.variable.domain.values)
                            {
                                form8.choices.Add(a);
                            }
                            form8.q = curused_fact.variable.quest;
                            var res = form8.ShowDialog();
                            if (res == DialogResult.OK)
                            {
                                string choice = form8.choice;
                                List<int> lkids = new List<int>();
                                if (curused_fact.value != choice)//Совпадает ли ответ пользователя с проверяемым фактом
                                {
                                    //Если нет то придеться делать откат от последнего правила и запоминать новый факт
                                    del_rule = used_rules[used_rules.Count - 1];
                                    int count_prem_facts = Es.rules[del_rule].prem.Count;
                                    int c = checked_facts.Count;
                                    int count_checked_prem_facts = 0;
                                    for (int i = 0; i < c; i++)
                                    {
                                        if (checked_facts[i].iparent == del_rule)
                                        {
                                            count_checked_prem_facts++;
                                            checked_facts[i].ChangeP(-1);
                                        }
                                    }
                                    int del_uncheck_facts = count_prem_facts - count_checked_prem_facts;
                                    if(del_uncheck_facts>unchecked_facts.Count)
                                    {
                                        del_uncheck_facts = unchecked_facts.Count;
                                    }
                                    for (int i = 0; i < del_uncheck_facts; i++)
                                    {
                                        unchecked_facts.Pop();
                                    }
                                    Fact addfact = new Fact(curused_fact.variable, choice);
                                    SmartFact newfact = new SmartFact(addfact, -1, lkids, -1, id_used_fact_target);
                                    checked_facts.Add(newfact);
                                    used_rules.RemoveAt(used_rules.Count - 1);
                                }
                                else //Иначе просто добавляем новы проверенных факт и переходим к след итерации в цикле
                                {
                                    SmartFact newfact = new SmartFact(curused_fact, used_rules[used_rules.Count - 1], lkids, -1, id_used_fact_target);
                                    checked_facts.Add(newfact);
                                    unchecked_facts.Pop();
                                }
                            }
                            else
                            {
                                notstop = false;
                            }
                            form8.Close();
                        }
                        else
                        {   //Находим правило в котором текущий факт будет в заключении правила
                            bool notnext = true;
                            currule = del_rule+1;
                            if (currule != 0) del_rule = -1;
                            while (currule<=lastrule&&notnext)
                            {
                                if(!used_rules.Contains(currule))
                                {
                                    for(int i=0;i<Es.rules[currule].concl.Count&&notnext;i++)
                                    {
                                        if(Es.rules[currule].concl[i].value == curused_fact.value&& Es.rules[currule].concl[i].variable.name == curused_fact.variable.name)
                                        {
                                            notnext = false;
                                        }
                                    }//Если нашлось такое правило то добавляем его в список, а связанные с ним непроверные факты в стек
                                    if (!notnext)
                                    {
                                        used_rules.Add(currule);
                                        int count_prem_facts = Es.rules[currule].prem.Count;
                                        for (int i = count_prem_facts - 1; i >= 0; i--)
                                        {
                                            unchecked_facts.Push(Es.rules[currule].prem[i]);
                                        }
                                        currule--;
                                    }
                                }
                                currule++;
                            }
                            if(currule>lastrule)
                            {
                                if(curused_fact.variable.type == 3)//Если выводимо-запращиваемая переменная то открываем новую форму
                                {
                                    Form8 form8 = new Form8();
                                    foreach (string a in curused_fact.variable.domain.values)
                                    {
                                        form8.choices.Add(a);
                                    }
                                    form8.q = curused_fact.variable.quest;
                                    var res = form8.ShowDialog();
                                    if (res == DialogResult.OK)
                                    {
                                        string choice = form8.choice;
                                        List<int> lkids = new List<int>();
                                        if (curused_fact.value != choice)
                                        {
                                            del_rule = used_rules[used_rules.Count - 1];
                                            int count_prem_facts = Es.rules[del_rule].prem.Count;
                                            int c = checked_facts.Count;
                                            int count_checked_prem_facts = 0;
                                            for (int i = 0; i < c; i++)
                                            {
                                                if (checked_facts[i].iparent == del_rule)
                                                {
                                                    count_checked_prem_facts++;
                                                    checked_facts[i].ChangeP(-1);
                                                }
                                            }
                                            int del_uncheck_facts = count_prem_facts - count_checked_prem_facts;
                                            if (del_uncheck_facts > unchecked_facts.Count)
                                            {
                                                del_uncheck_facts = unchecked_facts.Count;
                                            }
                                            for (int i = 0; i < del_uncheck_facts; i++)
                                            {
                                                unchecked_facts.Pop();
                                            }
                                            Fact addfact = new Fact(curused_fact.variable, choice);
                                            SmartFact newfact = new SmartFact(addfact, -1, lkids, -1, id_used_fact_target);
                                            checked_facts.Add(newfact);
                                            used_rules.RemoveAt(used_rules.Count - 1);
                                        }
                                        else
                                        {
                                            SmartFact newfact = new SmartFact(curused_fact, used_rules[used_rules.Count - 1], lkids, -1, id_used_fact_target);
                                            checked_facts.Add(newfact);
                                            unchecked_facts.Pop();
                                        }                                        
                                    }
                                    else
                                    {
                                        notstop = false;
                                    }
                                    form8.Close();
                                }
                                else
                                {
                                    if(used_rules.Count ==1)
                                    {
                                        notcheck_all = false;
                                    }
                                    else
                                    {
                                        del_rule = used_rules[used_rules.Count - 1];
                                        int count_prem_facts = Es.rules[del_rule].prem.Count;
                                        int c = checked_facts.Count;
                                        int count_checked_prem_facts = 0;
                                        for (int i = 0; i < c; i++)
                                        {
                                            if (checked_facts[i].iparent == del_rule)
                                            {
                                                count_checked_prem_facts++;
                                                checked_facts[i].ChangeP(-1);
                                            }
                                        }
                                        int del_uncheck_facts = count_prem_facts - count_checked_prem_facts;
                                        for (int i = 0; i < del_uncheck_facts; i++)
                                        {
                                            unchecked_facts.Pop();
                                        }
                                        used_rules.RemoveAt(used_rules.Count - 1);
                                    }
                                }
                            }
                        }
                    }
                    if(unchecked_facts.Count==0&&used_rules.Count>0)//Добавляем цель в список проверенных фактов
                    {
                        notvalue = false;
                        List<int> lkids = new List<int>();                        
                        for(int i=0;i<checked_facts.Count;i++)
                        {
                            if(checked_facts[i].iparent==rule_target)
                            {
                                lkids.Add(checked_facts[i].irule);
                            }
                        }
                        SmartFact newfact = new SmartFact(Es.rules[rule_target].concl[id_used_fact_target], -2, lkids, rule_target, id_used_fact_target);
                        checked_facts.Add(newfact);
                    }
                    used_rules.Clear();
                    unchecked_facts.Clear();
                }
                rule_target++;
                if(rule_target>lastrule)
                {
                    notall = false;
                }
            }
            if(!notall||!notstop)
            {
                label1.Text = "Цель не достигнута";
                button3.Visible = false;
            }
            else
            {
                label1.Text = checked_facts[checked_facts.Count - 1].fact.variable.name + " = " + checked_facts[checked_facts.Count - 1].fact.value;
                button3.Visible = true;
            }
        }

        public Form5()
        {
            InitializeComponent();
            Es = null;
            button3.Visible = false;
            checked_facts = new List<SmartFact>();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form5_Shown(object sender, EventArgs e)
        {
            int len = Es.variables.Count;
            for(int i=0;i<len;i++)
            {
                if(Es.variables[i].type == 2)
                {
                    comboBox1.Items.Add(Es.variables[i].name);
                }
            }
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            checked_facts.Clear();
            //МЛВ
            Conclusion();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            form6.list_facts = checked_facts;
            form6.list_rule = Es.rules;
            var res = form6.ShowDialog();
        }
    }
}
