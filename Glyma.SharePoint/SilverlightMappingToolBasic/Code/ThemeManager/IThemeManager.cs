using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverlightMappingToolBasic
{
    public interface IThemeManager
    {
        event EventHandler ThemeLoaded;
        void LoadTheme(string themeSvcUrl, string themeName);
        INodeSkin GetSkin(INodeTypeProxy nodeTypeProxy, string style);
    }
}
