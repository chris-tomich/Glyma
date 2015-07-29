using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMappingToolBasic
{
    public class RenderingContextInfo
    {
        private double _surfaceWidth = -1;
        private double _surfaceHeight = -1;

        public RenderingContextInfo()
        {
        }

        public double SurfaceTopLeftX
        {
            get;
            set;
        }

        public double SurfaceTopLeftY
        {
            get;
            set;
        }

        public double SurfaceWidth
        {
            get
            {
                if (_surfaceWidth == -1)
                {
                    throw new ArgumentNullException("No surface width has been defined.");
                }

                return _surfaceWidth;
            }
            set
            {
                _surfaceWidth = value;
            }
        }

        public double SurfaceHeight
        {
            get
            {
                if (_surfaceHeight == -1)
                {
                    throw new ArgumentNullException("No surface height has been defined.");
                }

                return _surfaceHeight;
            }
            set
            {
                _surfaceHeight = value;
            }
        }
    }
}
