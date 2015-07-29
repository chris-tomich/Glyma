namespace Glyma.UtilityService.Export.Common.Model
{
    public class NodeDescription
    {
        private string _description;
        private string _link;
        private string _type;

        public string Description
        {
            get
            {
                if (_description == null)
                {
                    _description = string.Empty;
                }
                return _description;
            }
            private set { _description = value; }
        }

        public string Link
        {
            get
            {
                if (_link == null)
                {
                    _link = string.Empty;
                }
                return _link;
            }
            private set { _link = value; }
        }

        public bool HasDescription { get; private set; }

        public string Type
        {
            get
            {
                if (_type == null)
                {
                    _type = "None";
                }
                return _type;
            }
            private set { _type = value; }
        }

        public NodeDescription(string descriptionType, string description, string url = null)
        {
            Type = descriptionType;
            Description = description;
            Link = url;
            if (Description.StartsWith("http://") || Description.StartsWith("https://"))
            {
                Type = "Iframe";
                Link = Description.Split(',')[0];
            }
            if (!string.IsNullOrEmpty(descriptionType) || !string.IsNullOrEmpty(description) || !string.IsNullOrEmpty(url))
            {
                HasDescription = true;
            }
        }
    }
}
