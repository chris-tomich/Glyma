namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole
{
    public class UpdateValue
    {
        public bool IsChecked { get; set; }

        public bool IsUpdatable { get; set; }

        public UpdateValue(bool isChecked, bool isUpdatable = true)
        {
            IsChecked = isChecked;
            IsUpdatable = isUpdatable;
        }
    }
}
