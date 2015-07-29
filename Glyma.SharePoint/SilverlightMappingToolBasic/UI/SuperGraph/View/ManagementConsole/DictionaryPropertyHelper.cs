using System.Collections.Generic;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole
{
    public static class DictionaryPropertyHelper
    {
        public static string GetPropertyStringValue(this Dictionary<string, string> dic, string key)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            return string.Empty;
        }

        public static bool GetPropertyBooleanValue(this Dictionary<string, string> dic, string key)
        {
            if (dic.ContainsKey(key))
            {
                var value= dic[key];
                if (value.ToLower() == "true")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
