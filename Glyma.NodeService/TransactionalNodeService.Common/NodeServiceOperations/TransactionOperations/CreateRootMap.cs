using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using TransactionalNodeService.Common.Model;
using System.Security.Principal;

namespace TransactionalNodeService.Common.TransactionOperations
{
    public class CreateRootMap
    {
        private const string CreateRootMapNode = @"INSERT INTO [Nodes] ([NodeUid], [NodeOriginalId], [NodeTypeUid], [DomainUid], [RootMapUid], [Created], [Modified], [CreatedBy], [ModifiedBy])
                                                                        VALUES (@NodeUid, @NodeOriginalId, @NodeTypeUid, @DomainUid, @RootMapUid, @Created, @Modified, @CreatedBy, @ModifiedBy)";

        private const string CreateRootMapNameMetadata = @"INSERT INTO [Metadata] ([MetadataId], [MetadataTypeUid], [NodeUid], [RelationshipUid], [DescriptorTypeUid], [MetadataName], [MetadataValue], [RootMapUid], [DomainUid], [Created], [Modified], [CreatedBy], [ModifiedBy])
                                                                                VALUES (@MetadataId, @MetadataTypeUid, @NodeUid, @RelationshipUid, @DescriptorTypeUid, @MetadataName, @MetadataValue, @RootMapUid, @DomainUid, @Created, @Modified, @CreatedBy, @ModifiedBy)";

        private const string CreateRootMapRelationship = @"INSERT INTO [Relationships] ([RelationshipUid], [RelationshipOriginalId], [RelationshipTypeUid], [DomainUid], [RootMapUid], [Created], [Modified], [CreatedBy], [ModifiedBy])
                                                                                    VALUES (@RelationshipUid, @RelationshipOriginalId, @RelationshipTypeUid, @DomainUid, @RootMapUid, @Created, @Modified, @CreatedBy, @ModifiedBy)";

        private const string CreateRootMapDescriptors = @"INSERT INTO [Descriptors] ([DescriptorUid], [DescriptorTypeUid], [NodeUid], [RelationshipUid])
                                                                                VALUES (@DescriptorUid, @DescriptorTypeUid, @NodeUid, @RelationshipUid)";

        private const string GetDomainNodeId = @"SELECT [NodeUid] FROM [Nodes] WHERE [NodeTypeUid] = @NodeTypeUid AND [DomainUid] = @DomainUid";

        protected string _user = null;

        public CreateRootMap(IDbConnectionAbstraction mapDbConnection, IDbConnectionAbstraction parametersDbConnection, IDbConnectionAbstraction sessionDbConnection)
        {
            MapDbConnection = mapDbConnection;
            ParametersDbConnection = parametersDbConnection;
            SessionDbConnection = sessionDbConnection;
        }

        private IDbConnectionAbstraction MapDbConnection
        {
            get;
            set;
        }

        private IDbConnectionAbstraction ParametersDbConnection
        {
            get;
            set;
        }

        private IDbConnectionAbstraction SessionDbConnection
        {
            get;
            set;
        }

        private string User
        {
            get
            {
                if (_user == null)
                {
                    _user = "anonymous";

                    WindowsIdentity currentUserIdentity = WindowsIdentity.GetCurrent();

                    if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                    {
                        /// This will capture for claims authentication.
                        _user = HttpContext.Current.User.Identity.Name;
                    }
                    else if (currentUserIdentity != null && currentUserIdentity.User != null && !string.IsNullOrEmpty(currentUserIdentity.User.Value))
                    {
                        _user = currentUserIdentity.User.Value + ";" + currentUserIdentity.Name;
                    }
                }

                return _user;
            }
        }

