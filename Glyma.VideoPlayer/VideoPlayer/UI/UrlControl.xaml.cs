using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using VideoPlayer.Controller;
using VideoPlayer.Controller.Listener;
using VideoPlayerSharedLib;

namespace VideoPlayer.UI
{
    public partial class UrlControl : UserControl
    {
        internal VideoPlayerMainContainer Remote;

        public class UrlControlData : INotifyPropertyChanged
        {
            private string _source;
            public string Source
            {
                get { return _source;}
                set {
                    _source = value;
                    OnPropertyChanged("Source");
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public UrlControlData Data;

        public UrlControl()
        {
            Data = new UrlControlData { Source = string.Empty };
            DataContext = Data;
            InitializeComponent();
        }

        private void GoButton_Clicked(object sender, RoutedEventArgs e)
        {
            Remote.MediaElementControl.Play(new Command
            {
                Name = "Play",
                Params = new List<Param>
                {
                    {new Param() {Name="Source",Value=UrlBox.Text}},
                    {new Param() {Name="AutoPlay",Value="true"}},
                    {new Param() {Name="NodeId",Value="00000000-0000-0000-0000-000000000000"}},
                }
            });
        }

        private void UrlBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Remote.MediaElementControl.Play(new Command
                {
                    Name = "Play",
                    Params = new List<Param>
                {
                    {new Param() {Name="Source",Value=UrlBox.Text}},
                    {new Param() {Name="AutoPlay",Value="true"}},
                    {new Param() {Name="NodeId",Value="00000000-0000-0000-0000-000000000000"}},
                }
                });
            }
        }
    }
}
