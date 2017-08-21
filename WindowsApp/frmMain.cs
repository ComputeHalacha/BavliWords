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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Masechta msch in Masechta.MasechtaList)
            {
                TreeNode masectaNode = new TreeNode(msch.NameEnglish);
                masectaNode.Tag = msch.NameEnglish;
                for (int i = 2; i <= msch.Dappim; i++)
                {
                    TreeNode daf = new TreeNode(i.ToString() + " / " + Program.ToNumberHeb(i)),
                        amudA = new TreeNode("Amud Alef - עמוד א"),
                        amudB = new TreeNode("Amud Bais - עמוד ב");
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
        
        private void AddNode(TreeNode node)
        {
            var item = new ListViewItem();
            item.Text = Program.ParseTag((string)node.Tag);
            item.Name = (string)node.Tag;            
            this.listView1.Items.Add(item);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var list = new List<string>();
            foreach (ListViewItem item in this.listView1.Items)
            {
                list.Add(item.Name);
            }
            var html = Program.getHtml(list, (int)this.numericUpDown1.Value, (int)this.numericUpDown2.Value);
            frmBrowser fb = new frmBrowser();
            fb.webBrowser1.DocumentText = Properties.Resources.HtmlTemplate.Replace("<!--RESULTS-->", html);
            fb.Show();
        }        

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                this.AddNode(e.Node);
            }
            else
            {
                this.listView1.Items.RemoveByKey((string)e.Node.Tag);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView1.SelectedItems)
            {
                this.listView1.Items.Remove(item);
            }
        }
    }
}
