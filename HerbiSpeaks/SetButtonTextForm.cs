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
    public partial class SetButtonTextForm : Form
    {
        private Button _currentButton;

        public SetButtonTextForm(Button btn)
        {
            _currentButton = btn;

            InitializeComponent();
        }

        private void SetButtonTextForm_Load(object sender, EventArgs e)
        {
            textBoxCurrentText.Text = _currentButton.Text;
            if (textBoxCurrentText.Text == "")
            {
                textBoxCurrentText.Text = _currentButton.AccessibleName;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // If the previous text on the button was not being shown, do not show the new text.
            // But if no text was available previously on the button at all, do show the new text.
            if ((_currentButton.Text != "") || (_currentButton.AccessibleName == ""))
            {
                _currentButton.Text = textBoxCurrentText.Text;
            }

            // Always set the accessible name of the button with the new text.
            _currentButton.AccessibleName = textBoxCurrentText.Text;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
