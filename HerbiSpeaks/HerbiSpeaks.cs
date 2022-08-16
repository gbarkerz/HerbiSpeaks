using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.IO.IsolatedStorage;
using System.Configuration;

namespace HerbiSpeaks
{
    public partial class HerbiSpeaks : Form
    {
        private System.Speech.Synthesis.SpeechSynthesizer synth;
        private bool _outputInProgress = false;

        private bool _editModeActive = false;

        // Board defaults.
        private Color _colBackground = Color.Black;
        private Color _colButtonText = Color.White;
        private Font _defaultFont;
        private int _borderThickness = 4;

        private Font _previousButtonFont;

        public int BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; }
        }

        // Board button defaults.
        private Size _defaultButtonSize = new Size(200, 200);
        private string _defaultButtonText = "ABC";

        // The button to which board-specific commands will be applied.
        private HerbiSpeaksButton _currentButton;

        // File stuff.
        private bool _fIsDirty = false;
        private string _currentFilename = "";

        // For moving and sizing buttons...
        private bool _fDragging = false;
        private int _dragCornerSize = 32;
        private bool _resizingButtonTopLeft = false;
        private bool _resizingButtonTopRight = false;
        private bool _resizingButtonBottomLeft = false;
        private bool _resizingButtonBottomRight = false;
        private Point _originalLocation;
        private Size _originalSize;
        private Point _dropOffset;

        // Most recently used folder for source of pictures and media.
        private string _mostRecentPictureFolder = "";

        // Most recently used folder for opening Herbi Speaks files, (and SaveAs).
        public string _mostRecentFileOpenFolder = "";
        
///* MEDIA
        private string _mostRecentMediaFolder = "";
        private string _mediaFileExtensionList = "*.asf;*.avi;*.mid;*.mov;*.mp3;*.mp4;*.mpeg;*.qt;*.wav;*.wma;*.wmv";
        private string _mediaFileFriendlyList = "*.asf, *.avi, *.mid, *.mov, *.mp3, *.mp4, *.mpeg, *.qt, *.wav, *.wma, *.wmv";
        
        // Maybe WMP also supports:
        // *.aif;*.aifc;*.aiff;*.au;*.dvr-ms;*.m1v;*.m2t;*.m2ts;*.m4v;*.mp2;*.mpe;*.mpg;*.mpv2;*.mod;*.m1v;*.snd;*.vob;*.wm;*.wtv";
        
