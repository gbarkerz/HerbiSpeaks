using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace HerbiSpeaks
{
    public partial class HerbiSpeaks
    {
        void HerbiSpeaks_KeyDown(object sender, KeyEventArgs e)
        {
            // Always cancel speech on a key down.
            CancelSpeech();

            PlayMedia("");

            HandleKeyPress(e.KeyCode);
        }

        void HandleKeyPress(Keys key)
        {
            PlayMedia("");

            if (key == Keys.Escape)
            {
                SetFullscreenState(false);
            }
            else if (key == Keys.F5)
            {
                SetFullscreenState(true);
            }
        }

        void HerbiSpeaks_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_outputInProgress)
            {
                _currentButton = null;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CancelSpeech();

                PlayMedia("");

                BuildContextMenu(false);
            }
        }

        void button_KeyDown(object sender, KeyEventArgs e)
        {
            // Always cancel speech on a key down.
            CancelSpeech();

            PlayMedia("");

            HandleKeyPress(e.KeyCode);
        }

        void button_MouseMove(object sender, MouseEventArgs e)
        {
            HerbiSpeaksButton btn = (HerbiSpeaksButton)sender;

            if (_editModeActive)
            {
                if (!_fDragging)
                {
                    if ((e.Location.X < _dragCornerSize) && (e.Location.Y < _dragCornerSize))
                    {
                        this.Cursor = Cursors.SizeNWSE;
                    }
                    else if ((btn.Width - e.Location.X < _dragCornerSize) && (e.Location.Y < _dragCornerSize))
                    {
                        this.Cursor = Cursors.SizeNESW;
                    }
                    else if ((e.Location.X < _dragCornerSize) && (btn.Height - e.Location.Y < _dragCornerSize))
                    {
                        this.Cursor = Cursors.SizeNESW;
                    }
                    else if ((btn.Width - e.Location.X < _dragCornerSize) && (btn.Height - e.Location.Y < _dragCornerSize))
                    {
                        this.Cursor = Cursors.SizeNWSE;
                    }
                    else
                    {
                        this.Cursor = Cursors.SizeAll;
                    }
                }
                else
                {
                    Point delta = new Point(e.X - _dropOffset.X, e.Y - _dropOffset.Y);

                    bool fAddOffset = true;

                    if (_resizingButtonTopLeft)
                    {
                        _rectDragFeedback.Size = new Size(Math.Max(64, Math.Abs(_originalSize.Width - delta.X)),
                                                          Math.Max(64, Math.Abs(_originalSize.Height - delta.Y)));

                        _rectDragFeedback.Location = new Point(_btnBeingMoved.Location.X + delta.X, _btnBeingMoved.Location.Y + delta.Y);
                    }
                    else if (_resizingButtonTopRight)
                    {
                        _rectDragFeedback.Size = new Size(Math.Max(64, Math.Abs(_originalSize.Width + delta.X)),
                                                          Math.Max(64, Math.Abs(_originalSize.Height - delta.Y)));

                        _rectDragFeedback.Location = new Point(_btnBeingMoved.Location.X, _btnBeingMoved.Location.Y + delta.Y);
                    }
                    else if (_resizingButtonBottomLeft)
                    {
                        _rectDragFeedback.Size = new Size(Math.Max(64, Math.Abs(_originalSize.Width - delta.X)),
                                                          Math.Max(64, Math.Abs(_originalSize.Height + delta.Y)));

                        _rectDragFeedback.Location = new Point(_btnBeingMoved.Location.X + delta.X, _btnBeingMoved.Location.Y);
                    }
                    else if (_resizingButtonBottomRight)
                    {
                        _rectDragFeedback.Size = new Size(Math.Max(64, Math.Abs(_originalSize.Width + delta.X)),
                                                          Math.Max(64, Math.Abs(_originalSize.Height + delta.Y)));

                        _rectDragFeedback.Location = new Point(_btnBeingMoved.Location.X, _btnBeingMoved.Location.Y);
                    }
                    else
                    {
                        fAddOffset = false;

                        _rectDragFeedback.Location = new Point(_originalLocation.X + (e.Location.X - _dropOffset.X),
                                                               _originalLocation.Y + (e.Location.Y - _dropOffset.Y));
                        _rectDragFeedback.Size = _btnBeingMoved.Size;
                    }

                    if (!_rectDragFeedbackPrevious.IsEmpty)
                    {
                        if (fAddOffset)
                        {
                            Button buttonParent = _currentButton.Parent as Button;
                            if (buttonParent != null)
                            {
                                _rectDragFeedback.X += buttonParent.Location.X;
                                _rectDragFeedback.Y += buttonParent.Location.Y;
                            }
                        }

                        _rectDragFeedbackFull = Rectangle.Union(_rectDragFeedbackPrevious, _rectDragFeedback);
                        this.Invalidate(_rectDragFeedbackFull, false);
                    }

                    _rectDragFeedbackPrevious = _rectDragFeedback;
                }
            }
        }

        void button_MouseEnter(object sender, EventArgs e)
        {
            HerbiSpeaksButton btn = (HerbiSpeaksButton)sender;

            if (btn.IsPictureButton)
            {
                return;
            }

            // Always try to set a big border around the button when the mouse is over it.

            btn.FlatAppearance.BorderColor = btn.ForeColor;
            btn.FlatAppearance.BorderSize = _borderThickness;

            // Is this button on a picture, and the button is usually transparent?
            if (IsButtonContainedInPicture(btn) && btn.ButtonTransparent)
            {
                // If this button is to remain transparent when the mouse is over it
                // don't change its background when the mouse is over it.
                if (!btn.ButtonTransparentOnHover)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = this.BackColor;
                }
            }

            if (btn.ImageHoverFull != null)
            {
                if (btn.TextPosition == "Middle")
                {
                    btn.BackgroundImage = btn.ImageHoverFull;
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.Image = null;
                }
                else
                {
                    btn.Image = btn.ImageHoverFullThumbnail;
                    btn.BackgroundImage = null;
                }
            }
        }

        void button_MouseLeave(object sender, EventArgs e)
        {
            HerbiSpeaksButton btn = (HerbiSpeaksButton)sender;

            // Always try to set no border on the button when the mouse is outside it.
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.BorderColor = this.BackColor;

            // Is this button on a picture, and the button is usually transparent?
            if (IsButtonContainedInPicture(btn) && btn.ButtonTransparent)
            {
                // If we made it not transparent when the mouse was over it, set it back
                // to being transparent now. Note that if the button is always transparent,
                // we did change its background when the mouse was over it.
                if (!btn.ButtonTransparentOnHover)
                {
                    btn.FlatStyle = FlatStyle.Popup;
                    btn.BackColor = Color.Transparent;
                }
            }
            else
            {
                // In all other cases, when the mouse moves out of a button, we
                // want it to have no border, and effectively no background colour.

                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = this.BackColor;
            }

            this.Cursor = Cursors.Default;

            if (btn.ImageHoverFull != null)
            {
                if (btn.ImageFull != null)
                {
                    if (btn.TextPosition == "Middle")
                    {
                        btn.BackgroundImage = btn.ImageFull;
                        btn.BackgroundImageLayout = ImageLayout.Stretch;
                        btn.Image = null;
                    }
                    else
                    {
                        btn.Image = btn.ImageFullThumbnail;
                        btn.BackgroundImage = null;
                    }
                }
                else
                {
                    btn.BackgroundImage = null;
                    btn.Image = null;
                }
            }
        }

        void button_MouseDown(object sender, MouseEventArgs e)
        {
            this.ContextMenu = null;

            HerbiSpeaksButton btn = sender as HerbiSpeaksButton;

            // In all case below, set _currentButton = btn, except when
            // a left mouse click is done and output is still in progress.

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (_editModeActive)
                {
                    _currentButton = btn;

                    indexSwitchBoardAfterOutput = -1;
                    strPlayMediaAfterSpeech = "";

                    _dropOffset = e.Location;

                    _resizingButtonTopLeft = false;
                    _resizingButtonTopRight = false;
                    _resizingButtonBottomLeft = false;
                    _resizingButtonBottomRight = false;

                    _originalLocation = btn.Location;
                    _originalSize = btn.Size;

                    if ((_dropOffset.X < _dragCornerSize) && (_dropOffset.Y < _dragCornerSize))
                    {
                        _resizingButtonTopLeft = true;

                        this.Cursor = Cursors.SizeNWSE;
                    }
                    else if ((btn.Width - _dropOffset.X < _dragCornerSize) && (_dropOffset.Y < _dragCornerSize))
                    {
                        _resizingButtonTopRight = true;

                        this.Cursor = Cursors.SizeNESW;
                    }
                    else if ((_dropOffset.X < _dragCornerSize) && (btn.Height - _dropOffset.Y < _dragCornerSize))
                    {
                        _resizingButtonBottomLeft = true;

                        this.Cursor = Cursors.SizeNESW;
                    }
                    else if ((btn.Width - _dropOffset.X < _dragCornerSize) && (btn.Height - _dropOffset.Y < _dragCornerSize))
                    {
                        _resizingButtonBottomRight = true;

                        this.Cursor = Cursors.SizeNWSE;
                    }
                    else
                    {
                        this.Cursor = Cursors.SizeAll;
                    }

                    _rectDragFeedbackPrevious.Width = 0;
                    _rectDragFeedbackPrevious.Height = 0;

                    Button buttonParent = _currentButton.Parent as Button;
                    if (buttonParent != null)
                    {
                        _originalLocation.X += buttonParent.Location.X;
                        _originalLocation.Y += buttonParent.Location.Y;
                    }

                    _fDragging = true;
                    _btnBeingMoved = btn;

                    btn.Capture = true;
                }
                else
                {
                    // If speech is in progress, ignore the left mouse click.
                    if (!_outputInProgress)
                    {
                        _currentButton = btn;

                        if (!btn.IsPictureButton)
                        {
                            InvokeButton(btn);
                        }
                    }
                }
            }
            else
            {
                _currentButton = btn;

                // Always cancel speech on a right click.
                CancelSpeech();
            }
        }

        private void InvokeButton(HerbiSpeaksButton btn)
        {
            indexSwitchBoardAfterOutput = -1;
            strPlayMediaAfterSpeech = "";

            // Find a board to jump to in response to this click, if there is a board.
            if (btn.BoardLink != null)
            {
                if (btn.BoardLink == "<random>")
                {
                    if (_boards.Count > 1)
                    {
                        int randomBoardIndex;

                        Random rand = new Random();

                        do
                        {
                            randomBoardIndex = rand.Next(_boards.Count);
                        }
                        while (_currentBoardIndex == randomBoardIndex);

                        this.indexSwitchBoardAfterOutput = randomBoardIndex;                           
                    }
                }
                else
                {
                    for (int i = 0; i < _boards.Count; ++i)
                    {
                        if (_boards[i].Name == btn.BoardLink)
                        {
                            this.indexSwitchBoardAfterOutput = i;

                            break;
                        }
                    }
                }
            }

            bool startedOutput = false;

            bool fGotMedia = ((btn.Media != null) && (btn.Media != ""));

            // If we're about to play media, check whether the button text should be spoken first.
            if (!fGotMedia || btn.ButtonTextSpokenBeforeMedia)
            {
                // If this button has no target board, or if it does have a target board 
                // and text is to be spoken regardless, speak the text aynchronously now.
                if ((btn.BoardLink == null) || (btn.BoardLink == "") || (btn.BoardLinkSpoken))
                {
                    String speakText = btn.Text;

                    if (speakText == "")
                    {
                        speakText = btn.AccessibleName;
                    }

                    if (speakText != "")
                    {
                        startedOutput = true;

                        System.Diagnostics.Debug.WriteLine("SpeakAsync: " + speakText);

                        _outputInProgress = true;

                        // We will switch boards if necessary when the speech has completed.
                        synth.SpeakAsync(speakText);
                    }
                }
            }

            if (!startedOutput)
            {
                startedOutput = PlayMedia(btn.Media);
            }
            else if ((btn.Media != null) && (btn.Media != ""))
            {
                strPlayMediaAfterSpeech = btn.Media;
            }

            // If we need to switch boards and no speech has been started, switch boards now.
            if ((this.indexSwitchBoardAfterOutput != -1) && !startedOutput)
            {
                int targetBoardIndex = this.indexSwitchBoardAfterOutput;

                this.indexSwitchBoardAfterOutput = -1;

                SwitchBoard(targetBoardIndex, true);
            }
        }

        void button_MouseUp(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;

            HerbiSpeaksButton btn = (HerbiSpeaksButton)sender;

            btn.Capture = false;

            Control ctl = (Control)sender;

            Point originalDragScreenPt = new Point(_originalLocation.X + _dropOffset.X, _originalLocation.Y + _dropOffset.Y);

            Point delta = new Point(e.X - _dropOffset.X, e.Y - _dropOffset.Y);

            if (_fDragging)
            {
                _currentButton = btn;

                _fIsDirty = true;

                if (_resizingButtonTopLeft)
                {
                    btn.Size = new Size(Math.Max(64, Math.Abs(_originalSize.Width - delta.X)),
                                        Math.Max(64, Math.Abs(_originalSize.Height - delta.Y)));

                    btn.Location = new Point(btn.Location.X + delta.X, btn.Location.Y + delta.Y);
                }
                else if (_resizingButtonTopRight)
                {
                    btn.Size = new Size(Math.Max(64, Math.Abs(_originalSize.Width + delta.X)),
                                        Math.Max(64, Math.Abs(_originalSize.Height - delta.Y)));

                    btn.Location = new Point(btn.Location.X, btn.Location.Y + delta.Y);
                }
                else if (_resizingButtonBottomLeft)
                {
                    btn.Size = new Size(Math.Max(64, Math.Abs(_originalSize.Width - delta.X)),
                                        Math.Max(64, Math.Abs(_originalSize.Height + delta.Y)));

                    btn.Location = new Point(btn.Location.X + delta.X, btn.Location.Y);
                }
                else if (_resizingButtonBottomRight)
                {
                    btn.Size = new Size(Math.Max(64, Math.Abs(_originalSize.Width + delta.X)),
                                        Math.Max(64, Math.Abs(_originalSize.Height + delta.Y)));
                }
                else
                {
                    delta = new Point(e.X - _dropOffset.X, e.Y - _dropOffset.Y);

                    btn.Location = new Point(_originalLocation.X + delta.X, _originalLocation.Y + delta.Y);


                    Button buttonParent = _currentButton.Parent as Button;
                    if (buttonParent != null)
                    {
                        btn.Left -= buttonParent.Location.X;
                        btn.Top -= buttonParent.Location.Y;

                        if (btn.Left < 0)
                        {
                            btn.Left = 0;
                        }

                        if (btn.Top < 0)
                        {
                            btn.Top = 0;
                        }

                        if (btn.Right > buttonParent.Width)
                        {
                            btn.Left = buttonParent.Width - btn.Width;
                        }

                        if (btn.Bottom > buttonParent.Height)
                        {
                            btn.Top = buttonParent.Height - btn.Height;
                        }
                    }

                }

                SetButtonCenterFromLocation(btn);

                _resizingButtonTopLeft = false;
                _resizingButtonTopRight = false;
                _resizingButtonBottomLeft = false;
                _resizingButtonBottomRight = false;

                _fDragging = false;

                ReloadThumbnail(btn);

                _btnBeingMoved = null;

                this.Update();
                this.Refresh();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)            
            {
                _currentButton = btn;

                PlayMedia("");

                BuildContextMenu(true);
            }
        }
    }
}
