using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverlightMappingToolBasic.Code.ColorsManagement;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Loading
{
    public partial class Loader : Grid
    {
        private readonly static DependencyProperty ColourProperty = DependencyProperty.Register("Colour", typeof(SolidColorBrush), typeof(Loader), new PropertyMetadata(new SolidColorBrush(ColorConverter.FromHex("#FF1F3B53"))));
        
        public Brush Colour
        {
            get { return GetValue(ColourProperty) as SolidColorBrush; }
            set { SetValue(ColourProperty, value); }
        }

        public Loader()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
