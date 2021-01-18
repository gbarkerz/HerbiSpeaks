using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HerbiSpeaks
{
    public class HerbiSpeaksBoard
    {
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        private string _name;
        private Color _backgroundColor;
    }
}
