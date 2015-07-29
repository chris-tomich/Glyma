using System;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses.Edit
{
    public class NameChangedEventArgs:EventArgs
    {
        public string OldText { get; set; }
        public string NewText { get; set; }
    }
}