        public Guid Create(Guid domainId, string name, NodeType nodeType, string originalId)
        {
            SqlCommand getDomainNodeId = new SqlCommand(GetDomainNodeId, MapDbConnection.Connection);
            getDomainNodeId.Parameters.Add(new SqlParameter("@NodeTypeUid", new Guid("263754C2-2F31-4D21-B9C4-6509E00A5E94")));
            getDomainNodeId.Parameters.Add(new SqlParameter("@DomainUid", domainId));

            Guid? domainNodeId = (Guid?)getDomainNodeId.ExecuteScalar();

            if (domainNodeId == null)
            {
                return Guid.Empty;
            }

            Guid nodeId = Guid.NewGuid();
            DateTime currentTime = DateTime.Now.ToUniversalTime();

            SqlCommand createRootMapCommand = new SqlCommand(CreateRootMapNode, MapDbConnection.Connection);
            createRootMapCommand.Parameters.Add(new SqlParameter("@NodeUid", nodeId));

            if (string.IsNullOrEmpty(originalId))
            {
                createRootMapCommand.Parameters.Add(new SqlParameter("@NodeOriginalId", DBNull.Value));
            }
            else
            {
                createRootMapCommand.Parameters.Add(new SqlParameter("@NodeOriginalId", originalId));
            }

            createRootMapCommand.Parameters.Add(new SqlParameter("@NodeTypeUid", nodeType.Id));
            createRootMapCommand.Parameters.Add(new SqlParameter("@DomainUid", domainId));
            createRootMapCommand.Parameters.Add(new SqlParameter("@RootMapUid", nodeId));
            createRootMapCommand.Parameters.Add(new SqlParameter("@Created", currentTime));
            createRootMapCommand.Parameters.Add(new SqlParameter("@Modified", currentTime));
            createRootMapCommand.Parameters.Add(new SqlParameter("@CreatedBy", User));
            createRootMapCommand.Parameters.Add(new SqlParameter("@ModifiedBy", User));

            createRootMapCommand.ExecuteNonQuery();

            Guid metadataId = Guid.NewGuid();

            SqlCommand metadataRootMapCommand = new SqlCommand(CreateRootMapNameMetadata, MapDbConnection.Connection);
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@MetadataId", metadataId));
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@MetadataTypeUid", new Guid("C7628C1E-77C1-4A07-A2E8-8DE9F4E9803C")));
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@NodeUid", nodeId));
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@RelationshipUid", DBNull.Value));
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@DescriptorTypeUid", DBNull.Value));
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@MetadataName", "Name"));

            if (string.IsNullOrEmpty(name))
            {
                metadataRootMapCommand.Parameters.Add(new SqlParameter("@MetadataValue", DBNull.Value));
            }
            else
            {
                metadataRootMapCommand.Parameters.Add(new SqlParameter("@MetadataValue", name));
            }

            metadataRootMapCommand.Parameters.Add(new SqlParameter("@RootMapUid", nodeId));
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@DomainUid", domainId));
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@Created", currentTime));
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@Modified", currentTime));
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@CreatedBy", User));
            metadataRootMapCommand.Parameters.Add(new SqlParameter("@ModifiedBy", User));

            metadataRootMapCommand.ExecuteNonQuery();

            Guid relationshipId = Guid.NewGuid();

            SqlCommand relationshipRootMapCommand = new SqlCommand(CreateRootMapRelationship, MapDbConnection.Connection);
            relationshipRootMapCommand.Parameters.Add(new SqlParameter("@RelationshipUid", relationshipId));
            relationshipRootMapCommand.Parameters.Add(new SqlParameter("@RelationshipOriginalId", ""));
            relationshipRootMapCommand.Parameters.Add(new SqlParameter(@"RelationshipTypeUid", new Guid("4AFF46D7-87BE-48DD-B703-A93E38EF8FFB")));
            relationshipRootMapCommand.Parameters.Add(new SqlParameter(@"DomainUid", domainId));
            relationshipRootMapCommand.Parameters.Add(new SqlParameter(@"RootMapUid", nodeId));
            relationshipRootMapCommand.Parameters.Add(new SqlParameter(@"Created", currentTime));
            relationshipRootMapCommand.Parameters.Add(new SqlParameter(@"Modified", currentTime));
            relationshipRootMapCommand.Parameters.Add(new SqlParameter(@"CreatedBy", User));
            relationshipRootMapCommand.Parameters.Add(new SqlParameter(@"ModifiedBy", User));

            relationshipRootMapCommand.ExecuteNonQuery();

            Guid fromDescriptorId = Guid.NewGuid();

            SqlCommand fromDescriptorRootMapCommand = new SqlCommand(CreateRootMapDescriptors, MapDbConnection.Connection);
            fromDescriptorRootMapCommand.Parameters.Add(new SqlParameter("@DescriptorUid", fromDescriptorId));
            fromDescriptorRootMapCommand.Parameters.Add(new SqlParameter("@DescriptorTypeUid", new Guid("96DA1782-058C-4F9B-BB1A-31B048F8C75A")));
            fromDescriptorRootMapCommand.Parameters.Add(new SqlParameter("@NodeUid", nodeId));
            fromDescriptorRootMapCommand.Parameters.Add(new SqlParameter("@RelationshipUid", relationshipId));

            fromDescriptorRootMapCommand.ExecuteNonQuery();

            Guid toDescriptorId = Guid.NewGuid();

            SqlCommand toDescriptorRootMapCommand = new SqlCommand(CreateRootMapDescriptors, MapDbConnection.Connection);
            toDescriptorRootMapCommand.Parameters.Add(new SqlParameter("@DescriptorUid", toDescriptorId));
            toDescriptorRootMapCommand.Parameters.Add(new SqlParameter("@DescriptorTypeUid", new Guid("07C91D35-4DAC-431B-966B-64C924B8CDAB")));
            toDescriptorRootMapCommand.Parameters.Add(new SqlParameter("@NodeUid", domainNodeId.Value));
            toDescriptorRootMapCommand.Parameters.Add(new SqlParameter("@RelationshipUid", relationshipId));

            toDescriptorRootMapCommand.ExecuteNonQuery();

            Guid sessionId = Guid.NewGuid();

            MapTransaction beginTransaction = new MapTransaction();
            beginTransaction.SessionUid = sessionId;
            beginTransaction.TransactionTimestamp = DateTime.Now.ToUniversalTime();
            beginTransaction.User = User;
            beginTransaction.OperationId = TransactionType.BeginSession;
            beginTransaction.PersistSessionObject(SessionDbConnection);

            MapParameter domainParameter = new MapParameter();
            domainParameter.SessionId = sessionId;
            domainParameter.IsDelayed = false;
            domainParameter.ParameterType = MapParameterType.Domain;
            domainParameter.Value = domainId;
            domainParameter.PersistSessionObject(ParametersDbConnection);

            MapParameter rootMapParameter = new MapParameter();
            rootMapParameter.SessionId = sessionId;
            rootMapParameter.IsDelayed = false;
            rootMapParameter.ParameterType = MapParameterType.RootMap;
            rootMapParameter.Value = nodeId;
            rootMapParameter.PersistSessionObject(ParametersDbConnection);

            MapTransaction createRootMapTransaction = new MapTransaction();
            createRootMapTransaction.SessionUid = sessionId;
            createRootMapTransaction.TransactionTimestamp = DateTime.Now.ToUniversalTime();
            createRootMapTransaction.User = User;
            createRootMapTransaction.OperationId = TransactionType.CreateRootMap;
            createRootMapTransaction.DomainParameterUid = domainParameter.Id;
            createRootMapTransaction.RootMapParameterUid = rootMapParameter.Id;
            createRootMapTransaction.NodeParameterUid = rootMapParameter.Id;
            createRootMapTransaction.NodeTypeUid = nodeType.Id;
            createRootMapTransaction.PersistSessionObject(SessionDbConnection);

            MapTransaction endTransaction = new MapTransaction();
            endTransaction.SessionUid = sessionId;
            endTransaction.TransactionTimestamp = DateTime.Now.ToUniversalTime();
            endTransaction.User = User;
            endTransaction.OperationId = TransactionType.CompleteSession;
            endTransaction.PersistSessionObject(SessionDbConnection);

            return nodeId;
        }
    }
}