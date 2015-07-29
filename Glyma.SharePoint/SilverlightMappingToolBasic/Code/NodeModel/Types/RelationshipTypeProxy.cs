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
using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic
{
    public class RelationshipTypeProxy : IRelationshipTypeProxy
    {
        protected RelationshipTypeProxy()
        {
        }

        public  RelationshipTypeProxy(SoapRelationshipType soapRelationshipType)
        {
            BaseSoapRelationshipType = soapRelationshipType;
        }

        public SoapRelationshipType BaseSoapRelationshipType
        {
            get;
            protected set;
        }

        #region Static Members

        private static object _padlock = new object();
        private static Dictionary<Guid, RelationshipTypeProxy> _relationshipTypes = null;

        public static RelationshipTypeProxy GetRelationshipType(SoapRelationshipType soapRelationshipType)
        {
            lock (_padlock)
            {
                if (_relationshipTypes == null)
                {
                    _relationshipTypes = new Dictionary<Guid, RelationshipTypeProxy>();
                }

                RelationshipTypeProxy relationshipType;

                if (_relationshipTypes.ContainsKey(soapRelationshipType.Id))
                {
                    relationshipType = _relationshipTypes[soapRelationshipType.Id];
                }
                else
                {
                    relationshipType = new RelationshipTypeProxy(soapRelationshipType);

                    _relationshipTypes[soapRelationshipType.Id] = relationshipType;
                }

                return relationshipType;
            }
        }

        #endregion

        #region IRelationshipType Members

        public Color LineColor
        {
            get;
            set;
        }

        public Brush LineStyle
        {
            get;
            set;
        }

        public string LineDescription
        {
            get;
            set;
        }

        #endregion

        #region ITypeElement Members

        public Guid Id
        {
            get
            {
                return BaseSoapRelationshipType.Id;
            }
            set
            {
                BaseSoapRelationshipType.Id = value;
            }
        }

        public string Name
        {
            get
            {
                return BaseSoapRelationshipType.Name;
            }
            set
            {
                BaseSoapRelationshipType.Name = value;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Name: {0}", this.Name);
        }
    }
}
