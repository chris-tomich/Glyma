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
using TransactionalNodeService.Proxy;

namespace TransactionalNodeService.Proxy
{
    public interface INodeTuple
    {
        INode Node { get; }
        ConnectionType ConnectionType { get; }
    }
}