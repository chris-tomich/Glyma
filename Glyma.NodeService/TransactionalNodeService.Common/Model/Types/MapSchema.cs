using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TransactionalNodeService.Common.Model
{
    public class MapSchema
    {
        protected const string SelectTypes = @"SELECT * FROM NodeTypes;
                                                SELECT * FROM RelationshipTypes;
                                                SELECT * FROM DescriptorTypes;
                                                SELECT * FROM MetadataTypes;";

        public MapSchema()
        {
        }

        public MapSchema(IDbConnectionAbstraction mapDbConnection)
        {
            MapDbConnection = mapDbConnection;

            LoadTypesFromDb();
        }

        protected IDbConnectionAbstraction MapDbConnection
        {
            get;
            set;
        }

        public TypeResponse LoadTypesFromDb()
        {
            TypeResponse types = new TypeResponse();
            SqlCommand selectTypes = new SqlCommand(SelectTypes, MapDbConnection.Connection);

            MapDbConnection.Open();

            SqlDataReader typesReader = selectTypes.ExecuteReader();

            int count = 0;

            do
            {
                count++;

                while (typesReader.Read())
                {
                    switch (count)
                    {
                        case 1:
                            NodeType nodeType = new NodeType();
                            nodeType.LoadSessionObject(typesReader);

                            types.NodeTypes[nodeType.Name] = nodeType;
                            break;
                        case 2:
                            RelationshipType relationshipType = new RelationshipType();
                            relationshipType.LoadSessionObject(typesReader);

                            types.RelationshipTypes[relationshipType.Name] = relationshipType;
                            break;
                        case 3:
                            DescriptorType descriptorType = new DescriptorType();
                            descriptorType.LoadSessionObject(typesReader);

                            types.DescriptorTypes[descriptorType.Name] = descriptorType;
                            break;
                        case 4:
                            MetadataType metadataType = new MetadataType();
                            metadataType.LoadSessionObject(typesReader);

                            types.MetadataTypes[metadataType.Name] = metadataType;
                            break;
                        default:
                            break;
                    }
                }
            }
            while (typesReader.NextResult());

            MapDbConnection.Close();

            return types;
        }
    }
}