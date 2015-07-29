using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using SilverlightMappingToolBasic.UI.ViewModel;


namespace SilverlightMappingToolBasic
{
    public class RootMapProperties
    {
        private static RootMapProperties _instance;
        private IDictionary<string, string> _metadata;

        private RootMapProperties()
        {
            _metadata = new Dictionary<string, string>();
        }

        public static RootMapProperties Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RootMapProperties();
                }
                return _instance;
            }
        }

        public IDictionary<string, string> Metadata
        {
            get { return _metadata; }
            set { _metadata = value; }
        }

        public Guid Id 
        { 
            get; 
            set; 
        }

        public Guid DomainId
        {
            get;
            set;
        }
    }
}