// MEDIA */

        // For switching out of fullscreen mode.
        private Rectangle _rectBeforeFullscreen;

        public Collection<HerbiSpeaksBoard> _boards = new Collection<HerbiSpeaksBoard>();
        private int _currentBoardIndex;

        private const int c_cGridLines = 17;
        private Pen _penGridLines;
        private Brush _brushDraggingFeedback;
        private Button _btnBeingMoved;
        private Rectangle _rectDragFeedback = new Rectangle();
        private Rectangle _rectDragFeedbackPrevious = new Rectangle();
        private Rectangle _rectDragFeedbackFull = new Rectangle();

        private Point ptCursorWhenContextMenuAppears = new Point(0, 0);

        private string _fileToOpen;

        private int indexSwitchBoardAfterOutput = -1;
        private string strPlayMediaAfterSpeech = "";

        // Thoughts on the use of indexSwitchBoardAfterOutput.
        //
        // indexSwitchBoardAfterOutput indicates whether a switch to a board is to occur,
        // after text speech and/or media has completed. So if a button is clicked and
        // speech and/or media is played, we may then switch to another board. A value
        // of -1 means that no switch is to occur.
        //
        // So indexSwitchBoardAfterOutput is checked in synth_SpeakCompleted() when speech is completed.
        // It is also checked in axWindowsMediaPlayer1_PlayStateChange() when media is canceled or ended.
        //
        // indexSwitchBoardAfterOutput is set to -1 in the following cases:
        //
        // - In CancelSpeech() as when never want to switch boards when speech is interrupted.
        // - When the left mouse button is pressed in Edit Mode.
        // - At the start of InvokeButton() is called to perform a buttons actions.
        // 
        // indexSwitchBoardAfterOutput is set to some board index ealry in InvokeButton(),
        // depending on the properties of the button clicked.
        //
        // In InvokeButton() is no speech or media is played, then we immediately switch to the target board.
        // In that case we also set indexSwitchBoardAfterOutput to -1 after switching to the target board.
        //
        // Note: SwitchBoard() is called to switch boards on completion of speech/media. Inside SwitchBoard()
        // we may call InvokeButton(), if the button has an auto-play button. But InvokeButton() can also
        // call SwitchBoard().
        //
        // The upshot is that InvokeButton() will set indexSwitchBoardAfterOutput to -1, and then may set it 
        // to a valid target. So whoever calls InvokeButton(), (OR SwitchBoard() which calls InvokeButton(),) 
        // must not set indexSwitchBoardAfterOutput to -1 after the call returns.         

        public HerbiSpeaks(string fileToOpen)
        {
            this._fileToOpen = fileToOpen;

            InitializeComponent();
        }
        
        private void UpgradeSettings()
        {
            if (Settings1.Default.FirstTimeRunningThisVersion)
            {
                Settings1.Default.Upgrade();

                Settings1.Default.Reload(); // To activate the settings.

                Settings1.Default.FirstTimeRunningThisVersion = false;

                Settings1.Default.Save(); // To save the new value of FirstTimeRunningThisVersion.
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpgradeSettings();

            if (axWindowsMediaPlayer1 != null)
            {
                axWindowsMediaPlayer1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(axWindowsMediaPlayer1_PlayStateChange);
            }
            else
            {
                MessageBox.Show(
                    this,
                    "The audio and video features of the Herbi Speaks app are not available on this computer.\r\n\r\n" +
                    "To enable these features, please visit the \"Apps & features\" page in the Windows Settings app. " +
                    "Then go to the \"Manage optional features page\" and add the \"Windows Media Player\" feature.\r\n\r\n" +
                    "When the Herbi Speaks app is then restarted, the audio and video features should be available.",
                    "Herbi Speaks",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            synth = new System.Speech.Synthesis.SpeechSynthesizer();
            synth.SpeakCompleted += new EventHandler<System.Speech.Synthesis.SpeakCompletedEventArgs>(synth_SpeakCompleted);

            this.MouseDown += new MouseEventHandler(HerbiSpeaks_MouseDown);
            this.KeyDown += new KeyEventHandler(HerbiSpeaks_KeyDown);

            Rectangle rectWorkingArea = Screen.GetWorkingArea(this);
            this.Location = rectWorkingArea.Location;
            this.Size = rectWorkingArea.Size;
            this.WindowState = FormWindowState.Maximized;

            this.FormClosing += new FormClosingEventHandler(HerbiSpeaks_FormClosing);

            // Create a default board.
            AddNewBoard("Default");
            SetCaptionText();

            LoadSettings();

            // Set the default board background color.
            _boards[0].BackgroundColor = this.BackColor;

            // Create a user folder for saved files if necessary.
            string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string herbiSpeaksFolder = docsFolder + "\\HerbiSpeaks";

            if (!Directory.Exists(herbiSpeaksFolder))
            {
                Directory.CreateDirectory(herbiSpeaksFolder);
            }

            this.Paint += new PaintEventHandler(HerbiSpeaks_Paint);

            _penGridLines = new Pen(new SolidBrush(Color.Black));
            _brushDraggingFeedback = new SolidBrush(Color.LightGray);

            if ((_fileToOpen != null) && (_fileToOpen != ""))
            {
                LoadHerbiFile(_fileToOpen);
            }
        }

        void synth_SpeakCompleted(object sender, System.Speech.Synthesis.SpeakCompletedEventArgs e)
        {
            if (strPlayMediaAfterSpeech != "")
            {
                PlayMedia(strPlayMediaAfterSpeech);
            }
            else
            {
                _outputInProgress = false;

                if (this.indexSwitchBoardAfterOutput != -1)
                {
                    int indexTargetBoard = this.indexSwitchBoardAfterOutput;

                    // Note that indexSwitchBoardAfterOutput may get set up again beneath SwitchBoard().
                    this.indexSwitchBoardAfterOutput = -1;

                    SwitchBoard(indexTargetBoard, true);
                }
            }
        }

        void CancelSpeech()
        {
            indexSwitchBoardAfterOutput = -1;
            strPlayMediaAfterSpeech = "";

            _outputInProgress = false;

            synth.SpeakAsyncCancelAll();
        }

///* MEDIA ****************************************************************
        void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            Debug.WriteLine("WMP state changed, new value " + e.newState.ToString());

            // From https://msdn.microsoft.com/en-us/library/windows/desktop/dd562460(v=vs.85).aspx
            // 1:    // Stopped
            // 8:    // MediaEnded

            if ((e.newState == 8) || (e.newState == 1))
            {
                int targetBoardIndex = this.indexSwitchBoardAfterOutput; 

                PlayMedia("");

                if (targetBoardIndex != -1)
                {
                    this.indexSwitchBoardAfterOutput = -1;

                    SwitchBoard(targetBoardIndex, true);
                }
            }
        }
//MEDIA ****************************************************************/

        void HerbiSpeaks_Paint(object sender, PaintEventArgs e)
        {
            if (_editModeActive)
            {
                if (_fDragging)
                {
                    e.Graphics.FillRectangle(_brushDraggingFeedback, _rectDragFeedback);
                }

                Point pt1 = new Point(0, 0);
                Point pt2 = new Point(0, 0);

                // First draw all the vertical lines.
                pt2.Y = this.ClientSize.Height;

                for (int i = 1; i < c_cGridLines; ++i)
                {
                    pt1.X = (i * this.ClientSize.Width) / c_cGridLines;
                    pt2.X = pt1.X;

                    e.Graphics.DrawLine(_penGridLines, pt1, pt2);
                }

                // Now draw all the horizontal lines.
                pt1.X = 0;
                pt2.X = this.ClientSize.Width;

                int offset = (menuStripApp.Visible ? menuStripApp.Height : 0);

                for (int i = 1; i < c_cGridLines; ++i)
                {
                    pt1.Y = offset + ((i * (this.ClientSize.Height - offset)) / c_cGridLines);
                    pt2.Y = pt1.Y;

                    e.Graphics.DrawLine(_penGridLines, pt1, pt2);
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            // Reposition all the controls at their relative position in the window.
            SetButtonLocations();

            if (_editModeActive)
            {
                // Refresh the grid lines.
                this.Refresh();
            }

            base.OnSizeChanged(e);
        }

        private void SetButtonLocations()
        {
            for (int i = this.Controls.Count - 1; i >= 0; --i)
            {
                HerbiSpeaksButton button = this.Controls[i] as HerbiSpeaksButton;
                if (button != null)
                {
                    SetButtonLocationFromCenter(button);
                }
            }
        }

        private void SetButtonLocationFromCenter(HerbiSpeaksButton button)
        {
            int width = this.Width;
            int height = this.Height;

            if (IsButtonContainedInPicture(button))
            {
                HerbiSpeaksButton parent = button.Parent as HerbiSpeaksButton;
                width = parent.Width;
                height = parent.Height;
            }

            button.Left = Math.Max(0, (int)((button.CenterX * (double)width) - (double)(button.Width / 2)));
            button.Top = Math.Max(0, (int)((button.CenterY * (double)height) - (double)(button.Height / 2)));

            if (button.Left + button.Width > this.Width)
            {
                button.Left = this.Width - button.Width;
            }

            if (button.Top + button.Height > this.Height)
            {
                button.Top = this.Height - button.Height;
            }
        }

        private bool IsButtonContainedInPicture(HerbiSpeaksButton button)
        {
            bool fButtonIsContainedInPicture = false;

            HerbiSpeaksButton parent = button.Parent as HerbiSpeaksButton;
            if (parent != null)
            {
                fButtonIsContainedInPicture = true;
            }

            return fButtonIsContainedInPicture;
        }

        private void SetButtonCenterFromLocation(HerbiSpeaksButton button)
        {
            // The Center point is relative to either the form, or the picture button that
            // contains the button.

            int width = this.Width;
            int height = this.Height;

            if (IsButtonContainedInPicture(button))
            {
                HerbiSpeaksButton parent = button.Parent as HerbiSpeaksButton;
                width = parent.Width;
                height = parent.Height;
            }

            button.CenterX = (double)(button.Location.X + (button.Size.Width / 2)) / (double)width;
            button.CenterY = (double)(button.Location.Y + (button.Size.Height / 2)) / (double)height;

            // If this is a picture button, set the position of the buttons it contains.
            if (button.Controls.Count > 0)
            {
                Control[] controls = new Control[button.Controls.Count];
                button.Controls.CopyTo(controls, 0);

                for (int i = 0; i < controls.Length; i++)
                {
                    HerbiSpeaksButton containedButton = controls[i] as HerbiSpeaksButton;

                    SetButtonLocationFromCenter(containedButton);
                }
            }
        }

        void HerbiSpeaks_FormClosing(object sender, FormClosingEventArgs e)
        {
            PlayMedia("");

            bool fDoClose = true;

            if (_fIsDirty)
            {
                bool fCancel = HandleDirtyFile();
                if (fCancel)
                {
                    fDoClose = false;
                }
            }

            if (fDoClose)
            {
                // Always save the default settings.
                SaveSettings();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void SaveSettings()
        {
            Settings1.Default.Save();
        }

        private void LoadSettings()
        {
            _colBackground = Settings1.Default.BackgroundColor;
            this.BackColor = _colBackground;

            _colButtonText = Settings1.Default.DefaultTextColor;
            _borderThickness = Settings1.Default.DefaultBorderThickness;
            
            if ((Settings1.Default.DefaultFontName != "") && (Settings1.Default.DefaultFontSize > 0))
            {
                _defaultFont = new Font(Settings1.Default.DefaultFontName, Settings1.Default.DefaultFontSize);
                this.Font = _defaultFont;
            }

            _mostRecentPictureFolder = Settings1.Default.MostRecentPictureFolder;
            _mostRecentFileOpenFolder = Settings1.Default.MostRecentFileOpenFolder;
        }

        private void SetEditMode(bool fActivateEditMode)
        {
            PlayMedia("");

            if (fActivateEditMode && !_editModeActive)
            {
                _editModeActive = true;
            }
            else if (!fActivateEditMode && _editModeActive)
            {
                _editModeActive = false;
            }

            Color newColor = (_editModeActive ? Color.Gray : _colBackground);
            if (this.BackColor != newColor)
            {
                this.BackColor = newColor;
            }
        }

        private void ToggleEditMode_Click(object sender, System.EventArgs e)
        {
            PlayMedia("");

            _editModeActive = !_editModeActive;

            Color newColor = (_editModeActive ? Color.Gray : _colBackground);
            
            this.BackColor = newColor;

            for (int i = this.Controls.Count - 1; i >= 0; --i)
            {
                HerbiSpeaksButton button = this.Controls[i] as HerbiSpeaksButton;
                if (button != null)
                {
                    if (button.BoardName == _boards[_currentBoardIndex].Name)
                    {
                        button.BackColor = newColor;
                    }
                }
            }
        }

        private void BringButtonToFront_Click(object sender, System.EventArgs e)
        {
            PlayMedia("");

            _fIsDirty = true;

            if (IsButtonContainedInPicture(_currentButton))
            {
                _currentButton.Parent.Controls.SetChildIndex(_currentButton, 0);
            }
            else
            {
                this.Controls.SetChildIndex(_currentButton, 0);
            }
        }

        private void AddButton_Click(object sender, System.EventArgs e)
        {
            AddButtonOrPicture(false, null);
        }

        private void AddPicture_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "Image Files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp|All Files (*.*)|*.*";

            if (_mostRecentPictureFolder != "")
            {
                fileDlg.InitialDirectory = _mostRecentPictureFolder;
            }
            else
            {
                fileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }

            if (fileDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _fIsDirty = true;

                AddButtonOrPicture(true, fileDlg.FileName);

                _mostRecentPictureFolder = Path.GetDirectoryName(fileDlg.FileName);
                Settings1.Default.MostRecentPictureFolder = _mostRecentPictureFolder;
            }
        }

        private void AddButtonOrPicture(bool fPictureButton, string pictureFileName)
        {
            _fIsDirty = true;

            HerbiSpeaksButton button = AddNewButton(true, fPictureButton);

            button.Size = _defaultButtonSize;
            button.Text = _defaultButtonText;
            button.AccessibleName = _defaultButtonText;
            button.ForeColor = _colButtonText;

            bool fAddingButtonInsidePicture = (!fPictureButton &&
                (_currentButton != null) && (_currentButton.IsPictureButton));

            button.ButtonTransparent = fAddingButtonInsidePicture;
            button.ButtonTransparentOnHover = fAddingButtonInsidePicture;

            if (!fAddingButtonInsidePicture)
            {
                button.Location = new Point(
                    ptCursorWhenContextMenuAppears.X - this.Left - (button.Width / 2),
                    ptCursorWhenContextMenuAppears.Y - this.Top - (button.Height / 2));
            }
            else
            {
                Point pt = new Point(
                    ptCursorWhenContextMenuAppears.X - this.Left - (button.Width / 2) - _currentButton.Left,
                    ptCursorWhenContextMenuAppears.Y - this.Top - (button.Height / 2) - _currentButton.Top);

                if (pt.X > _currentButton.Width - button.Width)
                {
                    pt.X = _currentButton.Width - button.Width;
                }

                if (pt.X < 0)
                {
                    pt.X = 0;
                }

                if (pt.Y > _currentButton.Height - button.Height)
                {
                    pt.Y = _currentButton.Height - button.Height;
                }

                if (pt.Y < 0)
                {
                    pt.Y = 0;
                }

                button.Location = pt;
            }

            SetButtonCenterFromLocation(button);

            if (_defaultFont != null)
            {
                button.Font = _defaultFont;
            }

            button.BoardName = _boards[_currentBoardIndex].Name;

            button.ButtonTextSpokenBeforeMedia = true;
            button.AutoPlayMedia = false;

            if (button.IsPictureButton)
            {
                button.ImageExtension = Path.GetExtension(pictureFileName);
                button.ImageFull = Image.FromFile(pictureFileName);

                button.Text = "";
                button.AccessibleName = "";

                button.TextAlign = ContentAlignment.MiddleCenter;
                button.ImageAlign = ContentAlignment.MiddleCenter;

                int maxDimension = 400;

                if ((button.ImageFull.Width <= maxDimension) &&
                    (button.ImageFull.Height <= maxDimension))
                {
                    button.Width = button.ImageFull.Width;
                    button.Height = button.ImageFull.Height;
                }
                else
                {
                    if (button.ImageFull.Width > button.ImageFull.Height)
                    {
                        button.Width = maxDimension;
                        button.Height = ((button.Width * button.ImageFull.Height) / button.ImageFull.Width);
                    }
                    else
                    {
                        button.Height = maxDimension;
                        button.Width = ((button.Height * button.ImageFull.Width) / button.ImageFull.Height);
                    }
                }

                ReloadThumbnail(button);
            }
            else // This is not a picture button.
            { 
                // Give the user a chance to set some useful text on the button now.

                _currentButton = button;
                
                SetButtonText();
            }
        }

        private HerbiSpeaksButton AddNewButton(bool fAddControl, bool fIsPictureButton)
        {
            HerbiSpeaksButton button = new HerbiSpeaksButton();

            bool fAddingButtonInsidePicture = fAddControl && (!fIsPictureButton &&
                (_currentButton != null) && (_currentButton.IsPictureButton));

            if (fAddControl)
            {
                if (fAddingButtonInsidePicture)
                {
                    _currentButton.Controls.Add(button);
                }
                else
                {
                    this.Controls.Add(button);
                }
            }

            button.IsPictureButton = fIsPictureButton;

            button.TextPosition = "Middle";

            button.MouseDown += new MouseEventHandler(button_MouseDown);
            button.MouseUp += new MouseEventHandler(button_MouseUp);

            button.MouseMove += new MouseEventHandler(button_MouseMove);
            button.MouseLeave += new EventHandler(button_MouseLeave);
            button.MouseEnter += new EventHandler(button_MouseEnter);

            button.KeyDown += new KeyEventHandler(button_KeyDown);

            button.FlatStyle = (fAddingButtonInsidePicture ? FlatStyle.Popup : FlatStyle.Flat);
            button.FlatAppearance.BorderSize = 0;

            if (_currentButton != null)
            {
                button.BackColor = Color.Transparent;
            }

            return button;
        }


        private void RemoveButton_Click(object sender, System.EventArgs e)
        {
            _fIsDirty = true;

            HerbiSpeaksButton parentButton = _currentButton.Parent as HerbiSpeaksButton;
            if (parentButton == null)
            {
                this.Controls.Remove(_currentButton);
            }
            else
            {
                // Remove all this picture button's contained buttons first.
                for (int i = _currentButton.Controls.Count - 1; i >= 0 ; i--)
                {
                    _currentButton.Controls.RemoveAt(i); 
                }

                parentButton.Controls.Remove(_currentButton);
            }

            _currentButton = null;
        }

        private void SetButtonTextButton_Click(object sender, System.EventArgs e)
        {
            SetButtonText();
        }

        private void SetButtonText()
        {
            PlayMedia("");

            SetButtonTextForm setButtonTextForm = new SetButtonTextForm(_currentButton);
            if (setButtonTextForm.ShowDialog(this) != System.Windows.Forms.DialogResult.Cancel)
            {
                _fIsDirty = true;
            }
        }

        private void SetButtonTextColorButton_Click(object sender, System.EventArgs e)
        {
            PlayMedia("");

            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _fIsDirty = true;

                _currentButton.ForeColor = dlg.Color;
            }
        }

        private void SetButtonTextFontButton_Click(object sender, System.EventArgs e)
        {
            PlayMedia("");

            FontDialog dlg = new FontDialog();
            dlg.Font = new Font(_currentButton.Font.Name, _currentButton.Font.Size);
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _fIsDirty = true;

                _currentButton.Font = dlg.Font;
            }
        }

        private void SetTransparentButtonOnHover_Click(object sender, System.EventArgs e)
        {
            _fIsDirty = true;

            PlayMedia("");

            _currentButton.ButtonTransparentOnHover = !_currentButton.ButtonTransparentOnHover;
        }

        private void SetTransparentButton_Click(object sender, System.EventArgs e)
        {
            _fIsDirty = true;

            PlayMedia("");

            _currentButton.ButtonTransparent = !_currentButton.ButtonTransparent;

            _currentButton.BackColor = (_currentButton.ButtonTransparent ? Color.Transparent : _colBackground);
        }

        private void SetButtonBoardLinkButton_Click(object sender, System.EventArgs e)
        {
            _fIsDirty = true;

            PlayMedia("");

            MenuItem item = (MenuItem)sender;

            if ((_currentButton.BoardLink == null) || (_currentButton.BoardLink == ""))
            {
                // When a new link is being addded, by default have the button text spoken
                // before switch boards.
                _currentButton.BoardLinkSpoken = true;
            }

            if (_currentButton.BoardLink == item.Text)
            {
                _currentButton.BoardLink = null;
            }
            else
            {
                _currentButton.BoardLink = item.Text;
            }
        }

        private void SetButtonBoardLinkRandomButton_Click(object sender, System.EventArgs e)
        {
            _fIsDirty = true;

            PlayMedia("");

            if ((_currentButton.BoardLink == null) || (_currentButton.BoardLink == ""))
            {
                // When a new link is being addded, by default have the button text spoken
                // before switch boards.
                _currentButton.BoardLinkSpoken = true;
            }

            if (_currentButton.BoardLink == "<random>")
            {
                _currentButton.BoardLink = null;
            }
            else
            {
                _currentButton.BoardLink = "<random>";
            }
        }

        private void SetButtonBoardLinkSpokenButton_Click(object sender, System.EventArgs e)
        {
            PlayMedia("");

            _currentButton.BoardLinkSpoken = !_currentButton.BoardLinkSpoken;
        }

        private void SetButtonPictureButton_Click(object sender, System.EventArgs e)
        {
            SetButtonPicture(false);
        }

        private void SetButtonHoverPictureButton_Click(object sender, System.EventArgs e)
        {
            SetButtonPicture(true);
        }

        private void SetButtonPicture(bool fHoverPicture)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "Image Files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp|All Files (*.*)|*.*";

            if (_mostRecentPictureFolder != "")
            {
                fileDlg.InitialDirectory = _mostRecentPictureFolder;
            }
            else
            {
                fileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }

            if (fileDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if (fHoverPicture)
                    {
                        _currentButton.ImageHoverExtension = Path.GetExtension(fileDlg.FileName);
                        _currentButton.ImageHoverFull = Image.FromFile(fileDlg.FileName);
                    }
                    else
                    {
                        _currentButton.ImageExtension = Path.GetExtension(fileDlg.FileName);
                        _currentButton.ImageFull = Image.FromFile(fileDlg.FileName);
                    }

                    _fIsDirty = true;

                    SetTextPositionOnButton(_currentButton);

                    _mostRecentPictureFolder = Path.GetDirectoryName(fileDlg.FileName);
                    Settings1.Default.MostRecentPictureFolder = _mostRecentPictureFolder;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,
                        "Sorry, the media file \"" + fileDlg.FileName + 
                        "\" couldn't be set on the button.",
                        "Herbi Speaks");

                    Debug.WriteLine("Error loading picture onto button: " + ex.Message);
                }
            }
        }

        public bool ThumbnailCallback()
        {
            return false;
        }

        private void SetTextPositionButton_Click(object sender, System.EventArgs e)
        {
            _fIsDirty = true;

            MenuItem item = (MenuItem)sender;
            _currentButton.TextPosition = item.Text;

            SetTextPositionOnButton(_currentButton);
        }

        private void SetTextPositionOnButton(HerbiSpeaksButton button)
        {
            if (button.TextPosition == "Top")
            {
                button.TextAlign = ContentAlignment.TopCenter;
                button.ImageAlign = ContentAlignment.BottomCenter;
            }
            else if (button.TextPosition == "Bottom")
            {
                button.TextAlign = ContentAlignment.BottomCenter;
                button.ImageAlign = ContentAlignment.TopCenter;
            }
            else 
            {
                button.TextAlign = ContentAlignment.MiddleCenter;
                button.ImageAlign = ContentAlignment.MiddleCenter;
            }

            // Reload the thumbnail.
            ReloadThumbnail(button);
        }

        private bool ReloadThumbnail(HerbiSpeaksButton btn)
        {
            bool fSuccess = false;

            Image.GetThumbnailImageAbort myCallback =
                new Image.GetThumbnailImageAbort(ThumbnailCallback);

            int textAreaSize = 0;

            if (btn.TextPosition == "Middle")
            {
                if (btn.ImageFull != null)
                {
                    btn.Image = null;
                    btn.BackgroundImage = btn.ImageFull;
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                }

                fSuccess = true;
            }
            else
            {
                btn.BackgroundImage = null;

                Size textSize = TextRenderer.MeasureText(btn.Text, btn.Font);

                textAreaSize = textSize.Height + 20;

                // Assume problems using the file are due to invalid file types.
                try
                {
                    if (btn.ImageFull != null)
                    {
                        btn.ImageFullThumbnail = btn.ImageFull.GetThumbnailImage(
                                                    btn.Width,
                                                    btn.Height - textAreaSize,
                                                    myCallback, IntPtr.Zero);

                        btn.Image = btn.ImageFullThumbnail;
                    }

                    if (btn.ImageHoverFull != null)
                    {
                        btn.ImageHoverFullThumbnail = btn.ImageHoverFull.GetThumbnailImage(
                                                        btn.Width,
                                                        btn.Height - textAreaSize,
                                                        myCallback, IntPtr.Zero);
                    }

                    fSuccess = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("HerbiSpeaks GetThumbnailImage - " + ex.Message);
                }
            }

            return fSuccess;
        }

        private void RemoveButtonPictureButton_Click(object sender, System.EventArgs e)
        {
            RemoveButtonPicture(false);
        }

        private void RemoveButtonHoverPictureButton_Click(object sender, System.EventArgs e)
        {
            RemoveButtonPicture(true);
        }

        private void RemoveButtonPicture(bool fHoverPicture)
        {
            _fIsDirty = true;

            if (fHoverPicture)
            {
                if (_currentButton.ImageHoverFull != null)
                {
                    _currentButton.ImageHoverFull = null;
                    _currentButton.ImageHoverExtension = "";
                }
            }
            else
            {
                if (_currentButton.ImageFull != null)
                {
                    _currentButton.ImageFull = null;
                    _currentButton.BackgroundImage = null;
                    _currentButton.Image = null;
                    _currentButton.ImageExtension = "";
                }

                // Always show the text now.
                _currentButton.Text = _currentButton.AccessibleName;
            }
        }

        ///* MEDIA ***************************************************************************
        private void SetButtonMediaButton_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "Media files (" + _mediaFileFriendlyList + ")|" + _mediaFileExtensionList + "|All Files (*.*)|*.*";

            //if ((_currentButton.Media != null) && (_currentButton.Media != ""))
            //{
            //    fileDlg.FileName = _currentButton.Media;
            //}

            if (_mostRecentMediaFolder != "")
            {
                fileDlg.InitialDirectory = _mostRecentMediaFolder;
            }
            else
            {
                fileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }

            if (fileDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _fIsDirty = true;

                _currentButton.Media = fileDlg.FileName;

                _mostRecentMediaFolder = Path.GetDirectoryName(fileDlg.FileName);
                Settings1.Default.MostRecentMediaFolder = _mostRecentPictureFolder;
            }
        }

        private void RemoveButtonMediaButton_Click(object sender, System.EventArgs e)
        {
            PlayMedia("");

            if (_currentButton.Media != null)
            {
                _fIsDirty = true;

                _currentButton.Media = null;
            }
        }

        private void ButtonTextSpokenBeforeMediaButton_Click(object sender, System.EventArgs e)
        {
            _fIsDirty = true;

            PlayMedia("");

            _currentButton.ButtonTextSpokenBeforeMedia = !_currentButton.ButtonTextSpokenBeforeMedia;
        }

        private void AutoPlayMediaButton_Click(object sender, System.EventArgs e)
        {
            _fIsDirty = true;

            PlayMedia("");

            bool fNewAutoPlayState = !_currentButton.AutoPlayMedia;

            // Only one button on this board can have media started automatically.
            if (fNewAutoPlayState)
            {
                for (int i = this.Controls.Count - 1; i >= 0; --i)
                {
                    HerbiSpeaksButton button = this.Controls[i] as HerbiSpeaksButton;
                    if (button != null)
                    {
                        if (button.BoardName == _boards[_currentBoardIndex].Name)
                        {
                            button.AutoPlayMedia = false;
                        }
                    }
                }
            }

            _currentButton.AutoPlayMedia = fNewAutoPlayState;
        }

        //MEDIA ***************************************************************************/
 
        private void SetButtonTextOverPictureButton_Click(object sender, System.EventArgs e)
        {
            _fIsDirty = true;

            if (_currentButton.Text == "")
            {
                _currentButton.Text = _currentButton.AccessibleName;
            }
            else
            {
                _currentButton.Text = "";
            }
        }

        private void BuildContextMenu(bool buttonMenu)
        {
            MenuItem submenuItem;

            ContextMenu contextMenu = new ContextMenu();
            this.ContextMenu = contextMenu;

            MenuItem item = new MenuItem("Add new button" +
                ((buttonMenu  && _currentButton.IsPictureButton) ? " to picture" : ""),
                AddButton_Click);
            item.Enabled = !buttonMenu || _currentButton.IsPictureButton;
            contextMenu.MenuItems.Add(item);

            bool fButtonBelongsToPicture = false;

            if (_currentButton != null)
            {
                if ((_currentButton.Parent as HerbiSpeaksButton) != null)
                {
                    fButtonBelongsToPicture = true;
                }
            }

            item = new MenuItem("Remove button" +
                (fButtonBelongsToPicture ? " from picture" : ""),
                RemoveButton_Click);
            item.Enabled = buttonMenu && !_currentButton.IsPictureButton;
            contextMenu.MenuItems.Add(item);

            item = new MenuItem("-");
            contextMenu.MenuItems.Add(item);

            item = new MenuItem(_editModeActive ? "Exit Edit Mode" : "Enter Edit Mode to move or resize buttons", ToggleEditMode_Click);
            item.Enabled = true;
            contextMenu.MenuItems.Add(item);

            item = new MenuItem("Bring button to front", BringButtonToFront_Click);
            item.Enabled = buttonMenu;
            contextMenu.MenuItems.Add(item);

            item = new MenuItem("-");
            contextMenu.MenuItems.Add(item);

            bool fRegularButtonMenu = (buttonMenu && !_currentButton.IsPictureButton);

            // Text properties.

            item = new MenuItem("Text");
            item.Enabled = fRegularButtonMenu;
            contextMenu.MenuItems.Add(item);

            submenuItem = new MenuItem("Set button text", SetButtonTextButton_Click);
            submenuItem.Enabled = fRegularButtonMenu;
            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Set text position");
            submenuItem.Enabled = fRegularButtonMenu;
            item.MenuItems.Add(submenuItem);

            MenuItem submenuItem2 = new MenuItem("Top", SetTextPositionButton_Click);
            submenuItem.MenuItems.Add(submenuItem2);
            submenuItem2.Checked = (fRegularButtonMenu && (_currentButton.TextPosition == "Top"));

            submenuItem2 = new MenuItem("Middle", SetTextPositionButton_Click);
            submenuItem.MenuItems.Add(submenuItem2);
            submenuItem2.Checked = (fRegularButtonMenu && (_currentButton.TextPosition == "Middle"));

            submenuItem2 = new MenuItem("Bottom", SetTextPositionButton_Click);
            submenuItem.MenuItems.Add(submenuItem2);
            submenuItem2.Checked = (fRegularButtonMenu && (_currentButton.TextPosition == "Bottom"));

            submenuItem.MenuItems.Add(submenuItem2);

            submenuItem = new MenuItem("Show text", SetButtonTextOverPictureButton_Click);
            submenuItem.Enabled = (fRegularButtonMenu && (_currentButton.ImageFull != null)) || 
                           (buttonMenu && fButtonBelongsToPicture);

            submenuItem.Checked = fRegularButtonMenu && (_currentButton.Text != "");
            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Set button text font", SetButtonTextFontButton_Click);
            submenuItem.Enabled = fRegularButtonMenu;
            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Set button text/border color", SetButtonTextColorButton_Click);
            submenuItem.Enabled = fRegularButtonMenu;
            item.MenuItems.Add(submenuItem);

            item = new MenuItem("-");
            contextMenu.MenuItems.Add(item);

            // Pictures.

            item = new MenuItem("Pictures");
            item.Enabled = fRegularButtonMenu;
            contextMenu.MenuItems.Add(item);

            submenuItem = new MenuItem("Set picture on button", SetButtonPictureButton_Click);
            submenuItem.Enabled = fRegularButtonMenu;
            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Remove picture from button", RemoveButtonPictureButton_Click);
            submenuItem.Enabled = (fRegularButtonMenu && (_currentButton.ImageFull != null));
            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Set hover picture on button", SetButtonHoverPictureButton_Click);
            submenuItem.Enabled = fRegularButtonMenu;
            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Remove hover picture from button", RemoveButtonHoverPictureButton_Click);
            submenuItem.Enabled = (fRegularButtonMenu && (_currentButton.ImageHoverFull != null));
            item.MenuItems.Add(submenuItem);

            item = new MenuItem("-");
            contextMenu.MenuItems.Add(item);

            item = new MenuItem("Pictures containing buttons");
            item.Enabled = true;
            contextMenu.MenuItems.Add(item);

            submenuItem = new MenuItem("Add new picture", AddPicture_Click);
            submenuItem.Enabled = !buttonMenu;
            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Remove picture and all its buttons", RemoveButton_Click);
            submenuItem.Enabled = buttonMenu && _currentButton.IsPictureButton;
            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Picture button is transparent", SetTransparentButton_Click);
            submenuItem.Enabled = buttonMenu && fButtonBelongsToPicture;
            submenuItem.Checked = buttonMenu && fButtonBelongsToPicture && _currentButton.ButtonTransparent;
            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Picture button is transparent on mouse hover", SetTransparentButtonOnHover_Click);
            submenuItem.Enabled = buttonMenu && fButtonBelongsToPicture && _currentButton.ButtonTransparent;
            submenuItem.Checked = buttonMenu && fButtonBelongsToPicture && _currentButton.ButtonTransparentOnHover;
            item.MenuItems.Add(submenuItem);

            // Media.
///* MEDIA ***************************************************************************

            item = new MenuItem("-");
            contextMenu.MenuItems.Add(item);

            item = new MenuItem("Media (audio or video)");
            item.Enabled = fRegularButtonMenu && (this.axWindowsMediaPlayer1 != null);
            contextMenu.MenuItems.Add(item);

            submenuItem = new MenuItem("Set media on button", SetButtonMediaButton_Click);
            submenuItem.Enabled = fRegularButtonMenu;
            item.MenuItems.Add(submenuItem);

            bool fMediaSet = (fRegularButtonMenu && (_currentButton.Media != null));

            submenuItem = new MenuItem();

            string itemName = "Remove media from button";

            submenuItem.Enabled = fMediaSet;
            if (submenuItem.Enabled)
            {
                string fileName = Path.GetFileNameWithoutExtension(_currentButton.Media);
                if (fileName.Length > 16)
                {
                    fileName = fileName.Substring(0, 15) + "...";
                }

                itemName += " (" + fileName + ")";

                submenuItem.Click += RemoveButtonMediaButton_Click;
            }

            submenuItem.Text = itemName;

            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Speak button text before playing media",
                ButtonTextSpokenBeforeMediaButton_Click);
            submenuItem.Enabled = fMediaSet;
            submenuItem.Checked = (fRegularButtonMenu && _currentButton.ButtonTextSpokenBeforeMedia);
            item.MenuItems.Add(submenuItem);

            submenuItem = new MenuItem("Play media when moving to this board",
                AutoPlayMediaButton_Click);
            submenuItem.Enabled = fMediaSet;
            submenuItem.Checked = (fRegularButtonMenu && _currentButton.AutoPlayMedia);
            item.MenuItems.Add(submenuItem);
             
