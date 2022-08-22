using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace HerbiSpeaks
{
    public partial class HerbiSpeaks
    {
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            HelpAbout aboutDlg = new HelpAbout();
            aboutDlg.ShowDialog(this);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            bool fDoOpen = true;

            if (_fIsDirty)
            {
                bool fCancel = HandleDirtyFile();
                if (fCancel)
                {
                    fDoOpen = false;
                }
            }

            if (fDoOpen)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Herbi Speaks Files|*.hrb";

                if (_mostRecentFileOpenFolder != "")
                {
                    dlg.InitialDirectory = _mostRecentFileOpenFolder;
                }
                else
                {
                    dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\HerbiSpeaks";
                }

                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    LoadHerbiFile(dlg.FileName);
                    
                    _mostRecentFileOpenFolder = Path.GetDirectoryName(dlg.FileName);
                    Settings1.Default.MostRecentFileOpenFolder = _mostRecentFileOpenFolder;
                }
            }
        }

        private void LoadHerbiFile(string fileToOpen)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Clear all the current content.
            ResetContent();

            _currentFilename = fileToOpen;

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(_currentFilename);

                LoadFile(doc, Path.GetDirectoryName(_currentFilename));

                SetCaptionText();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Sorry, this file can't be opened.\r\n\r\nDetails: " + 
                        ex.Message, "Herbi Speaks");                
            }

            Cursor.Current = Cursors.Default;
        }

        private void SetCaptionText()
        {
            string captionText = "Herbi Speaks V3.4";

            if (_currentFilename != "")
            {
                captionText += " - " + Path.GetFileNameWithoutExtension(_currentFilename);
            }

            this.Text = captionText + " (" + _boards[_currentBoardIndex].Name + ")";
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            DoSave();
        }

        private bool DoSave()
        {
            bool fCancel = false;

            if (_currentFilename != "")
            {
                SaveApp();

                _fIsDirty = false;
            }
            else
            {
                fCancel = SaveAs();
            }

            return fCancel;
        }

        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            SaveAs();
        }

        private bool SaveAs()
        {
            bool fCancel = false;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Herbi Speaks Files|*.hrb";

            if (_mostRecentFileOpenFolder != "")
            {
                dlg.InitialDirectory = _mostRecentFileOpenFolder;
            }
            else
            {
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\HerbiSpeaks";
            }

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _currentFilename = dlg.FileName;
                SetCaptionText();

                DoSave();

                _mostRecentFileOpenFolder = Path.GetDirectoryName(dlg.FileName);
                Settings1.Default.MostRecentFileOpenFolder = _mostRecentFileOpenFolder;
            }
            else
            {
                fCancel = true;
            }

            return fCancel;
        }

        private bool HandleDirtyFile()
        {
            bool fCancel = false;

            System.Windows.Forms.DialogResult result;

            result = MessageBox.Show(this, "Would you like to save your current changes?", "Herbi Speaks",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                fCancel = true;
            }
            else if (result == System.Windows.Forms.DialogResult.Yes)
            {
                fCancel = DoSave();
            }
            else if (result == System.Windows.Forms.DialogResult.No)
            {
                _fIsDirty = false;
            }

            return fCancel;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            bool fDoNew = true;

            if (_fIsDirty)
            {
                bool fCancel = HandleDirtyFile();
                if (fCancel)
                {
                    fDoNew = false;
                }
            }

            if (fDoNew)
            {
                ResetContent();

                // Always reset the current filename, in order to prompt the user for a name on next Save.
                _currentFilename = "";
                SetCaptionText();
            }
        }

        private void ResetContent()
        {
            PlayMedia("");

            SetEditMode(false);

            // Only remove the buttons from the form. (Don't remove the menu.)
            for (int i = this.Controls.Count - 1; i >= 0; --i)
            {
                Control ctrl = this.Controls[i];

                if (ctrl as Button != null)
                {
                    this.Controls.RemoveAt(i);
                }
            }

            _boards.Clear();

            // Create a default board.
            AddNewBoard("Default");
            SetCaptionText();

            LoadSettings();
        }

        private void SaveApp()
        {
            SetEditMode(false);

            string filename = _currentFilename;

            FileStream datafilestream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            // Create a new XmlTextWriter.
            XmlTextWriter xwriter = new XmlTextWriter(datafilestream, System.Text.Encoding.UTF8);

            // Write the beginning of the document including the document declaration. 
            xwriter.WriteStartDocument();

            xwriter.WriteStartElement("HerbiSpeaks");

            xwriter.WriteStartElement("Boards");

            foreach (HerbiSpeaksBoard board in _boards)
            {
                xwriter.WriteStartElement("Board");

                xwriter.WriteAttributeString("Name", board.Name);
                xwriter.WriteAttributeString("BackgroundColour", board.BackgroundColor.ToArgb().ToString());

                // Today all boards have the same text color etc.
                xwriter.WriteAttributeString("TextColour", this._colButtonText.ToArgb().ToString());

                if (this._defaultFont != null)
                {
                    xwriter.WriteAttributeString("DefaultFontName", this._defaultFont.Name);
                    xwriter.WriteAttributeString("DefaultFontSize", ((int)this._defaultFont.Size).ToString());
                }
                else
                {
                    xwriter.WriteAttributeString("DefaultFontName", this.Font.Name);
                    xwriter.WriteAttributeString("DefaultFontSize", ((int)this.Font.Size).ToString());
                }

                // Now output all buttons for this board.
                xwriter.WriteStartElement("BoardButtons");

                for (int i = 0; i < this.Controls.Count; ++i)
                {
                    HerbiSpeaksButton btn = this.Controls[i] as HerbiSpeaksButton;
                    if (btn != null)
                    {
                        if (btn.BoardName == board.Name)
                        {
                            xwriter.WriteStartElement("BoardButton");

                            WriteButtonDetails(xwriter, btn);

                            // Is this a picture button?
                            if (btn.IsPictureButton)
                            {
                                xwriter.WriteStartElement("PictureButtonContents");

                                for (int idxContainedBtn = 0; idxContainedBtn < btn.Controls.Count; ++idxContainedBtn)
                                {
                                    HerbiSpeaksButton btnContained = btn.Controls[idxContainedBtn] as HerbiSpeaksButton;
                                    if (btn != null)
                                    {
                                        xwriter.WriteStartElement("BoardButton");

                                        WriteButtonDetails(xwriter, btnContained);
                                        
                                        xwriter.WriteEndElement();
                                    }
                                }

                                xwriter.WriteEndElement();
                            }

                            // End the "BoardButton" element.
                            xwriter.WriteEndElement();
                        }
                    }
                }

                // End the "BoardButtons" element.
                xwriter.WriteEndElement();

                // End the "Board" element.
                xwriter.WriteEndElement();
            }

            // End the Boards element.
            xwriter.WriteEndElement();

            // End the HerbiSpeaks element.
            xwriter.WriteEndDocument();
            xwriter.Close();

            datafilestream.Close();
        }

        private void WriteButtonDetails(XmlTextWriter xwriter, HerbiSpeaksButton btn)
        {
            xwriter.WriteAttributeString("CenterX", btn.CenterX.ToString());
            xwriter.WriteAttributeString("CenterY", btn.CenterY.ToString());

            xwriter.WriteAttributeString("Width", btn.Size.Width.ToString());
            xwriter.WriteAttributeString("Height", btn.Size.Height.ToString());

            xwriter.WriteAttributeString("IsPictureButton", btn.IsPictureButton.ToString());

            xwriter.WriteAttributeString("BoardLink", btn.BoardLink);
            xwriter.WriteAttributeString("BoardLinkSpoken", btn.BoardLinkSpoken.ToString());

            string text = btn.Text;
            if (text == "")
            {
                text = btn.AccessibleName;

                xwriter.WriteAttributeString("ShowText", false.ToString());
            }

            xwriter.WriteAttributeString("Text", text);

            // Always write out the button's font name, font size and text colour, even
            // if any of those things are the same as the app's current default settings.
            xwriter.WriteAttributeString("TextColour", btn.ForeColor.ToArgb().ToString());
            xwriter.WriteAttributeString("FontName", btn.Font.Name);
            xwriter.WriteAttributeString("FontSize", ((int)btn.Font.Size).ToString());
            xwriter.WriteAttributeString("ButtonTransparent", ((bool)btn.ButtonTransparent).ToString());
            xwriter.WriteAttributeString("ButtonTransparentOnHover", ((bool)btn.ButtonTransparentOnHover).ToString());

            if ((btn.Media != null) && (btn.Media != ""))
            {
                xwriter.WriteAttributeString("Media", btn.Media);
            }

            xwriter.WriteAttributeString("ButtonTextSpokenBeforeMedia",
                btn.ButtonTextSpokenBeforeMedia.ToString());

            xwriter.WriteAttributeString("AutoPlayMedia",
                btn.AutoPlayMedia.ToString());

            xwriter.WriteAttributeString("TextPosition", btn.TextPosition);

            // Always stream out the full image. We don't care here how the image 
            // happens to be presented on the button at the moment.
            if (btn.ImageFull != null)
            {
                MemoryStream stream = new MemoryStream();
                btn.ImageFull.Save(stream, ImageFormat.Png);
                byte[] bytes = stream.ToArray();

                xwriter.WriteStartElement("PictureData");
                xwriter.WriteAttributeString("Length", bytes.Length.ToString());
                xwriter.WriteBase64(stream.ToArray(), 0, bytes.Length);
                xwriter.WriteEndElement();
            }

            if (btn.ImageHoverFull != null)
            {
                MemoryStream stream = new MemoryStream();
                btn.ImageHoverFull.Save(stream, ImageFormat.Png);
                byte[] bytes = stream.ToArray();

                xwriter.WriteStartElement("HoverPictureData");
                xwriter.WriteAttributeString("Length", bytes.Length.ToString());
                xwriter.WriteBase64(stream.ToArray(), 0, bytes.Length);
                xwriter.WriteEndElement();
            }
        }

        private void LoadFile(XmlDocument doc, string filePath)
        {
            _boards.Clear();

            SetEditMode(false);

            XmlNode appData = doc.SelectSingleNode("HerbiSpeaks");

            XmlNode boards = appData.SelectSingleNode("Boards");
            if (boards != null)
            {
                XmlNodeList boardList = boards.SelectNodes("Board");
                for (int i = 0; i < boardList.Count; ++i)
                {
                    LoadBoard(boardList[i], filePath, i);
                }

                _currentBoardIndex = 0;
            }
        }

        private string GetUniqueBoardName(string boardNameBase)
        {
            string boardNameFinal = boardNameBase;

            int idxAttempt = 1;

            for (int i = 0; i < _boards.Count; ++i)
            {
                if (String.Compare(_boards[i].Name, boardNameFinal, true) == 0)
                {
                    ++idxAttempt;

                    boardNameFinal = boardNameBase + " (" + idxAttempt.ToString() + ")";

                    i = 0;
                }
            }

            return boardNameFinal;
        }

        public void LoadBoard(XmlNode nodeBoard, string filePath, int boardIndex)
        {
            HerbiSpeaksBoard board = new HerbiSpeaksBoard();

            _boards.Add(board);

            string boardName = "Default";

            // First the board-level details.
            XmlNode node = nodeBoard.Attributes.GetNamedItem("Name");
            if (node != null)
            {
                boardName = node.Value;
            }

            // Beware duplicate names when importing boards.
            board.Name = GetUniqueBoardName(boardName);

            board.BackgroundColor = this.BackColor;

            node = nodeBoard.Attributes.GetNamedItem("BackgroundColour");
            if (node != null)
            {
                board.BackgroundColor = Color.FromArgb(Convert.ToInt32(node.Value));

                // Set the default board background color to be the color of the first board.
                if (boardIndex == 0)
                {
                    _colBackground = board.BackgroundColor;

                    this.BackColor = _boards[0].BackgroundColor;
                }
            }

            node = nodeBoard.Attributes.GetNamedItem("TextColour");
            if (node != null)
            {
                _colButtonText = Color.FromArgb(Convert.ToInt32(node.Value));
            }

            ReadDefaultFontDetails(nodeBoard);

            _previousButtonFont = null;

            // Next all the buttons.

            XmlNode nodeButtons = nodeBoard.SelectSingleNode("BoardButtons");

            XmlNodeList nodeButtonList = nodeButtons.SelectNodes("BoardButton");

            for (int i = 0; i < nodeButtonList.Count; ++i)
            {
                XmlNode nodeButton = nodeButtonList[i];
                if (nodeButton != null)
                {
                    HerbiSpeaksButton pictureButton = LoadButtonData(board, boardIndex, nodeButton, null);
                    if (pictureButton.IsPictureButton)
                    {
                        _currentButton = pictureButton;

                        XmlNode containedButtons = nodeButton.SelectSingleNode("PictureButtonContents");

                        XmlNodeList containedButtonsList = containedButtons.SelectNodes("BoardButton");
                        for (int j = 0; j < containedButtonsList.Count; ++j)
                        {
                            XmlNode containedNodeButton = containedButtonsList[j];

                            LoadButtonData(board, boardIndex, containedNodeButton, pictureButton);
                        }
                    }
                }
            }
        }

        private HerbiSpeaksButton LoadButtonData(HerbiSpeaksBoard board, int boardIndex, 
            XmlNode nodeButton,
            HerbiSpeaksButton containingPictureButton)
        {
            bool fIsPictureButton = false;

            XmlNode node = nodeButton.Attributes.GetNamedItem("IsPictureButton");
            if (node != null)
            {
                fIsPictureButton = Convert.ToBoolean(node.Value);
            }

            HerbiSpeaksButton button = AddNewButton(false, fIsPictureButton);

            if (containingPictureButton != null)
            {
                containingPictureButton.Controls.Add(button);
            }
            else
            {
                this.Controls.Add(button);
            }

            button.BoardName = board.Name;
            button.Visible = (boardIndex == 0);

            node = nodeButton.Attributes.GetNamedItem("Left");
            if (node != null)
            {
                button.Left = Convert.ToInt32(node.Value);
            }

            node = nodeButton.Attributes.GetNamedItem("Top");
            if (node != null)
            {
                button.Top = Convert.ToInt32(node.Value);
            }

            node = nodeButton.Attributes.GetNamedItem("Width");
            if (node != null)
            {
                button.Width = Convert.ToInt32(node.Value);
            }
            else
            {
                button.Width = _defaultButtonSize.Width;
            }

            SetButtonCenterFromLocation(button);

            node = nodeButton.Attributes.GetNamedItem("Height");
            if (node != null)
            {
                button.Height = Convert.ToInt32(node.Value);
            }
            else
            {
                button.Height = _defaultButtonSize.Height;
            }

            node = nodeButton.Attributes.GetNamedItem("CenterX");
            if (node != null)
            {
                button.CenterX = Convert.ToDouble(node.Value);
            }

            node = nodeButton.Attributes.GetNamedItem("CenterY");
            if (node != null)
            {
                button.CenterY = Convert.ToDouble(node.Value);
            }

            node = nodeButton.Attributes.GetNamedItem("BoardLink");
            if (node != null)
            {
                button.BoardLink = node.Value;
            }

            node = nodeButton.Attributes.GetNamedItem("BoardLinkSpoken");
            if (node != null)
            {
                button.BoardLinkSpoken = Convert.ToBoolean(node.Value);
            }

            SetButtonLocationFromCenter(button);

            bool fShowText = true;
            string text = _defaultButtonText;

            node = nodeButton.Attributes.GetNamedItem("ShowText");
            if (node != null)
            {
                fShowText = Convert.ToBoolean(node.Value);
            }

            node = nodeButton.Attributes.GetNamedItem("Text");
            if (node != null)
            {
                text = node.Value;
            }

            button.AccessibleName = text;

            if (fShowText)
            {
                button.Text = text;
            }
            else
            {
                button.Text = "";
            }

            node = nodeButton.Attributes.GetNamedItem("TextColour");
            if (node != null)
            {
                button.ForeColor = Color.FromArgb(Convert.ToInt32(node.Value));
            }
            else
            {
                button.ForeColor = _colButtonText;
            }

            // By default, buttons are not transparent.
            button.BackColor = this.BackColor;
            button.FlatStyle = FlatStyle.Flat;

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.BorderColor = this.BackColor;

            node = nodeButton.Attributes.GetNamedItem("ButtonTransparent");
            if (node != null)
            {
                button.ButtonTransparent = Convert.ToBoolean(node.Value);
                if (button.ButtonTransparent)
                {
                    button.BackColor = Color.Transparent;
                    button.FlatStyle = FlatStyle.Popup;
                }
            }

            node = nodeButton.Attributes.GetNamedItem("ButtonTransparentOnHover");
            if (node != null)
            {
                button.ButtonTransparentOnHover = Convert.ToBoolean(node.Value);
            }

            string fontName = "";
            int fontSize = 0;

            node = nodeButton.Attributes.GetNamedItem("FontName");
            if (node != null)
            {
                fontName = node.Value;
            }

            node = nodeButton.Attributes.GetNamedItem("FontSize");
            if (node != null)
            {
                fontSize = Convert.ToInt32(node.Value);
            }

            if (fontName == "")
            {
                fontName = _defaultFont.Name;
            }

            if (fontSize == 0)
            {
                fontSize = (int)_defaultFont.Size;
            }

            // Don't create fonts unnecessarily.
            if ((fontName == _defaultFont.Name) &&
                (fontSize == (int)_defaultFont.Size))
            {
                button.Font = _defaultFont;
            }
            else if ((_previousButtonFont != null) &&
                        (fontName == _previousButtonFont.Name) &&
                        (fontSize == (int)_previousButtonFont.Size))
            {
                button.Font = _previousButtonFont;
            }
            else
            {
                button.Font = new Font(fontName, fontSize);
            }

            _previousButtonFont = button.Font;

            node = nodeButton.Attributes.GetNamedItem("Media");
            if (node != null)
            {
                button.Media = node.Value;
            }

            node = nodeButton.Attributes.GetNamedItem("ButtonTextSpokenBeforeMedia");
            if (node != null)
            {
                button.ButtonTextSpokenBeforeMedia = Convert.ToBoolean(node.Value);
            }
            else
            {
                button.ButtonTextSpokenBeforeMedia = true;
            }

            node = nodeButton.Attributes.GetNamedItem("AutoPlayMedia");
            if (node != null)
            {
                button.AutoPlayMedia = Convert.ToBoolean(node.Value);
            }

            node = nodeButton.Attributes.GetNamedItem("TextPosition");
            if (node != null)
            {
                button.TextPosition = node.Value;
            }
            else
            {
                button.TextPosition = "Middle";
            }

            int pictureDataLength = 0;
            int hoverPictureDataLength = 0;

            try
            {
                XmlNode nodePicture = nodeButton.SelectSingleNode("PictureData");
                if (nodePicture != null)
                {
                    XmlNode nodeLength = nodePicture.Attributes.GetNamedItem("Length");
                    if (nodeLength != null)
                    {
                        pictureDataLength = Convert.ToInt32(nodeLength.Value);
                        Debug.WriteLine("PictureData Length: " + pictureDataLength);
                        if (pictureDataLength > 0)
                        {
                            byte[] bytes = Convert.FromBase64String(nodePicture.InnerXml);

                            using (MemoryStream stream = new MemoryStream(bytes))
                            {
                                var image = Image.FromStream(stream);

                                // Reduce the memory load due to the original (potentially very large)
                                // picture, by resizing it down to the size of the button.
                                button.ImageFull = ResizeImage(image, button.Width, button.Height);

                                image.Dispose();
                            }
                        }
                    }
                }

                XmlNode nodeHoverPicture = nodeButton.SelectSingleNode("HoverPictureData");
                if (nodeHoverPicture != null)
                {
                    XmlNode nodeLength = nodeHoverPicture.Attributes.GetNamedItem("Length");
                    if (nodeLength != null)
                    {
                        hoverPictureDataLength = Convert.ToInt32(nodeLength.Value);
                        Debug.WriteLine("HoverPictureData Length: " + hoverPictureDataLength);
                        if (hoverPictureDataLength > 0)
                        {
                            byte[] bytes = Convert.FromBase64String(nodeHoverPicture.InnerXml);

                            using (MemoryStream stream = new MemoryStream(bytes))
                            {
                                var image = Image.FromStream(stream);

                                button.ImageHoverFull = ResizeImage(image, button.Width, button.Height);

                                image.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading picture on board " + boardIndex + ": " + ex.Message);
            }

            // When loading pictures onto buttons, first the original (potentially very large)
            // picture is loaded from file, and then resized down to the size of the buttons.
            // When the file contains many large pictures, this can result in an OOM exception
            // when loading the pictures. As such force some garbage collection to reduce the 
            // chances of the exception being raised.

            // While explicitly calling GC.Collect() here during development seemed to avoid 
            // an OOM exception, recent testing seemed to suggest it's not required.
            //if ((pictureDataLength > 0) || (hoverPictureDataLength > 0))
            //{
            //    GC.Collect();
            //}

            // Now present the image in the most appropriate manner.
            SetTextPositionOnButton(button);

            return button;
        }

        // Barker: Copied most of this helpful function from StackOverflow.
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }

                graphics.Dispose();
            }

            return destImage;
        }

        private void ReadDefaultFontDetails(XmlNode nodeBoard)
        {
            XmlNode nodeName = nodeBoard.Attributes.GetNamedItem("DefaultFontName");
            XmlNode nodeSize = nodeBoard.Attributes.GetNamedItem("DefaultFontSize");

            if (((nodeName != null) && (nodeName.Value.ToString() != this.Font.Name)) ||
                ((nodeSize != null) && (nodeSize.Value.ToString() != this.Font.Size.ToString())))
            {
                string fontName;

                if ((nodeName != null) && (nodeName.Value.ToString() != this.Font.Name))
                {
                    fontName = nodeName.Value.ToString();
                }
                else
                {
                    fontName = this.Font.Name;
                }

                int fontSize;

                if ((nodeSize != null) && (nodeSize.Value.ToString() != this.Font.Size.ToString()))
                {
                    fontSize = Convert.ToInt32(nodeSize.Value);
                }
                else
                {
                    fontSize = (int)this.Font.Size;
                }

                _defaultFont = new Font(fontName, fontSize);
            }
        }
    }
}
