CREATE PROCEDURE dbo.GetChildNodes 
	@MapId UNIQUEIDENTIFIER, 
	@NodeId UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @FromToRelationship uniqueidentifier
	DECLARE @TranscludedFromToRelationship uniqueidentifier
	DECLARE @ToDescriptor uniqueidentifier
	DECLARE @FromDescriptor uniqueidentifier
	DECLARE @TransclusionMapDescriptor uniqueidentifier
	
	SET @FromToRelationship = 'FE31AE41-5501-4B13-8F4A-AFE902A78F40'
	SET @TranscludedFromToRelationship = '9B64675F-D27B-42F1-BDAA-38D4697CC27A'
	SET @ToDescriptor = '07C91D35-4DAC-431B-966B-64C924B8CDAB'
	SET @FromDescriptor = '96DA1782-058C-4F9B-BB1A-31B048F8C75A'
	SET @TransclusionMapDescriptor = '47107835-A485-4A23-BF0C-3FC631A07777'
	
	SELECT ParentRelationships.DomainUid AS DomainId, MapConnection.NodeUid AS MapId, ParentConnections.NodeUid AS Id, ParentMetadata.MetadataValue AS Name, ChildConnections.NodeUid AS ChildId, ChildMetadata.MetadataValue AS ChildName, ChildNodeTypes.NodeTypeName AS ChildNodeType
	FROM 
	(SELECT * FROM Relationships WHERE RelationshipTypeUid IN (@FromToRelationship, @TranscludedFromToRelationship)) ParentRelationships INNER JOIN
	(SELECT * FROM Descriptors WHERE DescriptorTypeUid = @FromDescriptor) ChildConnections ON ChildConnections.RelationshipUid = ParentRelationships.RelationshipUid INNER JOIN
	(SELECT * FROM Descriptors WHERE DescriptorTypeUid = @ToDescriptor AND NodeUid = @NodeId) ParentConnections ON ChildConnections.RelationshipUid = ParentConnections.RelationshipUid INNER JOIN
	Nodes ChildNodes ON ChildNodes.NodeUid = ChildConnections.NodeUid INNER JOIN
	(SELECT NodeTypeUid, REPLACE(REPLACE(NodeTypeName, 'Compendium', ''), 'Node', '') AS 'NodeTypeName' FROM dbo.NodeTypes) ChildNodeTypes ON ChildNodeTypes.NodeTypeUid = ChildNodes.NodeTypeUid INNER JOIN
	(SELECT * FROM Metadata WHERE MetadataName = 'Name') ChildMetadata ON ChildMetadata.NodeUid = ChildConnections.NodeUid INNER JOIN
	(SELECT * FROM Metadata WHERE MetadataName = 'Name') ParentMetadata ON ParentMetadata.NodeUid = ParentConnections.NodeUid LEFT OUTER JOIN
	(SELECT * FROM Descriptors WHERE DescriptorTypeUid = @TransclusionMapDescriptor) MapConnection ON MapConnection.RelationshipUid = ChildConnections.RelationshipUid 
	WHERE MapConnection.NodeUid IS NULL OR MapConnection.NodeUid = @MapId

END

