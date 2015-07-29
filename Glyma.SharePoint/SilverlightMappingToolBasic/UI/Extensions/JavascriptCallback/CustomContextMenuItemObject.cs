namespace SilverlightMappingToolBasic.UI.Extensions.JavascriptCallback
{
    public class CustomContextMenuItemObject
    {
        public string Name { get; private set; }

        public string JavascriptMethod { get; private set; }

        public CustomContextMenuItemObject(string name, string javascriptMethod)
        {
            Name = name;
            JavascriptMethod = javascriptMethod;
        }
    }
}
