using System;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses
{
    public class InsertImageEventArgs : EventArgs
    {
        public Uri Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Extension { get; set; }
    }
}
