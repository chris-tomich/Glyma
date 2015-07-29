using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel;
using Telerik.Windows.Controls;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole
{
    public class MasterDetailTemplateSelector: DataTemplateSelector
    {
        public DataTemplate ProjectTemplate { get; set; }

        public DataTemplate RootMapTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var project = item as Project;
            if (project != null)
            {
                return this.ProjectTemplate;
            }

            var rootMap = item as RootMap;
            if (rootMap != null)
            {
                return this.RootMapTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
