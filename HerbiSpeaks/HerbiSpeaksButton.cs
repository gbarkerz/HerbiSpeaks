using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace HerbiSpeaks
{
    class HerbiSpeaksButton : Button
    {
        public bool IsPictureButton
        {
            get { return _isPictureButton; }
            set { _isPictureButton = value; }
        }

        public string Media
        {
            get { return _media; }
            set { _media = value; }
        }

        public bool ButtonTextSpokenBeforeMedia
        {
            get { return _buttonTextSpokenBeforeMedia; }
            set { _buttonTextSpokenBeforeMedia = value; }
        }

        public bool AutoPlayMedia
        {
            get { return _autoPlayMedia; }
            set { _autoPlayMedia = value; }
        }

        public string ImageExtension
        {
            get { return _imageExtension; }
            set { _imageExtension = value; }
        }

        public string ImageHoverExtension
        {
            get { return _imageHoverExtension; }
            set { _imageHoverExtension = value; }
        }

        public double CenterX
        {
            get { return _centerX; }
            set { _centerX = value; }
        }

        public double CenterY
        {
            get { return _centerY; }
            set { _centerY = value; }
        }

        public string BoardName
        {
            get { return _boardName; }
            set { _boardName = value; }
        }

        public string BoardLink
        {
            get { return _boardLink; }
            set { _boardLink = value; }
        }

        public bool BoardLinkSpoken
        {
            get { return _boardLinkSpoken; }
            set { _boardLinkSpoken = value; }
        }

        public string TextPosition
        {
            get { return _textPosition; }
            set { _textPosition = value; }
        }

        public Image ImageFull
        {
            get { return _imageFull; }
            set { _imageFull = value; }
        }

        public Image ImageHoverFull
        {
            get { return _imageHoverFull; }
            set { _imageHoverFull = value; }
        }

        public Image ImageFullThumbnail
        {
            get { return _imageFullThumbnail; }
            set { _imageFullThumbnail = value; }
        }

        public Image ImageHoverFullThumbnail
        {
            get { return _imageHoverFullThumbnail; }
            set { _imageHoverFullThumbnail = value; }
        }

        public bool ButtonTransparent
        {
            get { return _buttonTransparent; }
            set { _buttonTransparent = value; }
        }

        public bool ButtonTransparentOnHover
        {
            get { return _buttonTransparentOnHover; }
            set { _buttonTransparentOnHover = value; }
        }

        private bool _isPictureButton;
        private string _media;
        private bool _buttonTextSpokenBeforeMedia;
        private bool _autoPlayMedia;
        private string _imageExtension;
        private string _imageHoverExtension;
        private double _centerX;
        private double _centerY;
        private string _boardName;
        private string _boardLink;
        private bool _boardLinkSpoken;
        private string _textPosition;
        private Image _imageFull;
        private Image _imageHoverFull;
        private Image _imageFullThumbnail;
        private Image _imageHoverFullThumbnail;
        private bool _buttonTransparent;
        private bool _buttonTransparentOnHover;
    }
}
