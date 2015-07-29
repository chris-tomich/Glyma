using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Glyma.UtilityService.Proxy
{
    public class GlymaUser : IGlymaUser
    {
        private string _displayName;

        public string Name
        {
            get; 
            set;
        }

        public string DisplayName
        {
            get
            {
                if (_displayName == null)
                {
                    _displayName = ProcessName(Name);
                }
                return _displayName;
            }
        }

        public GlymaUser(string name)
        {
            Name = name;
        }

        private string ProcessName(string name)
        {
            if (name.Contains(";"))
            {
                //AD based names like: S-1-5-21-3851129750-3191885917-1653347546-2606;GLYMADEMO\sp_web
                //or 24;#John User
                int semiColonPos = name.IndexOf(";");
                name = name.Substring(semiColonPos + 1);
            }
            else if (name.Contains("|"))
            {
                //Claims based names like: 05.t|social network|paul.culmsee@sevensigma.com.au
                int pipeLastPos = name.LastIndexOf("|");
                name = name.Substring(pipeLastPos + 1);
            }
            name = name.Trim('#'); //trims # from start or end
            return name;
        }
    }
}
