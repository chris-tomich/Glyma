using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.RatingControl
{
    public partial class SuperRating : MenuItem
    {
        public ViewModel.NodeProperties Rate
        {
            get
            {
                return DataContext as ViewModel.NodeProperties;
            }
        }

        public SuperRating()
        {
            InitializeComponent();
        }
    }
}