//MEDIA ***************************************************************************/
 
            item = new MenuItem("-");
            contextMenu.MenuItems.Add(item);

            // Link to another board.
            if (_boards.Count > 1)
            {
                item = new MenuItem("Link to another board");
                item.Enabled = fRegularButtonMenu;
                contextMenu.MenuItems.Add(item);

                bool foundBoardLink = false;

                for (int i = 0; i < _boards.Count; ++i)
                {
                    submenuItem = new MenuItem(_boards[i].Name, SetButtonBoardLinkButton_Click);
                    submenuItem.Enabled = (fRegularButtonMenu && (i != _currentBoardIndex));

                    if ((_currentButton != null) && (_currentButton.BoardLink == _boards[i].Name))
                    {
                        foundBoardLink = true;

                        submenuItem.Checked = true;
                    }

                    item.MenuItems.Add(submenuItem);
                }

                submenuItem = new MenuItem("-");
                item.MenuItems.Add(submenuItem);

                submenuItem = new MenuItem("Link to random board", SetButtonBoardLinkRandomButton_Click);
                submenuItem.Enabled = fRegularButtonMenu;

                if ((_currentButton != null) && (_currentButton.BoardLink == "<random>"))
                {
                    foundBoardLink = true;

                    submenuItem.Checked = true;
                }

                item.MenuItems.Add(submenuItem);

                submenuItem = new MenuItem("-");
                item.MenuItems.Add(submenuItem);

                submenuItem = new MenuItem("Speak button text when moving to other board", SetButtonBoardLinkSpokenButton_Click);
                submenuItem.Enabled = fRegularButtonMenu;

                if (!foundBoardLink || ((_currentButton != null) &&(_currentButton.BoardLinkSpoken)))
                {
                    submenuItem.Checked = true;
                }

                item.MenuItems.Add(submenuItem);

            }
            else
            {
                item = new MenuItem("Link to board");
                item.Enabled = false;
                contextMenu.MenuItems.Add(item);
            }

            item = new MenuItem("-");
            contextMenu.MenuItems.Add(item);

            // View.
            item = new MenuItem(menuStripApp.Visible ? "Go to Fullscreen view (F5)" : "Leave Fullscreen view (Escape)", 
                                FullscreenButton_Click);
            item.Enabled = true;
            contextMenu.MenuItems.Add(item);

            // If a button is added in response to this, add it where the user click to invoke the context menu.
            ptCursorWhenContextMenuAppears = Cursor.Position;

            // The call to Show() is relative to the app window, so account for the window not being at (0, 0);
            Point pt = new Point(Cursor.Position.X - this.Left, Cursor.Position.Y - 20 - this.Top);
            this.ContextMenu.Show(this, pt);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Note that default changes are always saved and don't mark the file as dirty.
        private void setbackgroundColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _fIsDirty = true;

                _colBackground = dlg.Color;

                this.BackColor = (_editModeActive ? Color.Gray : _colBackground);

                Settings1.Default.BackgroundColor = _colBackground;

                for (int i = 0; i < _boards.Count; ++i)
                {
                    if (i == _currentBoardIndex)
                    {
                        _boards[i].BackgroundColor = _colBackground;
                    }
                }
            }
        }

        private void setDefaultfontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            FontDialog dlg = new FontDialog();

            if (_defaultFont != null)
            {
                dlg.Font = new Font(_defaultFont.Name, _defaultFont.Size);
            }

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _defaultFont = dlg.Font;

                Settings1.Default.DefaultFontName = _defaultFont.Name;
                Settings1.Default.DefaultFontSize = _defaultFont.Size;
            }
        }

        private void defaultButtontextColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _colButtonText = dlg.Color;

                Settings1.Default.DefaultTextColor = _colButtonText;
            }
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFullscreenState(true);
        }

        private void FullscreenButton_Click(object sender, System.EventArgs e)
        {
            SetFullscreenState(menuStripApp.Visible);
        }

        private void SetFullscreenState(bool fFullscreen)
        {
            PlayMedia("");

            if (fFullscreen)
            {
                this.WindowState = FormWindowState.Normal;

                this.TopMost = true;

                _rectBeforeFullscreen = new Rectangle(this.Location, this.Size);

                menuStripApp.Visible = false;

                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                Rectangle rect = Screen.GetBounds(this);
                this.Location = rect.Location;
                this.Size = rect.Size;
            }
            else if (!menuStripApp.Visible) // Check whether we're already in fullscreen mode.
            {
                this.TopMost = false;

                menuStripApp.Visible = true;

                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

                this.Location = _rectBeforeFullscreen.Location;
                this.Size = _rectBeforeFullscreen.Size;
            }
        }

        private void defaultButtonBorderThicknessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            ButtonBorder dlg = new ButtonBorder(this);
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Settings1.Default.DefaultBorderThickness = _borderThickness;
            }
        }

        private void AddNewBoard(string boardName)
        {
            PlayMedia("");

            HerbiSpeaksBoard board = new HerbiSpeaksBoard();
            board.Name = boardName;
            board.BackgroundColor = _colBackground;

            _boards.Add(board);
            _currentBoardIndex = _boards.Count - 1;
        }

        private void boardsToolStripMenuItem_Opening(object sender, EventArgs e)
        {
            PlayMedia("");

            ToolStripMenuItem stripMenuItem = (ToolStripMenuItem)sender;

            // Todo: One day remove the use of hard-coded indices here.

            // Only enable Remove Board item if we have more than one board.
            stripMenuItem.DropDownItems[3].Enabled = (_boards.Count > 1);
            stripMenuItem.DropDownItems[4].Enabled = (_boards.Count > 1);

            int minBoardIndex = 7;

            for (int i = stripMenuItem.DropDownItems.Count - 1; i > minBoardIndex; --i)
            {
                ToolStripItem item = stripMenuItem.DropDownItems[i];
                stripMenuItem.DropDownItems.Remove(item);
            }

            for (int i = 0; i < _boards.Count; ++i)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(_boards[i].Name);
                item.Click += new EventHandler(item_Click);

                if (i == _currentBoardIndex)
                {
                    item.Checked = true;
                }

                stripMenuItem.DropDownItems.Add(item);
            }
        }

        void item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            for (int i = 0; i < _boards.Count; ++i)
            {
                if (_boards[i].Name == item.Text)
                {
                    SwitchBoard(i, false);

                    break;
                }
            }
        }

        private void SwitchBoard(int index, bool fAllowAutoPlay)
        {
            // SetEditMode() will stop any in-progress media.
            SetEditMode(false);

            _currentBoardIndex = index;

            HerbiSpeaksButton buttonAutoPlay = null;

            // NOTE: Working backwards through the collection can lead to a reordering of 
            // the items as the visibility is set. This could be worked around by calling
            //     this.Controls.SetChildIndex(button, i);
            // after setting the visibility. But instead, just work forwards through the
            // list as the reordering doesn't seem to happen then.

            for (int i = 0; i < this.Controls.Count; ++i)
            {
                HerbiSpeaksButton button = this.Controls[i] as HerbiSpeaksButton;
                if (button != null)
                {
                    button.Visible = (button.BoardName == _boards[_currentBoardIndex].Name);

                    if (fAllowAutoPlay && button.Visible && button.AutoPlayMedia)
                    {
                        buttonAutoPlay = button;
                    }

                    if (button.Visible && button.IsPictureButton)
                    {
                        if (button.Controls.Count > 0)
                        {
                            Control[] controls = new Control[button.Controls.Count];
                            button.Controls.CopyTo(controls, 0);

                            for (int j = 0; j < controls.Length; ++j)
                            {
                                HerbiSpeaksButton containedButton = controls[j] as HerbiSpeaksButton;
                                containedButton.Visible = true;

                                if (fAllowAutoPlay && containedButton.AutoPlayMedia)
                                {
                                    buttonAutoPlay = containedButton;
                                }
                            }
                        }
                    }
                }
            }

            if (_boards[_currentBoardIndex].BackgroundColor.A == 0xFF)
            {
                this.BackColor = _boards[_currentBoardIndex].BackgroundColor;
            }

            SetCaptionText();

            if (buttonAutoPlay != null)
            {
                _currentButton = buttonAutoPlay;

                InvokeButton(buttonAutoPlay);
            }
        }

        private bool IsBoardNameUnique(string boardName)
        {
            bool boardNameIsUnique = true;

            // Don't allow two boards to have the same name.
            for (int i = 0; i < _boards.Count; ++i)
            {
                if (String.Compare(_boards[i].Name, boardName, true) == 0)
                {
                    boardNameIsUnique = false;

                    string msg = 
                        "A board cannot be added if it has the same name as an existing board.\r\n\r\n" + 
                        "A board called \"" + boardName + "\" already exists.\r\n\r\n" + 
                        "Try adding a board with a different name.";

                    MessageBox.Show(this, msg, "Herbi Speaks");

                    break;
                }
            }

            return boardNameIsUnique;
        }

        private void addBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            BoardName boardDlg = new BoardName(this, "Board " + (_boards.Count + 1).ToString());
            if (boardDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string boardName = boardDlg.BoardNameResult.Trim();

                bool addBoard = IsBoardNameUnique(boardName);
                if (addBoard)
                {
                    _fIsDirty = true;

                    AddNewBoard(boardName);

                    SwitchBoard(_boards.Count - 1, true);

                    SetCaptionText();
                }
            }
        }

        private void makeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            BoardName boardDlg = new BoardName(this, "Board " + (_boards.Count + 1).ToString());
            if (boardDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                int indexBoardBeingCloned = _currentBoardIndex;

                string boardName = boardDlg.BoardNameResult.Trim();

                bool addBoard = IsBoardNameUnique(boardName);
                if (addBoard)
                {
                    _fIsDirty = true;

                    // When we add a new board here, _currentBoardIndex will change to reference the new board.
                    AddNewBoard(boardName);

                    // Show the new empty board.
                    SwitchBoard(_currentBoardIndex, true);

                    for (int i = this.Controls.Count - 1; i >= 0; --i)
                    {
                        HerbiSpeaksButton button = this.Controls[i] as HerbiSpeaksButton;
                        if (button != null)
                        {
                            // Is this button on the current board?
                            if (button.BoardName == _boards[indexBoardBeingCloned].Name)
                            {
                                _currentButton = null;

                                // Create a copy of this button, and add it to the new board.
                                HerbiSpeaksButton clonedButton = AddNewButton(true, button.IsPictureButton);

                                SetClonedData(button, clonedButton);

                                if (button.IsPictureButton && (button.Controls.Count > 0))
                                {
                                    _currentButton = clonedButton;
                                    
                                    Control[] controls = new Control[button.Controls.Count];
                                    button.Controls.CopyTo(controls, 0);

                                    for (int j = 0; j < controls.Length; j++)
                                    {
                                        HerbiSpeaksButton containedButton = controls[j] as HerbiSpeaksButton;

                                        clonedButton = AddNewButton(true, false);

                                        SetClonedData(containedButton, clonedButton);
                                    }
                                }
                            }
                        }
                    }

                    SetCaptionText();
                }
            }
        }

        private void SetClonedData(HerbiSpeaksButton button, HerbiSpeaksButton clonedButton)
        {
            clonedButton.Text = button.Text;
            clonedButton.AccessibleName = button.AccessibleName;

            clonedButton.Location = button.Location;
            clonedButton.Size = button.Size;
            SetButtonCenterFromLocation(clonedButton);

            clonedButton.ForeColor = button.ForeColor;
            clonedButton.Font = button.Font;

            // By default, buttons are not transparent.
            clonedButton.BackColor = this.BackColor;
            clonedButton.FlatStyle = FlatStyle.Flat;

            clonedButton.FlatAppearance.BorderSize = 0;
            clonedButton.FlatAppearance.BorderColor = this.BackColor;

            clonedButton.ButtonTransparent = button.ButtonTransparent;
            if (clonedButton.ButtonTransparent)
            {
                clonedButton.BackColor = Color.Transparent;
                clonedButton.FlatStyle = FlatStyle.Popup;
            }

            clonedButton.ButtonTransparentOnHover = button.ButtonTransparentOnHover;

            clonedButton.Media = button.Media;
            clonedButton.ButtonTextSpokenBeforeMedia = button.ButtonTextSpokenBeforeMedia;
            clonedButton.AutoPlayMedia = button.AutoPlayMedia;

            clonedButton.TextPosition = button.TextPosition;

            clonedButton.ImageFull = button.ImageFull;
            clonedButton.ImageHoverFull = button.ImageHoverFull;
            clonedButton.Image = button.Image;
            clonedButton.BackgroundImage = button.BackgroundImage;
            clonedButton.BackgroundImageLayout = button.BackgroundImageLayout;

            clonedButton.TextAlign = button.TextAlign;
            clonedButton.ImageAlign = button.ImageAlign;

            clonedButton.ImageExtension = button.ImageExtension;
            clonedButton.ImageHoverExtension = button.ImageHoverExtension;

            clonedButton.BoardLink = button.BoardLink;
            clonedButton.BoardLinkSpoken = button.BoardLinkSpoken;

            clonedButton.BoardName = _boards[_currentBoardIndex].Name;
        }

        private void renameCurrentBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            BoardName boardDlg = new BoardName(this, _boards[_currentBoardIndex].Name);
            if (boardDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _fIsDirty = true;

                string newBoardName = boardDlg.BoardNameResult.Trim();

                bool addBoard = IsBoardNameUnique(newBoardName);
                if (addBoard)
                {
                    // For all buttons on this board, update their board name.
                    for (int i = this.Controls.Count - 1; i >= 0; --i)
                    {
                        HerbiSpeaksButton button = this.Controls[i] as HerbiSpeaksButton;
                        if (button != null)
                        {
                            if (button.BoardName == _boards[_currentBoardIndex].Name)
                            {
                                button.BoardName = newBoardName;

                                if (button.Controls.Count > 0)
                                {
                                    Control[] controls = new Control[button.Controls.Count];
                                    button.Controls.CopyTo(controls, 0);

                                    for (int j = 0; j < controls.Length; j++)
                                    {
                                        HerbiSpeaksButton containedButton = controls[j] as HerbiSpeaksButton;

                                        containedButton.BoardName = newBoardName;
                                    }
                                }
                            }

                            // If any buttons link to the board being renamed, update that link.
                            if (button.BoardLink == _boards[_currentBoardIndex].Name)
                            {
                                button.BoardLink = newBoardName;
                            }

                            if (button.Controls.Count > 0)
                            {
                                Control[] controls = new Control[button.Controls.Count];
                                button.Controls.CopyTo(controls, 0);

                                for (int j = 0; j < controls.Length; j++)
                                {
                                    HerbiSpeaksButton containedButton = controls[j] as HerbiSpeaksButton;

                                    if (containedButton.BoardLink == _boards[_currentBoardIndex].Name)
                                    {
                                        containedButton.BoardLink = newBoardName;
                                    }
                                }
                            }
                        }
                    }

                    // Now update the name of the board itself.
                    _boards[_currentBoardIndex].Name = newBoardName;

                    SetCaptionText();
                }
            }
        }

        private void deleteCurrentBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            if (MessageBox.Show(this, "Are you sure you want to delete board \"" + _boards[_currentBoardIndex].Name + "\"?", "Delete Board", 
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                _fIsDirty = true;

                for (int i = this.Controls.Count - 1; i >= 0; --i)
                {
                    HerbiSpeaksButton button = this.Controls[i] as HerbiSpeaksButton;
                    if (button != null)
                    {
                        if (button.BoardName == _boards[_currentBoardIndex].Name)
                        {
                            Control control = this.Controls[i];

                            // Remove all this button's contained controls first.
                            for (int j = control.Controls.Count - 1; j >= 0; j--)
                            {
                                control.Controls.RemoveAt(j);
                            }

                            this.Controls.RemoveAt(i);
                        }

                        // If any buttons link to the board being deleted, remove that link.
                        if (button.BoardLink == _boards[_currentBoardIndex].Name)
                        {
                            button.BoardLink = null;
                        }
                    }
                }

                _boards.RemoveAt(_currentBoardIndex);
                _currentBoardIndex = 0;

                SwitchBoard(_currentBoardIndex, true);

                SetCaptionText();
            }
        }

        private void reorderBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            ReorderBoard dlg = new ReorderBoard(_boards);
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Collection<HerbiSpeaksBoard> newBoards = new Collection<HerbiSpeaksBoard>();

                for (int i = 0; i < dlg.ReorderedBoards.Count(); ++i)
                {
                    for (int j = 0; j < _boards.Count; ++j)
                    {
                        if (dlg.ReorderedBoards[i] == _boards[j].Name)
                        {
                            newBoards.Add(_boards[j]);

                            break;
                        }
                    }
                }

                _boards.Clear();

                _boards = newBoards;

                SwitchBoard(0, true);
            }
        }

