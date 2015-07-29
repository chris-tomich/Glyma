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

namespace Glyma.UtilityService.Proxy
{
    public class ExportJobCollection : Dictionary<Guid, IExportJob>
    {
        public ExportJobCollection()
            : base()
        {
        }

        public ExportJobCollection(int capacity)
            : base(capacity)
        {
        }

        public ExportJobCollection(IDictionary<Guid, IExportJob> dictionary)
            : base(dictionary)
        {
        }
    }
}
