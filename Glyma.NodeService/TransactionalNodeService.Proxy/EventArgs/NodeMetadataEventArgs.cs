﻿using System;
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

namespace TransactionalNodeService.Proxy
{
    public class NodeMetadataEventArgs : EventRegisterEventArgs
    {
        public NodeMetadataEventArgs()
            : base()
        {
        }

        public NodeMetadataCollection Metadata
        {
            get;
            set;
        }
    }
}