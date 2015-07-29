using System;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole
{
    public class MetadataChangedEventArgs : EventArgs
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
