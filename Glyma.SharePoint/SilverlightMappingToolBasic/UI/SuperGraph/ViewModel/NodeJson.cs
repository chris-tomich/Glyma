using System;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class NodeJson : Dictionary<string, object>
    {
        public Guid DomainId
        {
            get
            {
                return (Guid)this["DomainId"];
            }
            set
            {
                this["DomainId"] = value;
            }
        }

        public Guid Id
        {
            get
            {
                return (Guid)this["Id"];
            }
            set
            {
                this["Id"] = value;
            }
        }

        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
            set
            {
                this["Name"] = value;
            }
        }

        public int XPosition
        {
            get
            {
                return (int)(this["XPosition"]);
            }
            set
            {
                this["XPosition"] = value;
            }
        }

        public int YPosition
        {
            get
            {
                return (int)(this["YPosition"]);
            }
            set
            {
                this["YPosition"] = value;
            }
        }

        public string NodeType
        {
            get
            {
                return (string)this["NodeType"];
            }
            set
            {
                this["NodeType"] = value;
            }
        }

        public string Description
        {
            get
            {
                return (string)this["Description"];
            }
            set
            {
                this["Description"] = value;
            }
        }


        public NodeJson(IEnumerable<KeyValuePair<string, string>> properties, Guid id, Guid domainId)
        {
            Id = id;
            DomainId = domainId;
            foreach (var metadatapair in properties)
            {
                if (metadatapair.Key.Contains("."))
                {
                    string[] referenceNames = metadatapair.Key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<string, object> nestedReferenceObject = this;

                    for (int i = 0; i < (referenceNames.Length - 1); i++)
                    {
                        string referenceName = referenceNames[i];

                        if (nestedReferenceObject.ContainsKey(referenceName))
                        {
                            if (nestedReferenceObject[referenceName] is Dictionary<string, object>)
                            {
                                nestedReferenceObject = (Dictionary<string, object>)nestedReferenceObject[referenceName];
                            }
                            else
                            {
                                var nextNestedReferenceObject = new Dictionary<string, object>();

                                nextNestedReferenceObject["this"] = nestedReferenceObject[referenceName];
                                nestedReferenceObject[referenceName] = nextNestedReferenceObject;
                                nestedReferenceObject = nextNestedReferenceObject;
                            }
                        }
                        else
                        {
                            var nextNestedReferenceObject = new Dictionary<string, object>();

                            nestedReferenceObject[referenceName] = nextNestedReferenceObject;
                            nestedReferenceObject = nextNestedReferenceObject;
                        }
                    }

                    var lastReferenceName = referenceNames[referenceNames.Length - 1];
                    nestedReferenceObject[lastReferenceName] = metadatapair.Value;
                }
                else
                {
                    this[metadatapair.Key] = metadatapair.Value;
                }
            }
        }

        private void InitialiseMetadata(IEnumerable<KeyValuePair<string, IMetadata>> metadataCollection)
        {
            foreach (var metadatapair in metadataCollection)
            {
                if (metadatapair.Key.Contains("."))
                {
                    string[] referenceNames = metadatapair.Key.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<string, object> nestedReferenceObject = this;

                    for (int i = 0; i < (referenceNames.Length - 1); i++)
                    {
                        string referenceName = referenceNames[i];

                        if (nestedReferenceObject.ContainsKey(referenceName))
                        {
                            if (nestedReferenceObject[referenceName] is Dictionary<string, object>)
                            {
                                nestedReferenceObject = (Dictionary<string, object>)nestedReferenceObject[referenceName];
                            }
                            else
                            {
                                var nextNestedReferenceObject = new Dictionary<string, object>();

                                nextNestedReferenceObject["this"] = nestedReferenceObject[referenceName];
                                nestedReferenceObject[referenceName] = nextNestedReferenceObject;
                                nestedReferenceObject = nextNestedReferenceObject;
                            }
                        }
                        else
                        {
                            var nextNestedReferenceObject = new Dictionary<string, object>();

                            nestedReferenceObject[referenceName] = nextNestedReferenceObject;
                            nestedReferenceObject = nextNestedReferenceObject;
                        }
                    }

                    var lastReferenceName = referenceNames[referenceNames.Length - 1];
                    nestedReferenceObject[lastReferenceName] = metadatapair.Value.Value;
                }
                else
                {
                    this[metadatapair.Key] = metadatapair.Value.Value;
                }

                //this[metadatapair.Key] = metadatapair.Value.Value;
            }
        }

        public NodeJson(INode node)
        {
            Id = node.Proxy.Id;
            DomainId = node.Proxy.DomainId;

            InitialiseMetadata(node.Metadata);
        }





        public NodeJson(Node node)
        {
            Id = node.Proxy.Id;
            DomainId = node.Proxy.DomainId;
            Name = node.Name;
            Description = node.Description;
            XPosition = (int)(node.Location.X);
            YPosition = (int)(node.Location.Y);
            NodeType = node.NodeType;
            InitialiseMetadata(node.Metadata);
        }
    }
}
