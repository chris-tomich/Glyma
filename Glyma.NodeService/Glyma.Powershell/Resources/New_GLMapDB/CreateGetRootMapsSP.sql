CREATE PROCEDURE dbo.GetRootMaps 
	@DomainId uniqueidentifier = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @MapContainerRelationship uniqueidentifier
	DECLARE @ToDescriptor uniqueidentifier
	DECLARE @FromDescriptor uniqueidentifier
	DECLARE @DomainNodeType uniqueidentifier
	DECLARE @MapNodeType uniqueidentifier
	
	SET @MapContainerRelationship = '4AFF46D7-87BE-48DD-B703-A93E38EF8FFB'
	SET @ToDescriptor = '07C91D35-4DAC-431B-966B-64C924B8CDAB'
	SET @FromDescriptor = '96DA1782-058C-4F9B-BB1A-31B048F8C75A'
	SET @DomainNodeType = '263754C2-2F31-4D21-B9C4-6509E00A5E94'
	SET @MapNodeType = 'B8C354CB-C7D0-4982-9A0F-6C4368FAB749'
		
   IF @DomainId IS NULL
   BEGIN
      -- Get all root map nodes for all domains.
      SELECT ChildNodes.DomainUid AS DomainId, ParentMetadata.MetadataValue AS DomainName, ChildNodes.RootMapUid AS RootMapId, ParentConnections.NodeUid AS MapId, ParentMetadata.MetadataValue AS MapName, ChildConnections.NodeUid AS Id, ChildNodeTypes.NodeTypeName AS NodeType, ChildMetadata.MetadataName AS FieldName, ChildMetadata.MetadataValue AS FieldValue 
      FROM (SELECT * FROM dbo.Relationships WHERE RelationshipTypeUid = @MapContainerRelationship) MapRelationships INNER JOIN  
      (SELECT * FROM dbo.Descriptors WHERE DescriptorTypeUid = @ToDescriptor) ParentConnections ON MapRelationships.RelationshipUid = ParentConnections.RelationshipUid INNER JOIN 
      (SELECT * FROM dbo.Descriptors WHERE DescriptorTypeUid = @FromDescriptor) ChildConnections ON MapRelationships.RelationshipUid = ChildConnections.RelationshipUid INNER JOIN 
      (SELECT * FROM dbo.Nodes WHERE NodeTypeUid = @DomainNodeType) ParentNodes ON ParentNodes.NodeUid = ParentConnections.NodeUid INNER JOIN 
      (SELECT * FROM dbo.Nodes WHERE NodeTypeUid = @MapNodeType) ChildNodes ON ChildNodes.NodeUid = ChildConnections.NodeUid INNER JOIN 
      (SELECT * FROM dbo.Metadata WHERE MetadataName = 'Name') ParentMetadata ON ParentMetadata.NodeUid = ParentConnections.NodeUid INNER JOIN 
      Metadata ChildMetadata ON ChildMetadata.NodeUid = ChildConnections.NodeUid INNER JOIN 
      (SELECT NodeTypeUid, REPLACE(REPLACE(NodeTypeName, 'Compendium', ''), 'Node', '') AS 'NodeTypeName' FROM dbo.NodeTypes) ChildNodeTypes ON ChildNodes.NodeTypeUid = ChildNodeTypes.NodeTypeUid 
      WHERE ChildMetadata.MetadataName NOT IN ('XPosition', 'YPosition', 'Video.StartPosition', 'Video.EndPosition', 'CollapseState', 'Visibility', 'AuthorVisibility')
      ORDER BY ChildConnections.NodeUid	
   END
   ELSE
   BEGIN
      -- Get all root maps for the specified domain.
      SELECT ChildNodes.DomainUid AS DomainId, ParentMetadata.MetadataValue AS DomainName, ChildNodes.RootMapUid AS RootMapId, ParentConnections.NodeUid AS MapId, ParentMetadata.MetadataValue AS MapName, ChildConnections.NodeUid AS Id, ChildNodeTypes.NodeTypeName AS NodeType, ChildMetadata.MetadataName AS FieldName, ChildMetadata.MetadataValue AS FieldValue 
      FROM (SELECT * FROM dbo.Relationships WHERE RelationshipTypeUid = @MapContainerRelationship AND DomainUid = @DomainId) MapRelationships INNER JOIN  
      (SELECT * FROM dbo.Descriptors WHERE DescriptorTypeUid = @ToDescriptor) ParentConnections ON MapRelationships.RelationshipUid = ParentConnections.RelationshipUid INNER JOIN 
      (SELECT * FROM dbo.Descriptors WHERE DescriptorTypeUid = @FromDescriptor) ChildConnections ON ChildConnections.RelationshipUid = MapRelationships.RelationshipUid INNER JOIN 
      (SELECT * FROM dbo.Nodes WHERE NodeTypeUid = @DomainNodeType) ParentNodes ON ParentNodes.NodeUid = ParentConnections.NodeUid INNER JOIN 
      (SELECT * FROM dbo.Nodes WHERE NodeTypeUid = @MapNodeType) ChildNodes ON ChildNodes.NodeUid = ChildConnections.NodeUid INNER JOIN 
      (SELECT * FROM dbo.Metadata WHERE MetadataName = 'Name') ParentMetadata ON ParentMetadata.NodeUid = ParentConnections.NodeUid INNER JOIN 
      Metadata ChildMetadata ON ChildMetadata.NodeUid = ChildConnections.NodeUid INNER JOIN 
      (SELECT NodeTypeUid, REPLACE(REPLACE(NodeTypeName, 'Compendium', ''), 'Node', '') AS 'NodeTypeName' FROM dbo.NodeTypes) ChildNodeTypes ON ChildNodes.NodeTypeUid = ChildNodeTypes.NodeTypeUid 
      WHERE ChildMetadata.MetadataName NOT IN ('XPosition', 'YPosition', 'Video.StartPosition', 'Video.EndPosition', 'CollapseState', 'Visibility', 'AuthorVisibility')
      ORDER BY ChildConnections.NodeUid			
   END
END