///* MEDIA ***************************************************************************
        private bool PlayMedia(string mediaFullPath)
        {
            if (axWindowsMediaPlayer1 == null)
            {
                Debug.WriteLine("Herbi Speaks: PlayMedia() called when WMP not available.");

                _outputInProgress = false;

                return false;
            }

            bool playingMedia = false;

            // Were we supplied with a media file path?
            if (!String.IsNullOrEmpty(mediaFullPath))
            {
                // Does this file exist?
                if (!File.Exists(mediaFullPath))
                {
                    // The media file wasn't found at the referenced path, so see if it exists
                    // in the same folder as where the Herbi Speaks board file is located.
                    if (!String.IsNullOrEmpty(mediaFullPath) && !String.IsNullOrEmpty(_currentFilename))
                    {
                        var filePathAttempt = Path.GetDirectoryName(_currentFilename) +
                            "\\" + Path.GetFileName(mediaFullPath);

                        if (File.Exists(filePathAttempt))
                        {
                            // Use this backup path.
                            mediaFullPath = filePathAttempt;
                        }
                        else
                        {
                            MessageBox.Show(this,
                                "Sorry, the media file \"" + mediaFullPath + "\" couldn't be found on this computer.\r\n\r\n" +
                                "Please copy the media file into the referenced folder on this computer, or reference " +
                                "a different media file which is available on this computer.",
                                "Herbi Speaks");

                            Debug.WriteLine("Herbi Speaks: Media file not found, " + mediaFullPath);

                            mediaFullPath = null;
                        }
                    }
                }
            }

            if (String.IsNullOrEmpty(mediaFullPath))
            {
                _outputInProgress = false;

                axWindowsMediaPlayer1.Visible = false;
                axWindowsMediaPlayer1.Bounds = new Rectangle(0, 0, 0, 0);

                if (axWindowsMediaPlayer1.URL != "")
                {
                    axWindowsMediaPlayer1.URL = "";
                }
            }
            else
            {
                string mediaFileExtension = Path.GetExtension(mediaFullPath).ToLower();
                bool isVideo = (mediaFileExtension == ".asf" || mediaFileExtension == ".avi" ||
                                mediaFileExtension == ".mov" || mediaFileExtension == ".mp4" ||
                                mediaFileExtension == ".qt" || mediaFileExtension == ".wm" ||
                                mediaFileExtension == ".wmv");

                axWindowsMediaPlayer1.uiMode = "none";
                axWindowsMediaPlayer1.enableContextMenu = false;

                if (isVideo)
                {
                    axWindowsMediaPlayer1.Visible = true;
                    // axWindowsMediaPlayer1.windowlessVideo = true;

                    Rectangle videoBounds = _currentButton.Bounds;

                    if (_currentButton != null)
                    {
                        if (IsButtonContainedInPicture(_currentButton))
                        {
                            HerbiSpeaksButton parent = _currentButton.Parent as HerbiSpeaksButton;

                            videoBounds.Offset(parent.Left, parent.Top);
                        }
                    }

                    // videoBounds.Inflate(-8, -8);
                    axWindowsMediaPlayer1.Bounds = videoBounds;
                }
                else
                {
                    axWindowsMediaPlayer1.Visible = false;
                    axWindowsMediaPlayer1.Bounds = new Rectangle(0, 0, 0, 0);
                }

                try
                {
                    axWindowsMediaPlayer1.URL = mediaFullPath;

                    playingMedia = true;

                    _outputInProgress = true;
                }
                catch
                {
                    MessageBox.Show(this,
                        "Sorry, this file can't be played.",
                        "Herbi Speaks");
                }
            }

            return playingMedia;
        }

        private void importBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayMedia("");

            ImportBoards dlg = new ImportBoards(this);
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _fIsDirty = true;
            }
       }

