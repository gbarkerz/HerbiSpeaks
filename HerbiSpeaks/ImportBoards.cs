using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace HerbiSpeaks
{
    public partial class ImportBoards : Form
    {
        private HerbiSpeaks _mainForm;

        public ImportBoards(HerbiSpeaks mainForm)
        {
            this._mainForm = mainForm;

            InitializeComponent();

            this.buttonOK.Enabled = true;

            this.listBoxBoards.CheckOnClick = true;
            this.listBoxBoards.ItemCheck += listBoxBoards_ItemCheck;
        }

        void listBoxBoards_ItemCheck(object sender, ItemCheckEventArgs e)
        {
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.Close();
        }

        private XmlNodeList _boardList;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            if (this.listBoxBoards.CheckedItems.Count > 0)
            {
                for (int i = 0; i < this.listBoxBoards.CheckedItems.Count; ++i)
                {
                    int idx = this.listBoxBoards.Items.IndexOf(this.listBoxBoards.CheckedItems[i]);
                    if (idx >= 0)
                    {
                        _mainForm.LoadBoard(
                            _boardList[idx],
                            "", // Unused filepath
                            -1); // If this is zero, the buttons on the board are visible.
                    }
                }

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }

            this.Close();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Herbi Speaks Files|*.hrb";

            if (_mainForm._mostRecentFileOpenFolder != "")
            {
                dlg.InitialDirectory = _mainForm._mostRecentFileOpenFolder;
            }
            else
            {
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\HerbiSpeaks";
            }

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                LoadFileBoards(dlg.FileName);

                _mainForm._mostRecentFileOpenFolder = Path.GetDirectoryName(dlg.FileName);
                Settings1.Default.MostRecentFileOpenFolder = _mainForm._mostRecentFileOpenFolder;
            }
        }

        private void LoadFileBoards(string fileName)
        {
            // Can we open the file?
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);

                labelFileName.Text = Path.GetFileNameWithoutExtension(fileName);

                listBoxBoards.Items.Clear();

                listBoxBoards.Enabled = false;
                labelSelectBoards.Enabled = false;
                buttonSelectAll.Enabled = false;

                XmlNode appData = doc.SelectSingleNode("HerbiSpeaks");

                XmlNode boards = appData.SelectSingleNode("Boards");
                if (boards != null)
                {
                    _boardList = boards.SelectNodes("Board");
                    for (int i = 0; i < _boardList.Count; ++i)
                    {
                        string boardName = "Default";

                        XmlNode nodeBoard = _boardList[i].Attributes.GetNamedItem("Name");
                        if (nodeBoard != null)
                        {
                            boardName = nodeBoard.Value;
                        }

                        listBoxBoards.Items.Add(boardName);
                    }

                    if (listBoxBoards.Items.Count > 0)
                    {
                        listBoxBoards.Enabled = true;
                        labelSelectBoards.Enabled = true;
                        buttonSelectAll.Enabled = true;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    this,
                    "The selected file could not be opened. Please check that the file is not already open in another app.",
                    "Herbi Speaks",
                    MessageBoxButtons.OK);
            }
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listBoxBoards.Items.Count; ++i)
            {
                this.listBoxBoards.SetItemChecked(i, true);
            }
        }
    }
}
