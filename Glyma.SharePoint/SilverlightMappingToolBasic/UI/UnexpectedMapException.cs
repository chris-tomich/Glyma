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

namespace SilverlightMappingToolBasic.UI
{
    public class UnexpectedMapException : Exception
    {
        public UnexpectedMapException()
            : base()
        {
        }

        public UnexpectedMapException(string message)
            : base(message)
        {
        }

        public UnexpectedMapException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
