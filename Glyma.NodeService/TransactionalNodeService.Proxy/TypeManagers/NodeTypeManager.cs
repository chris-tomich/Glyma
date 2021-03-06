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
using Proxy = TransactionalNodeService.Proxy;
using Service = TransactionalNodeService.Service;

namespace TransactionalNodeService.Proxy
{
    public class NodeTypeManager : TypeManager<Service.NT, Proxy.NodeType>
    {
        public NodeTypeManager()
        {
        }
    }
}
