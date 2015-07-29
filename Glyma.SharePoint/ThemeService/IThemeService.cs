using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ThemeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IThemeService
    {
        [OperationContract]
        ThemeResult GetTheme(string name);

        [OperationContract]
        string GetContextMenuXaml(string name);
    }

    [DataContract]
    public class ThemeResult
    {
        bool successValue = false;
        Theme themeValue = null;

        [DataMember]
        public bool Success
        {
            get { return successValue; }
            set { successValue = value; }
        }

        [DataMember]
        public Theme Theme
        {
            get { return themeValue; }
            set { themeValue = value; }
        }
    }

}
