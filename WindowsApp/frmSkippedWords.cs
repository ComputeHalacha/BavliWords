using System;
using System.Windows.Forms;

namespace WordsInShas
{
    public partial class frmSkippedWords : Form
    {
        public frmSkippedWords()
        {
            InitializeComponent();
        }

        private void frmSkippedWords_Load(object sender, EventArgs e)
        {
            this.LoadList();
        }

        private void LoadList()
        {
            this.listBox1.Items.Clear();
            foreach(string word in Properties.Settings.Default.SkipWords)
            {
                this.listBox1.Items.Add(word);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(string word in this.listBox1.SelectedItems)
            {
                Properties.Settings.Default.SkipWords.Remove(word);               
            }
            Properties.Settings.Default.Save();
            this.LoadList();
        }
    }
}
