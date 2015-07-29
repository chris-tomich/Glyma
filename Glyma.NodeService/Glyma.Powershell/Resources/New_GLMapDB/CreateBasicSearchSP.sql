-- =============================================
-- Author:		Chris Tomich
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BasicSearch]
	-- Add the parameters for the stored procedure here
	@DomainId UNIQUEIDENTIFIER,
	@SearchTerms NVARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @MatchingMetadata TABLE
	(
		[MetadataId] UNIQUEIDENTIFIER NOT NULL,
		[MetadataTypeUid] UNIQUEIDENTIFIER NULL,
		[NodeUid] UNIQUEIDENTIFIER NULL,
		[RelationshipUid] UNIQUEIDENTIFIER NULL,
		[DescriptorTypeUid] UNIQUEIDENTIFIER NULL,
		[MetadataName] NVARCHAR(50) NULL,
		[MetadataValue] NVARCHAR(max) NULL,
		[DomainUid] [uniqueidentifier],
		[RootMapUid] [uniqueidentifier] NULL,
		[Created] [datetime2](7) NULL,
		[Modified] [datetime2](7) NULL,
		[CreatedBy] [nvarchar](100),
		[ModifiedBy] [nvarchar](100)
	)
	
	DECLARE @MatchingNodes TABLE
	(
		NodeUid UNIQUEIDENTIFIER NOT NULL,
		NodeOriginalId NVARCHAR(50),
		NodeTypeUid UNIQUEIDENTIFIER,
		DomainUid UNIQUEIDENTIFIER,
		[RootMapUid] [uniqueidentifier] NULL,
		[Created] [datetime2](7) NULL,
		[Modified] [datetime2](7) NULL,
		[CreatedBy] [nvarchar](100),
		[ModifiedBy] [nvarchar](100)
	)
	
    INSERT INTO @MatchingNodes
		SELECT DISTINCT Nodes.* FROM
			Nodes
		INNER JOIN
			(SELECT DISTINCT *
			FROM Metadata
			WHERE FREETEXT (Metadata.MetadataValue, @SearchTerms)) AS MatchingMetadata
		ON Nodes.NodeUid = MatchingMetadata.NodeUid
		WHERE Nodes.DomainUid = @DomainId
	
	SELECT * FROM @MatchingNodes
		
	SELECT Metadata.* FROM Metadata INNER JOIN @MatchingNodes ON Metadata.NodeUid = [@MatchingNodes].NodeUid
END