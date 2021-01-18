using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Collections.ObjectModel;

namespace HerbiSpeaks
{
    public partial class ReorderBoard : Form
    {
        public Collection<HerbiSpeaksBoard> _boards;
        private Collection<string> _reorderedBoards = new Collection<string>();

        public Collection<string> ReorderedBoards
        {
            get { return _reorderedBoards; }
            set { _reorderedBoards = value; }
        }

        public ReorderBoard(Collection<HerbiSpeaksBoard> boards)
        {
            _boards = boards;

            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBoxBoards.Items.Count; ++i)
            {
                _reorderedBoards.Add(listBoxBoards.Items[i].ToString());
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReorderBoard_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < _boards.Count; ++i)
            {
                listBoxBoards.Items.Add(_boards[i].Name);
            }
        }

        private void listBoxBoards_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxBoards.SelectedIndex;

            buttonMoveDown.Enabled = (index < listBoxBoards.Items.Count - 1);
            buttonMoveUp.Enabled = (index > 0);
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            MoveItem(true);
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            MoveItem(false);
        }

        private void MoveItem(bool moveUp)
        {
            object obj = listBoxBoards.SelectedItem;

            int index = listBoxBoards.SelectedIndex;
            listBoxBoards.Items.RemoveAt(index);

            int newIndex;

            if (moveUp)
            {
                newIndex = Math.Max(0, index - 1);
            }
            else
            {
                newIndex = Math.Min(listBoxBoards.Items.Count, index + 1);
            }

            listBoxBoards.Items.Insert(newIndex, obj);

            listBoxBoards.SelectedIndex = newIndex;
        }
    }
}
