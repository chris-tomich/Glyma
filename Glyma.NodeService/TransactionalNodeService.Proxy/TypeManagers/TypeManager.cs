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
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy
{
    public abstract class TypeManager<ServiceType, TypeManagerType> : ITypeManager<ServiceType, TypeManagerType> where TypeManagerType : IMapType
    {
        private Dictionary<Guid, TypeManagerType> _typeByGuid = null;
        private Dictionary<string, TypeManagerType> _typeByName = null;
        private Dictionary<Guid, ServiceType> _serviceTypeById = null;

        protected Dictionary<Guid, TypeManagerType> TypeByGuid
        {
            get
            {
                if (_typeByGuid == null)
                {
                    _typeByGuid = new Dictionary<Guid, TypeManagerType>();
                }

                return _typeByGuid;
            }
        }

        protected Dictionary<string, TypeManagerType> TypeByName
        {
            get
            {
                if (_typeByName == null)
                {
                    _typeByName = new Dictionary<string, TypeManagerType>();
                }

                return _typeByName;
            }
        }

        protected Dictionary<Guid, ServiceType> ServiceTypeById
        {
            get
            {
                if (_serviceTypeById == null)
                {
                    _serviceTypeById = new Dictionary<Guid, ServiceType>();
                }

                return _serviceTypeById;
            }
        }

        public TypeManagerType this[Guid typeId]
        {
            get
            {
                return TypeByGuid[typeId];
            }
        }

        public TypeManagerType this[string typeName]
        {
            get
            {
                if (typeName == null)
                {
                    return default(TypeManagerType);
                }

                return TypeByName[typeName];
            }
        }


        public void Add(ServiceType serviceType, TypeManagerType mapType)
        {
            TypeByGuid[mapType.Id] = mapType;
            TypeByName[mapType.Name] = mapType;
            ServiceTypeById[mapType.Id] = serviceType;
        }

        public ServiceType ConvertProxyToService(TypeManagerType mapType)
        {
            if (mapType == null)
            {
                return default(ServiceType);
            }

            return ServiceTypeById[mapType.Id];
        }

        public bool ContainsKey(Guid typeId)
        {
            return TypeByGuid.ContainsKey(typeId);
        }

        public bool ContainsKey(string typeName)
        {
            return TypeByName.ContainsKey(typeName);
        }
    }
}
