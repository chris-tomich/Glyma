using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Glyma.Security
{
    [ServiceContract(Namespace = "http://sevensigma.com.au/GlymaSecurityService")]
    public interface IGlymaSecurityService
    {
        [OperationContract]
        GetPermissionNameResponse GetUsersPermissionLevelName(string webUrl);

        [OperationContract]
        GetPermissionLevelResponse GetUsersPermissionLevel(string webUrl);

        [OperationContract]
        GetAllSecurityGroupsResponse GetAllSecurityGroups(string webUrl);

        [OperationContract]
        GetSecurableContextIdResponse GetSecurableContextId(string webUrl);

        [OperationContract]
        ResponseObject UpdateSecurityAssociations(string webUrl, IList<GlymaSecurityAssociation> securityAssocation);

        [OperationContract]
        GetSecurityAssociationsResponse GetSecurityAssociations(string webUrl, IEnumerable<GlymaSecurityGroup> groups, GlymaSecurableObject securbleObject);

        [OperationContract]
        GetPermissionNameResponse GetPermissionNameForObject(string webUrl, GlymaSecurableObject securbleObject);

        [OperationContract]
        GetPermissionLevelResponse GetPermissionLevelForObject(string webUrl, GlymaSecurableObject securbleObject);

        [OperationContract]
        ResponseObject SetProjectManagerGroupAssociations(string webUrl, GlymaSecurableObject securableObject);

        [OperationContract]
        ResponseObject BreakRootMapInheritance(string webUrl, GlymaSecurableObject securableObject);

        [OperationContract]
        GetSecurityAssociationsResponse RestoreRootMapInheritance(string webUrl, GlymaSecurableObject securableObject);
    }
}
