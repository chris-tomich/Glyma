using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SilverlightMappingToolBasic.UI
{
    public partial class Loading : ChildWindow
    {
        private int _value;

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value >= 0 && value <= 100)
                    _value = value;
                else if (value > 100)
                    _value = 100;
                else
                    _value = 0;
                Progress.Text = string.Format("{0} %", _value);
            }
        }

        public bool IsProgressVisible
        {
            get
            {
                return Progress.Visibility == Visibility.Visible;
            }
            set
            {
                Progress.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string LeftBottomStatusText
        {
            set
            {
                LeftBottomStatus.Text = value;
            }
            get
            {
                return LeftBottomStatus.Text;
            }
        }

        public string RightBottomStatusText
        {
            set
            {
                RightBottomStatus.Text = value;
            }
            get
            {
                return RightBottomStatus.Text;
            }
        }

        public Loading()
        {
            InitializeComponent();
        }

        private void LoadingChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.RootVisual.SetValue(IsEnabledProperty, true);
        }



    }


}

