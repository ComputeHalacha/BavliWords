using System.Windows.Forms;

namespace WordsInShas
{   
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class frmBrowser : Form
    {
        public frmBrowser()
        {
            InitializeComponent();            
            this.webBrowser1.ObjectForScripting = this;
        }

        public void AddWordToSkipped(int wordRank)
        {
            Program.SkipWordByRank(wordRank);
        }
    }
}
