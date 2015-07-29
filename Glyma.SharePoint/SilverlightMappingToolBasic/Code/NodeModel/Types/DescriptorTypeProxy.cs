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
    public class DescriptorTypeProxy : IDescriptorTypeProxy
    {
        protected DescriptorTypeProxy()
        {
        }

        public DescriptorTypeProxy(SoapDescriptorType soapDescriptorType)
        {
            BaseSoapDescriptorType = soapDescriptorType;
        }

        public SoapDescriptorType BaseSoapDescriptorType
        {
            get;
            protected set;
        }

        #region Static Members

        private static object _padlock = new object();
        private static Dictionary<Guid, DescriptorTypeProxy> _descriptorTypes = null;

        public static DescriptorTypeProxy GetDescriptorType(SoapDescriptorType soapDescriptorType)
        {
            lock (_padlock)
            {
                if (_descriptorTypes == null)
                {
                    _descriptorTypes = new Dictionary<Guid, DescriptorTypeProxy>();
                }

                DescriptorTypeProxy descriptorType;

                if (_descriptorTypes.ContainsKey(soapDescriptorType.Id))
                {
                    descriptorType = _descriptorTypes[soapDescriptorType.Id];
                }
                else
                {
                    descriptorType = new DescriptorTypeProxy(soapDescriptorType);

                    _descriptorTypes[soapDescriptorType.Id] = descriptorType;
                }

                return descriptorType;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region IDescriptorType Members

        public Brush DescriptorImage
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
                return BaseSoapDescriptorType.Id;
            }
            set
            {
                BaseSoapDescriptorType.Id = value;
            }
        }

        public string Name
        {
            get
            {
                return BaseSoapDescriptorType.Name;
            }
            set
            {
                BaseSoapDescriptorType.Name = value;
            }
        }

        #endregion
    }
}
