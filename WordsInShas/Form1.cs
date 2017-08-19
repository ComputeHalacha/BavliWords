using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordsInShas
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach(Masechta msch in Program.masechtaList)
            {
                TreeNode masectaNode = new TreeNode(msch.NameEnglish);
                masectaNode.Tag = msch.NameEnglish;
                for(int i = 2; i <= msch.Dappim; i++)
                {
                    TreeNode daf = new TreeNode(i.ToString() + " / " + Program.ToNumberHeb(i)),
                        amudA = new TreeNode("Amud 1 / עמוד א'"),
                        amudB = new TreeNode("Amud 2 / עמוד ב'");
                    daf.Tag = masectaNode.Tag + "," + i.ToString();
                    amudA.Tag = daf.Tag + ",1";
                    amudB.Tag = daf.Tag + ",2";
                    daf.Nodes.Add(amudA);
                    daf.Nodes.Add(amudB);
                    masectaNode.Nodes.Add(daf);
                }
                this.treeView1.Nodes.Add(masectaNode);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] splitTag = ((string)this.treeView1.SelectedNode.Tag).Split(',');
            var item = new ListViewItem();
            if (splitTag.Length == 1)
            {
                item.Text = "The entire Maseches " + splitTag[0];
            }
            else if (splitTag.Length == 2)
            {
                item.Text ="Maseches " + splitTag[0] + ", The entire Daf " + splitTag[1] + " / " + Program.ToNumberHeb(Convert.ToInt32(splitTag[1]));
            }
            else if (splitTag.Length == 3)
            {
                item.Text = "Maseches " + splitTag[0] + ", Daf " + splitTag[1] + " / " + Program.ToNumberHeb(Convert.ToInt32(splitTag[1])) + ", Amud " + splitTag[2] + " / " + Program.ToNumberHeb(Convert.ToInt32(splitTag[2]));
            }

            item.Tag = this.treeView1.SelectedNode.Tag;
            this.listView1.Items.Add(item);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView1.SelectedItems)
            {
                this.listView1.Items.Remove(item);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var list = new List<string>();
            foreach(ListViewItem item in this.listView1.Items)
            {
                list.Add((string)item.Tag);
            }
            var html = Program.getHtml(list, (int)this.numericUpDown1.Value);
        }
    }
}
