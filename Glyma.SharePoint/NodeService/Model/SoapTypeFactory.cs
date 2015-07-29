using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NodeService
{
    public class SoapTypeFactory
    {
        private static object _lock = null;
        private static SoapTypeFactory _singleton = null;
        private static Dictionary<Guid, ISoapTypeElement> _soapTypes = null; 

        static SoapTypeFactory()
        {
            _lock = new object();
            _singleton = new SoapTypeFactory();
            _soapTypes = new Dictionary<Guid, ISoapTypeElement>();
        }

        public static ISoapTypeElement GetSoapType<SoapType>(Guid typeUid) where SoapType : ISoapTypeElement
        {
            lock (_lock)
            {
                if (_soapTypes.ContainsKey(typeUid))
                {
                    return _soapTypes[typeUid];
                }

                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    if (typeof(SoapType).IsAssignableFrom(typeof(SoapNodeType)))
                    {
                        var nodeType = (from dbNodeType in mappingDb.NodeTypes where dbNodeType.NodeTypeUid == typeUid select dbNodeType).First();

                        SoapNodeType soapNodeType = new SoapNodeType();
                        soapNodeType.Id = nodeType.NodeTypeUid;
                        soapNodeType.Name = nodeType.NodeTypeName;

                        return (ISoapTypeElement)soapNodeType;
                    }
                    else if (typeof(SoapType).IsAssignableFrom(typeof(SoapDescriptorType)))
                    {
                        var nodeType = (from dbNodeType in mappingDb.DescriptorTypes where dbNodeType.DescriptorTypeUid == typeUid select dbNodeType).First();

                        SoapDescriptorType soapDescriptorType = new SoapDescriptorType();
                        soapDescriptorType.Id = nodeType.DescriptorTypeUid;
                        soapDescriptorType.Name = nodeType.DescriptorTypeName;

                        return (ISoapTypeElement)soapDescriptorType;
                    }
                    else if (typeof(SoapType).IsAssignableFrom(typeof(SoapRelationshipType)))
                    {
                        var nodeType = (from dbNodeType in mappingDb.RelationshipTypes where dbNodeType.RelationshipTypeUid == typeUid select dbNodeType).First();

                        SoapRelationshipType soapRelationshipType = new SoapRelationshipType();
                        soapRelationshipType.Id = nodeType.RelationshipTypeUid;
                        soapRelationshipType.Name = nodeType.RelationshipTypeName;

                        return (ISoapTypeElement)soapRelationshipType;
                    }
                    else if (typeof(SoapType).IsAssignableFrom(typeof(SoapMetadataType)))
                    {
                        var nodeType = (from dbNodeType in mappingDb.MetadataTypes where dbNodeType.MetadataTypeUid == typeUid select dbNodeType).First();

                        SoapMetadataType soapMetadataType = new SoapMetadataType();
                        soapMetadataType.Id = nodeType.MetadataTypeUid;
                        soapMetadataType.Name = nodeType.MetadataTypeName;

                        return (ISoapTypeElement)soapMetadataType;
                    }
                    else
                    {
                        throw new NotSupportedException("The requested type is not supported");
                    }
                }
            }
        }
    }
}