//MEDIA ***************************************************************************/

        // BARKER: Remove printing!
/************************************************************************************************
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // BARKER: The full page isn't printed!
            // BARKER: The (default) white button text won't appear on the white background!

            // Capture the screen before we show the PrintDialog.

            // Barker: Do we really have to do a screen capture here?
            // Barker: Always turn off the background while we capture the image.

            Color currentBackgroundColor = this.BackColor;
            this.BackColor = Color.White;
            this.Refresh();

            CaptureScreen();

            this.BackColor = currentBackgroundColor;

            PrintDialog dlg = new PrintDialog();

            // Allow the user to choose the page range to be printed.
            // Barker: Enable this. (Today the print dlg may be saying All will be printed, 
            // when in fact we'll only print the current board.)
            // dlg.AllowSomePages = true;

            PrintDocument docToPrint = new PrintDocument();
            docToPrint.PrintPage += new PrintPageEventHandler(docToPrint_PrintPage);

            // Set the Document property to the PrintDocument for which the PrintPage Event has been handled. 
            // To display the dialog, either this property or the PrinterSettings property must be set.
            dlg.Document = docToPrint;

            // Note that it's up to the user to selected portrait or landscape.
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                docToPrint.Print();
            }
        }

        Bitmap memoryImage;

        private void CaptureScreen()
        {
            Point ptScreen = this.PointToScreen(new Point(0, 0));

            Graphics myGraphics = this.CreateGraphics();
            Size s = this.ClientSize;
            memoryImage = new Bitmap(s.Width, s.Height, myGraphics);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            memoryGraphics.CopyFromScreen(ptScreen.X, ptScreen.Y + this.menuStripApp.Height, 0, 0, s);
        }

        private void docToPrint_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(memoryImage, 0, 0);
        }
*************************************************************************************************/

    }
}
