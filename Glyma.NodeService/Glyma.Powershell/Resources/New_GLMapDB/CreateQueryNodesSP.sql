-- =============================================
-- Author:		Chris Tomich
-- Create date: 29/04/2014
-- Description:	This procedure was created to aid with the activity feeds.
-- =============================================
CREATE PROCEDURE QueryNodes
	-- Add the parameters for the stored procedure here
	@Nodes AS [dbo].[NodesList] READONLY
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @NodeDetails AS TABLE
	(
		[MapNodeUid] [uniqueidentifier] NULL,
		[ChildNodeUid] [uniqueidentifier] NULL
	)

	INSERT INTO @NodeDetails
		SELECT [OrganisedAndDistinctNodes].[MapNodeUid], [OrganisedAndDistinctNodes].[ChildNodeUid] FROM
		(
			SELECT DISTINCT [MapContainerRelationships].[ChildNodeUid], [Descriptors].[NodeUid] AS MapNodeUid FROM
			(
				SELECT [ToDescriptors].[ChildNodeUid], [Relationships].[RelationshipUid] FROM
				(
					SELECT [@Nodes].[NodeUid] AS ChildNodeUid, [Descriptors].[RelationshipUid] FROM [Descriptors] JOIN @Nodes ON [Descriptors].[NodeUid] = [@Nodes].[NodeUid] WHERE [Descriptors].[DescriptorTypeUid] = '96DA1782-058C-4F9B-BB1A-31B048F8C75A'
				) ToDescriptors JOIN [Relationships] ON [ToDescriptors].[RelationshipUid] = [Relationships].[RelationshipUid] WHERE [Relationships].[RelationshipTypeUid] = '4AFF46D7-87BE-48DD-B703-A93E38EF8FFB'
			) MapContainerRelationships JOIN [Descriptors] ON MapContainerRelationships.[RelationshipUid] = [Descriptors].[RelationshipUid] WHERE [Descriptors].[DescriptorTypeUid] = '07C91D35-4DAC-431B-966B-64C924B8CDAB'
		) OrganisedAndDistinctNodes

	SELECT [@NodeDetails].[MapNodeUid], [Nodes].* FROM @NodeDetails JOIN [Nodes] ON [@NodeDetails].[ChildNodeUid] = [Nodes].[NodeUid]

	SELECT [Metadata].* FROM [Metadata] JOIN @Nodes ON [Metadata].[NodeUid] = [@Nodes].[NodeUid] WHERE [Metadata].[RelationshipUid] IS NULL
	UNION
	SELECT [Metadata].* FROM [Metadata] JOIN @NodeDetails ON [Metadata].[NodeUid] = [@NodeDetails].[MapNodeUid] WHERE [Metadata].[RelationshipUid] IS NULL AND [Metadata].[MetadataName] = 'Name'
END