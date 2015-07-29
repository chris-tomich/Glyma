using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses
{
    public partial class BreadcrumbContainer : UserControl
    {
        public BreadcrumbContainer()
        {
            InitializeComponent();
        }

        public IEnumerable<BreadcrumbControl> BreadcrumbControls
        {
            get
            {
                return LayoutRoot.Children.OfType<BreadcrumbControl>();
            }
        }

        public int Count
        {
            get
            {
                return LayoutRoot.Children.Count;
            }
        }

        public void Add(BreadcrumbControl control)
        {
            if (BreadcrumbControls.Contains(control))
            {
                return;
            }

            foreach (var breadcrumb in BreadcrumbControls)
            {
                breadcrumb.BreadcrumbData.HasVisibleProperties = false;
            }
            LayoutRoot.Children.Add(control);
        }

        public int IndexOf(BreadcrumbControl control)
        {
            if (LayoutRoot.Children.Contains(control))
            {
                return LayoutRoot.Children.IndexOf(control);
            }
            return -1;
        }

        public void RemoveAt(int index)
        {
            LayoutRoot.Children.RemoveAt(index);
        }

        public void Clear()
        {
            LayoutRoot.Children.Clear();
        }
    }
}
