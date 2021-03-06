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

namespace TransactionalNodeService.InProcess
{
    internal class DelayedNodeAction
    {
        public Proxy.TransactionActionType Action
        {
            get;
            set;
        }

        public Proxy.NodeType NodeType
        {
            get;
            set;
        }
    }
}
