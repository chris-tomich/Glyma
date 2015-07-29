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
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.Controls
{
    public class AvailableDomainsEventArgs : EventArgs
    {
        public AvailableDomainsEventArgs()
        {
        }

        public AvailableDomainsEventArgs(Dictionary<string, Guid> domains)
        {
            Domains = domains;
        }

        public Dictionary<string, Guid> Domains
        {
            get;
            private set;
        }
    }
}
