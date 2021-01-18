namespace HerbiSpeaks
{
    partial class ImportBoards
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.labelFileName = new System.Windows.Forms.Label();
            this.listBoxBoards = new System.Windows.Forms.CheckedListBox();
            this.labelSelectBoards = new System.Windows.Forms.Label();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(434, 393);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(96, 40);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(322, 393);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(96, 40);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "Import";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(393, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a Herbi Speaks file with the boards you want to import:";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(413, 26);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(98, 34);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "&Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // labelFileName
            // 
            this.labelFileName.AutoSize = true;
            this.labelFileName.Location = new System.Drawing.Point(19, 87);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(109, 17);
            this.labelFileName.TabIndex = 2;
            this.labelFileName.Text = "No file selected.";
            // 
            // listBoxBoards
            // 
            this.listBoxBoards.Enabled = false;
            this.listBoxBoards.FormattingEnabled = true;
            this.listBoxBoards.Location = new System.Drawing.Point(20, 147);
            this.listBoxBoards.Name = "listBoxBoards";
            this.listBoxBoards.ScrollAlwaysVisible = true;
            this.listBoxBoards.Size = new System.Drawing.Size(507, 225);
            this.listBoxBoards.TabIndex = 4;
            // 
            // labelSelectBoards
            // 
            this.labelSelectBoards.AutoSize = true;
            this.labelSelectBoards.Enabled = false;
            this.labelSelectBoards.Location = new System.Drawing.Point(19, 124);
            this.labelSelectBoards.Name = "labelSelectBoards";
            this.labelSelectBoards.Size = new System.Drawing.Size(242, 17);
            this.labelSelectBoards.TabIndex = 3;
            this.labelSelectBoards.Text = "Select the boards you want to import:";
            // 
            // buttonSelectAll
            // 
            this.buttonSelectAll.Location = new System.Drawing.Point(22, 393);
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Size = new System.Drawing.Size(135, 33);
            this.buttonSelectAll.TabIndex = 5;
            this.buttonSelectAll.Text = "&Select all boards";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // ImportBoards
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 445);
            this.Controls.Add(this.buttonSelectAll);
            this.Controls.Add(this.labelSelectBoards);
            this.Controls.Add(this.listBoxBoards);
            this.Controls.Add(this.labelFileName);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportBoards";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import boards";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label labelFileName;
        private System.Windows.Forms.CheckedListBox listBoxBoards;
        private System.Windows.Forms.Label labelSelectBoards;
        private System.Windows.Forms.Button buttonSelectAll;
    }
}