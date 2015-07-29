using System;
using System.ComponentModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class BreadcrumbItem : BreadcrumbItemBase
    {
        public override sealed Node Node
        {
            get; 
            set;
        }

        public override string Name
        {
            get
            {
                var name = Node.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }
                return "(No Name)";
            }
        }

        public BreadcrumbItem(Node node)
        {
            Node = node;
            Node.PropertyChanged += NodeOnPropertyChanged;
        }

        private void NodeOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                OnNotifyPropertyChanged("Name");
            }
        }
    }
}
