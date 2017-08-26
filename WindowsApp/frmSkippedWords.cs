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

        private void frmSkippedWords_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void LoadList()
        {
            this.dataGridView1.Rows.Clear();
            foreach(string word in Properties.Settings.Default.SkipWords)
            {
                this.dataGridView1.Rows.Add(word, "Remove");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 1)
            {
                string word = (string)this.dataGridView1.Rows[e.RowIndex].Cells[0].Value;
                Properties.Settings.Default.SkipWords.Remove(word);
                this.dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
        }        
    }
}
