using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverlightMappingToolBasic.UI.ViewModel;
using Proxy = TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.ViewModel
{
    public interface IViewModelMetadataFactory
    {
        /// TODO: May need to add methods for adding nodes and relationships. For now just need the ability to create metadata.
        /// 

        IMetadata CreateMetadata(Proxy.IMetadataSet newMetadataSet);
    }
}
