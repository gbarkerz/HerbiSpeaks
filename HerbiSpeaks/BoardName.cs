using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HerbiSpeaks
{
    public partial class BoardName : Form
    {
        HerbiSpeaks _mainForm;
        string _boardName;

        public BoardName(HerbiSpeaks mainForm, string boardName)
        {
            InitializeComponent();

            _mainForm = mainForm;
            _boardName = boardName;
        }

        public string BoardNameResult
        {
            get { return _boardName; }
            set { _boardName = value; }
        }

        private void BoardName_Load(object sender, EventArgs e)
        {
            textBoxBoardName.Text = _boardName;
            textBoxBoardName.SelectAll();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            _boardName = textBoxBoardName.Text;

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.Close();
        }
    }
}
