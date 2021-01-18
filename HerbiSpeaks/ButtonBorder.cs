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
    public partial class ButtonBorder : Form
    {
        HerbiSpeaks _parentForm;

        public ButtonBorder(HerbiSpeaks parentForm)
        {
            _parentForm = parentForm;

            InitializeComponent();
        }

        private void ButtonBorder_Load(object sender, EventArgs e)
        {
            // The first item is "No border".
            this.comboBoxThickness.SelectedIndex = _parentForm.BorderThickness;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // The first item is "No border".
            _parentForm.BorderThickness = this.comboBoxThickness.SelectedIndex;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.Close();
        }
    }
}
