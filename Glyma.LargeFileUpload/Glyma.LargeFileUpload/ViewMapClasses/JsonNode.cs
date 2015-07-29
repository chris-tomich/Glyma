using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Glyma.HttpHandlers.ViewMapClasses
{
    public class JsonNode : Hashtable
    {
        public JsonNode()
        {
        }

        public JsonNode(QueryMapNode node)
        {
            RootMapId = node.RootMapUid.Value;
            UniqueId = node.NodeUid;
            Name = node.FindSingleMetadata("Name").MetadataValue.Replace('"', '\'');

            QueryMapMetadata xPositionMetadata = node.FindSingleMetadataDefaultView("XPosition");

            if (xPositionMetadata != null && !string.IsNullOrEmpty(xPositionMetadata.MetadataValue))
            {
                double xPosition;

                if (double.TryParse(xPositionMetadata.MetadataValue, out xPosition))
                {
                    int truncatedXPosition = (int)xPosition;

                    XPosition = truncatedXPosition;
                }
                else
                {
                    IsProperlyStructured = false;
                    return;
                }
            }

            QueryMapMetadata yPositionMetadata = node.FindSingleMetadataDefaultView("YPosition");

            if (yPositionMetadata != null && !string.IsNullOrEmpty(yPositionMetadata.MetadataValue))
            {
                double yPosition;

                if (double.TryParse(yPositionMetadata.MetadataValue, out yPosition))
                {
                    int truncatedYPosition = (int)yPosition;

                    YPosition = truncatedYPosition;
                }
                else
                {
                    IsProperlyStructured = false;
                    return;
                }
            }

            NodeType = node.NodeType;

            foreach (QueryMapMetadata metadata in node.FindAllMetadata())
            {
                if (metadata.MetadataName != "Name" && metadata.MetadataName != "XPosition" && metadata.MetadataName != "YPosition")
                {
                    if (metadata.MetadataName.Contains("."))
                    {
                        string[] referenceNames = metadata.MetadataName.Split(new char[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                        Hashtable nestedReferenceObject = this;

                        for (int i = 0; i < (referenceNames.Length - 1); i++)
                        {
                            string referenceName = referenceNames[i];

                            if (nestedReferenceObject.ContainsKey(referenceName))
                            {
                                if (nestedReferenceObject[referenceName] is Hashtable)
                                {
                                    nestedReferenceObject = (Hashtable)nestedReferenceObject[referenceName];
                                }
                                else
                                {
                                    Hashtable nextNestedReferenceObject = new Hashtable();

                                    nextNestedReferenceObject["this"] = nestedReferenceObject[referenceName];
                                    nestedReferenceObject[referenceName] = nextNestedReferenceObject;
                                    nestedReferenceObject = nextNestedReferenceObject;
                                }
                            }
                            else
                            {
                                Hashtable nextNestedReferenceObject = new Hashtable();

                                nestedReferenceObject[referenceName] = nextNestedReferenceObject;
                                nestedReferenceObject = nextNestedReferenceObject;
                            }
                        }

                        string lastReferenceName = referenceNames[referenceNames.Length - 1];
                        nestedReferenceObject[lastReferenceName] = metadata.MetadataValue;
                    }
                    else
                    {
                        this[metadata.MetadataName] = metadata.MetadataValue;
                    }
                }
            }

            IsProperlyStructured = true;
        }

        [ScriptIgnore]
        public bool IsProperlyStructured
        {
            get;
            set;
        }

        [ScriptIgnore]
        public Guid RootMapId
        {
            get;
            set;
        }

        public Guid UniqueId
        {
            get
            {
                return (Guid)this["uniqueId"];
            }
            set
            {
                this["uniqueId"] = value;
            }
        }

        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        public int XPosition
        {
            get
            {
                return (int)this["xPosition"];
            }
            set
            {
                this["xPosition"] = value;
            }
        }

        public int YPosition
        {
            get
            {
                return (int)this["yPosition"];
            }
            set
            {
                this["yPosition"] = value;
            }
        }

        public string NodeType
        {
            get
            {
                return (string)this["nodeType"];
            }
            set
            {
                this["nodeType"] = value;
            }
        }
    }
}
