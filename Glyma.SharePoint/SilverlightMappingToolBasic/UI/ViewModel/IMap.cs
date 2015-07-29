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

namespace SilverlightMappingToolBasic.UI.ViewModel
{
    public interface IMap
    {
        IDictionary<Guid, INode> Nodes { get; set; }
        IDictionary<Guid, IRelationship> Relationships { get; set; }
    }
}
