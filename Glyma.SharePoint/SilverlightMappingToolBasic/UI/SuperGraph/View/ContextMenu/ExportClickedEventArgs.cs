using System;
using System.Collections.Generic;
using Glyma.UtilityService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public class ExportClickedEventArgs : EventArgs
    {
        private List<Guid> _selectedNodes; 
        public ExportType Type { get; set; }

        public List<Guid> SelectedNodes
        {
            get
            {
                if (_selectedNodes == null)
                {
                    _selectedNodes = new List<Guid>();
                }
                return _selectedNodes;
            }
        }
    }
}
