using System;

namespace HerbiSpeaks
{
    partial class HerbiSpeaks
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HerbiSpeaks));
            this.menuStripApp = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setbackgroundColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultButtontextColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDefaultfontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultButtonBorderThicknessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameCurrentBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteCurrentBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importboardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            try
            {
                this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("WMP failed to create. " + ex.Message);

                axWindowsMediaPlayer1 = null;
            }

            this.menuStripApp.SuspendLayout();

            if (axWindowsMediaPlayer1 != null)
            {
                ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            }

            this.SuspendLayout();
            // 
            // menuStripApp
            // 
            this.menuStripApp.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripApp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.boardsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripApp.Location = new System.Drawing.Point(0, 0);
            this.menuStripApp.Name = "menuStripApp";
            this.menuStripApp.Padding = new System.Windows.Forms.Padding(12, 2, 0, 2);
            this.menuStripApp.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStripApp.Size = new System.Drawing.Size(1268, 42);
            this.menuStripApp.TabIndex = 0;
            this.menuStripApp.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveasToolStripMenuItem,
            this.toolStripSeparator2,
            this.printToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(64, 38);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(330, 38);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(330, 38);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(330, 38);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveasToolStripMenuItem
            // 
            this.saveasToolStripMenuItem.Name = "saveasToolStripMenuItem";
            this.saveasToolStripMenuItem.Size = new System.Drawing.Size(330, 38);
            this.saveasToolStripMenuItem.Text = "Save &as...";
            this.saveasToolStripMenuItem.Click += new System.EventHandler(this.saveasToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(327, 6);
            this.toolStripSeparator2.Visible = false;
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Enabled = false;
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(330, 38);
            this.printToolStripMenuItem.Text = "&Print current board...";
            this.printToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(327, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(330, 38);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullScreenToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(78, 38);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            this.fullScreenToolStripMenuItem.Size = new System.Drawing.Size(267, 38);
            this.fullScreenToolStripMenuItem.Text = "&Fullscreen (F5)";
            this.fullScreenToolStripMenuItem.Click += new System.EventHandler(this.fullScreenToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setbackgroundColourToolStripMenuItem,
            this.defaultButtontextColourToolStripMenuItem,
            this.setDefaultfontToolStripMenuItem,
            this.defaultButtonBorderThicknessToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(111, 38);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // setbackgroundColourToolStripMenuItem
            // 
            this.setbackgroundColourToolStripMenuItem.Name = "setbackgroundColourToolStripMenuItem";
            this.setbackgroundColourToolStripMenuItem.Size = new System.Drawing.Size(407, 38);
            this.setbackgroundColourToolStripMenuItem.Text = "&Background colour...";
            this.setbackgroundColourToolStripMenuItem.Click += new System.EventHandler(this.setbackgroundColourToolStripMenuItem_Click);
            // 
            // defaultButtontextColourToolStripMenuItem
            // 
            this.defaultButtontextColourToolStripMenuItem.Name = "defaultButtontextColourToolStripMenuItem";
            this.defaultButtontextColourToolStripMenuItem.Size = new System.Drawing.Size(407, 38);
            this.defaultButtontextColourToolStripMenuItem.Text = "Default button &text colour...";
            this.defaultButtontextColourToolStripMenuItem.Click += new System.EventHandler(this.defaultButtontextColourToolStripMenuItem_Click);
            // 
            // setDefaultfontToolStripMenuItem
            // 
            this.setDefaultfontToolStripMenuItem.Name = "setDefaultfontToolStripMenuItem";
            this.setDefaultfontToolStripMenuItem.Size = new System.Drawing.Size(407, 38);
            this.setDefaultfontToolStripMenuItem.Text = "Default button text &font...";
            this.setDefaultfontToolStripMenuItem.Click += new System.EventHandler(this.setDefaultfontToolStripMenuItem_Click);
            // 
            // defaultButtonBorderThicknessToolStripMenuItem
            // 
            this.defaultButtonBorderThicknessToolStripMenuItem.Name = "defaultButtonBorderThicknessToolStripMenuItem";
            this.defaultButtonBorderThicknessToolStripMenuItem.Size = new System.Drawing.Size(407, 38);
            this.defaultButtonBorderThicknessToolStripMenuItem.Text = "B&utton border thickness...";
            this.defaultButtonBorderThicknessToolStripMenuItem.Click += new System.EventHandler(this.defaultButtonBorderThicknessToolStripMenuItem_Click);
            // 
            // boardsToolStripMenuItem
            // 
            this.boardsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addBoardToolStripMenuItem,
            this.makeToolStripMenuItem,
            this.renameCurrentBoardToolStripMenuItem,
            this.deleteCurrentBoardToolStripMenuItem,
            this.reorderBoardToolStripMenuItem,
            this.toolStripSeparator1,
            this.importboardsToolStripMenuItem,
            this.toolStripSeparator4});
            this.boardsToolStripMenuItem.Name = "boardsToolStripMenuItem";
            this.boardsToolStripMenuItem.Size = new System.Drawing.Size(99, 38);
            this.boardsToolStripMenuItem.Text = "&Boards";
            this.boardsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.boardsToolStripMenuItem_Opening);
            // 
            // addBoardToolStripMenuItem
            // 
            this.addBoardToolStripMenuItem.Name = "addBoardToolStripMenuItem";
            this.addBoardToolStripMenuItem.Size = new System.Drawing.Size(503, 38);
            this.addBoardToolStripMenuItem.Text = "&Add board";
            this.addBoardToolStripMenuItem.Click += new System.EventHandler(this.addBoardToolStripMenuItem_Click);
            // 
            // makeToolStripMenuItem
            // 
            this.makeToolStripMenuItem.Name = "makeToolStripMenuItem";
            this.makeToolStripMenuItem.Size = new System.Drawing.Size(503, 38);
            this.makeToolStripMenuItem.Text = "&Make new board from current board";
            this.makeToolStripMenuItem.Click += new System.EventHandler(this.makeToolStripMenuItem_Click);
            // 
            // renameCurrentBoardToolStripMenuItem
            // 
            this.renameCurrentBoardToolStripMenuItem.Name = "renameCurrentBoardToolStripMenuItem";
            this.renameCurrentBoardToolStripMenuItem.Size = new System.Drawing.Size(503, 38);
            this.renameCurrentBoardToolStripMenuItem.Text = "&Rename current board";
            this.renameCurrentBoardToolStripMenuItem.Click += new System.EventHandler(this.renameCurrentBoardToolStripMenuItem_Click);
            // 
            // deleteCurrentBoardToolStripMenuItem
            // 
            this.deleteCurrentBoardToolStripMenuItem.Name = "deleteCurrentBoardToolStripMenuItem";
            this.deleteCurrentBoardToolStripMenuItem.Size = new System.Drawing.Size(503, 38);
            this.deleteCurrentBoardToolStripMenuItem.Text = "&Delete current board";
            this.deleteCurrentBoardToolStripMenuItem.Click += new System.EventHandler(this.deleteCurrentBoardToolStripMenuItem_Click);
            // 
            // reorderBoardToolStripMenuItem
            // 
            this.reorderBoardToolStripMenuItem.Name = "reorderBoardToolStripMenuItem";
            this.reorderBoardToolStripMenuItem.Size = new System.Drawing.Size(503, 38);
            this.reorderBoardToolStripMenuItem.Text = "Re&order boards";
            this.reorderBoardToolStripMenuItem.Click += new System.EventHandler(this.reorderBoardToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(500, 6);
            // 
            // importboardsToolStripMenuItem
            // 
            this.importboardsToolStripMenuItem.Name = "importboardsToolStripMenuItem";
            this.importboardsToolStripMenuItem.Size = new System.Drawing.Size(503, 38);
            this.importboardsToolStripMenuItem.Text = "Import &boards";
            this.importboardsToolStripMenuItem.Click += new System.EventHandler(this.importBoardToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(500, 6);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(77, 38);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(179, 38);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // axWindowsMediaPlayer1
            // 
            if (this.axWindowsMediaPlayer1 != null)
            {
                this.axWindowsMediaPlayer1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
                this.axWindowsMediaPlayer1.Enabled = true;
                this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(13, 483);
                this.axWindowsMediaPlayer1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
                this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
                this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
                this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(221, 47);
                this.axWindowsMediaPlayer1.TabIndex = 2;
                this.axWindowsMediaPlayer1.Visible = false;
            }
            // 
            // HerbiSpeaks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1268, 868);

            if (axWindowsMediaPlayer1 != null)
            {
                this.Controls.Add(this.axWindowsMediaPlayer1);
            }

            this.Controls.Add(this.menuStripApp);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripApp;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "HerbiSpeaks";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Herbi Speaks";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStripApp.ResumeLayout(false);
            this.menuStripApp.PerformLayout();

            if (axWindowsMediaPlayer1 != null)
            {
                try
                {
                    ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Media EndInit failed. " + ex.Message);

                    axWindowsMediaPlayer1 = null;
                }
            }

            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripApp;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setbackgroundColourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setDefaultfontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultButtontextColourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultButtonBorderThicknessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boardsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameCurrentBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorderBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem makeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem deleteCurrentBoardToolStripMenuItem;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.ToolStripMenuItem importboardsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;

    }
}

