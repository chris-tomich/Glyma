using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Data.SqlClient;
using Glyma.Powershell.Base;
using Glyma.Powershell.Smo;
using System.Reflection;

namespace Glyma.Powershell.Update.v1_5_0_r1
{
    /// <summary>
    /// This class will update a pre-v1.4.7rev1 of the Glyma server to be v1.4.7rev1 compliant.
    /// </summary>
    internal class Update_v1_5_0_r1Base : IUpdateGLDatabase
    {
        private string _transactionDatabaseServer = null;

        private Guid DomainNodeType
        {
            get
            {
                return new Guid("263754C2-2F31-4D21-B9C4-6509E00A5E94");
            }
        }

        private string MapDatabaseConnectionString
        {
            get
            {
                return "Data Source=" + DatabaseServer + ";Initial Catalog=" + MapDatabaseName + ";Integrated Security=True";
            }
        }

        private string TransactionDatabaseConnectionString
        {
            get
            {
                return "Data Source=" + TransactionDatabaseServer + ";Initial Catalog=" + TransactionDatabaseName + ";Integrated Security=True";
            }
        }

        /// <summary>
        /// This is mandatory field. It will be used for both the map database and the transaction database (if none is separately declared).
        /// </summary>
        public string DatabaseServer
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the map database.
        /// </summary>
        public string MapDatabaseName
        {
            get;
            set;
        }

        /// <summary>
        /// If this field is set, it will be used as the database server for the transaction database. If it is not set, the DatabaseServer property will be used.
        /// </summary>
        public string TransactionDatabaseServer
        {
            get
            {
                if (_transactionDatabaseServer == null)
                {
                    return DatabaseServer;
                }

                return _transactionDatabaseServer;
            }
            set
            {
                _transactionDatabaseServer = value;
            }
        }

        /// <summary>
        /// The name of the transaction database.
        /// </summary>
        public string TransactionDatabaseName
        {
            get;
            set;
        }

        public void ExecuteCmdletBase(PSCmdlet callingCmdlet)
        {
            EmbeddedSqlScript createGetGlymaDbVersionScript = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r1.CreateGetGlymaDbVersion.sql");

            using (SqlConnection mapDbConnection = new SqlConnection(MapDatabaseConnectionString))
            {
                mapDbConnection.Open();

                try
                {
                    EmbeddedSqlScript addNewColumnsScript = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r1.MapDbAddNewColumns.sql");
                    addNewColumnsScript.ExecuteNonQuery(mapDbConnection);
                }
                catch
                {
                }

                try
                {
                    EmbeddedSqlScript addNewColumnContraints = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r1.MapDbAddNewColumnConstraints.sql");
                    addNewColumnContraints.ExecuteNonQuery(mapDbConnection);
                }
                catch
                {
                }

                try
                {
                    EmbeddedSqlScript modifyQueryMapSP = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r1.ModifyQueryMapSP.sql");
                    modifyQueryMapSP.ExecuteNonQuery(mapDbConnection);
                }
                catch
                {
                }

                try
                {
                    EmbeddedSqlScript createQueryMapSP = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r1.CreateQueryMapSP.sql");
                    createQueryMapSP.ExecuteNonQuery(mapDbConnection);
                }
                catch
                {
                }

                try
                {
                    EmbeddedSqlScript createNodesListTableType = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r1.CreateNodesListTableType.sql");
                    createNodesListTableType.ExecuteNonQuery(mapDbConnection);
                }
                catch
                {
                }

                try
                {
                    EmbeddedSqlScript createQueryNodesSP = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r1.CreateQueryNodesSP.sql");
                    createQueryNodesSP.ExecuteNonQuery(mapDbConnection);
                }
                catch
                {
                }

                try
                {
                    createGetGlymaDbVersionScript.ExecuteNonQuery(mapDbConnection);
                }
                catch
                {
                }

                mapDbConnection.Close();
            }

            using (SqlConnection transactionDbConnection = new SqlConnection(TransactionDatabaseConnectionString))
            {
                transactionDbConnection.Open();

                try
                {
                    EmbeddedSqlScript addNewColumnsScript = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r1.TransactionDbModifyColumns.sql");
                    addNewColumnsScript.ExecuteNonQuery(transactionDbConnection);
                }
                catch
                {
                }

                try
                {
                    createGetGlymaDbVersionScript.ExecuteNonQuery(transactionDbConnection);
                }
                catch
                {
                }

                transactionDbConnection.Close();
            }

            QueryMapResultConsumer domainNodesConsumer = new QueryMapResultConsumer();

            using (MappingToolDatabaseDataContext mapDatabaseContext = new MappingToolDatabaseDataContext(MapDatabaseConnectionString))
            {
                mapDatabaseContext.CommandTimeout = 600;
                var domainNodes = from node in mapDatabaseContext.Nodes
                                  where node.NodeTypeUid.HasValue && node.NodeTypeUid.Value == DomainNodeType
                                  select node;

                if (domainNodes != null && domainNodes.Count() > 0)
                {
                    foreach (var domainNode in domainNodes)
                    {
                        var domainMaps = mapDatabaseContext.QueryMapMultiDepth(domainNode.DomainUid, domainNode.NodeUid, 1, true);

                        domainNodesConsumer.Consume(domainMaps);
                    }
                }

                var rootMaps = from qNode in domainNodesConsumer.Nodes.Values
                               select qNode.NodeUid;

                DateTime startTime = new DateTime(2014, 04, 01, 0, 0, 0);

                foreach (var map in domainNodesConsumer.Nodes.Values)
                {
                    if (map.NodeTypeUid.HasValue && map.NodeTypeUid != DomainNodeType)
                    {
                        QueryMapResultConsumer mapNodesConsumer = new QueryMapResultConsumer();
                        DbIntegrityUtilities integrityUtilities = new DbIntegrityUtilities(mapNodesConsumer);

                        var mapNodes = mapDatabaseContext.QueryMapMultiDepth(map.DomainUid, map.NodeUid, -1, false);

                        callingCmdlet.WriteVerbose("Processing nodes");
                        mapNodesConsumer.Consume(mapNodes);
                        integrityUtilities.DetectAndFixMapCollisions(callingCmdlet, mapDatabaseContext, rootMaps, false, false);
                        callingCmdlet.WriteVerbose("Finished processing nodes");

                        callingCmdlet.WriteVerbose("Processing nodes and related metadata");
                        foreach (var node in mapNodesConsumer.Nodes.Values)
                        {
                            var dbNodes = from qNode in mapDatabaseContext.Nodes
                                          where qNode.NodeUid == node.NodeUid
                                          select qNode;

                            if (dbNodes != null && dbNodes.Count() > 0)
                            {
                                var dbNode = dbNodes.First();

                                dbNode.Created = startTime;
                                dbNode.Modified = startTime;
                                dbNode.RootMapUid = map.NodeUid;

                                foreach (var dbMetadata in dbNode.Metadatas)
                                {
                                    dbMetadata.Created = startTime;
                                    dbMetadata.Modified = startTime;

                                    if (dbMetadata.DomainUid == null)
                                    {
                                        dbMetadata.DomainUid = map.DomainUid;
                                    }

                                    if (dbMetadata.RootMapUid == null)
                                    {
                                        dbMetadata.RootMapUid = map.NodeUid;
                                    }
                                }

                                mapDatabaseContext.SubmitChanges();
                            }

                            callingCmdlet.WriteVerbose("Finished node " + node.NodeUid);
                        }
                        callingCmdlet.WriteVerbose("Finished processing nodes");

                        callingCmdlet.WriteVerbose("Processing relationships and related metadata");
                        foreach (var relationship in mapNodesConsumer.Relationships.Values)
                        {
                            var dbRelationships = from qRelationship in mapDatabaseContext.Relationships
                                                  where qRelationship.RelationshipUid == relationship.RelationshipUid
                                                  select qRelationship;

                            if (dbRelationships != null && dbRelationships.Count() > 0)
                            {
                                var dbRelationship = dbRelationships.First();

                                dbRelationship.Created = startTime;
                                dbRelationship.Modified = startTime;
                                dbRelationship.RootMapUid = map.NodeUid;

                                foreach (var dbMetadata in dbRelationship.Metadatas)
                                {
                                    dbMetadata.Created = startTime;
                                    dbMetadata.Modified = startTime;

                                    if (dbMetadata.DomainUid == null)
                                    {
                                        dbMetadata.DomainUid = map.DomainUid;
                                    }

                                    if (dbMetadata.RootMapUid == null)
                                    {
                                        dbMetadata.RootMapUid = map.NodeUid;
                                    }
                                }

                                mapDatabaseContext.SubmitChanges();
                            }

                            callingCmdlet.WriteVerbose("Finished relationship " + relationship.RelationshipUid);
                        }
                        callingCmdlet.WriteVerbose("Finished processing relationships");
                    }
                }
            }
        }
    }
}
