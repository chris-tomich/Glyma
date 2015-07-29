-- =============================================
-- Author:		Chris Tomich
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[QueryMap]
	-- Add the parameters for the stored procedure here
	@DomainId uniqueidentifier,
	@NodeId uniqueidentifier, 
	@Depth int,
	@FullDomain bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @Nodes TABLE
	(
		[Level] INT,
		[Origin] UNIQUEIDENTIFIER,
		[RelationshipUid] UNIQUEIDENTIFIER,
		[DescriptorTypeUid] UNIQUEIDENTIFIER,
		NodeUid UNIQUEIDENTIFIER,
		NodeOriginalId NVARCHAR(50),
		NodeTypeUid UNIQUEIDENTIFIER,
		DomainUid UNIQUEIDENTIFIER,
		[RootMapUid] [uniqueidentifier] NULL,
		[Created] [datetime2](7) NULL,
		[Modified] [datetime2](7) NULL,
		[CreatedBy] [nvarchar](100),
		[ModifiedBy] [nvarchar](100)
	)
	
	DECLARE @NextNodesToSearch TABLE
	(
		NodeUid UNIQUEIDENTIFIER,
		NodeOriginalId NVARCHAR(50),
		NodeTypeUid UNIQUEIDENTIFIER,
		DomainUid UNIQUEIDENTIFIER,
		[RootMapUid] [uniqueidentifier] NULL,
		[Created] [datetime2](7) NULL,
		[Modified] [datetime2](7) NULL,
		[CreatedBy] [nvarchar](100),
		[ModifiedBy] [nvarchar](100)
	)

	DECLARE @CurrentLevel INT
	DECLARE @NodesCollectedPreviously INT
	DECLARE @NodesCollected INT

	SET @CurrentLevel = 0
	SET @NodesCollectedPreviously = 0
	SET @NodesCollected = 1

	INSERT INTO @Nodes
		SELECT 0 AS [Level], CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) AS [Origin], CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) AS [RelationshipUid], CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) AS [DescriptorTypeUid], [Nodes].[NodeUid], [Nodes].[NodeOriginalId], [Nodes].[NodeTypeUid], [Nodes].[DomainUid], [Nodes].[RootMapUid], [Nodes].[Created], [Nodes].[Modified], [Nodes].[CreatedBy], [Nodes].[ModifiedBy] FROM [Nodes] WHERE [Nodes].[DomainUid] = @DomainId AND [Nodes].[NodeUid] = @NodeId
		
	INSERT INTO @NextNodesToSearch
		SELECT [Nodes].[NodeUid], [Nodes].[NodeOriginalId], [Nodes].[NodeTypeUid], [Nodes].[DomainUid], [Nodes].[RootMapUid], [Nodes].[Created], [Nodes].[Modified], [Nodes].[CreatedBy], [Nodes].[ModifiedBy] FROM [Nodes] WHERE [Nodes].[DomainUid] = @DomainId AND [Nodes].[NodeUid] = @NodeId

	WHILE (@Depth >= 0 AND @CurrentLevel < @Depth) OR (@Depth < 0 AND @NodesCollectedPreviously <> @NodesCollected)
	BEGIN
		SELECT DISTINCT [@Nodes].[Level], [@Nodes].[Origin], [@Nodes].[RelationshipUid], [@Nodes].[DescriptorTypeUid], [@Nodes].[NodeUid], [@Nodes].[NodeOriginalId], [@Nodes].[NodeTypeUid], [@Nodes].[DomainUid], [@Nodes].[RootMapUid], [@Nodes].[Created], [@Nodes].[Modified], [@Nodes].[CreatedBy], [@Nodes].[ModifiedBy] FROM @Nodes WHERE [@Nodes].[Level] = @CurrentLevel;
		
		IF @FullDomain = 0
		BEGIN
			WITH
			RecursiveNodeSearch
			AS
			(
				SELECT 0 AS [Level], CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) AS [Origin], CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) AS [RelationshipUid], CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) AS [DescriptorTypeUid], [@NextNodesToSearch].[NodeUid], [@NextNodesToSearch].[NodeOriginalId], [@NextNodesToSearch].[NodeTypeUid], [@NextNodesToSearch].[DomainUid], [@NextNodesToSearch].[RootMapUid], [@NextNodesToSearch].[Created], [@NextNodesToSearch].[Modified], [@NextNodesToSearch].[CreatedBy], [@NextNodesToSearch].[ModifiedBy] FROM @NextNodesToSearch
				UNION ALL
				SELECT [Level], SecondLevelDescriptors.[Origin], [SecondLevelDescriptors].[RelationshipUid], [SecondLevelDescriptors].[DescriptorTypeUid], [FilteredNodes].[NodeUid], [FilteredNodes].[NodeOriginalId], [FilteredNodes].[NodeTypeUid], [FilteredNodes].[DomainUid], [FilteredNodes].[RootMapUid], [FilteredNodes].[Created], [FilteredNodes].[Modified], [FilteredNodes].[CreatedBy], [FilteredNodes].[ModifiedBy] FROM
				(
					SELECT [Level], MatchingRelationships.[Origin], [Descriptors].[DescriptorUid], [Descriptors].[DescriptorTypeUid], [Descriptors].[NodeUid], [Descriptors].[RelationshipUid] FROM
					(
						SELECT [Level], MatchingDescriptors.[Origin], [FilteredRelationships].[RelationshipUid], [FilteredRelationships].[RelationshipOriginalId], [FilteredRelationships].[RelationshipTypeUid], [FilteredRelationships].[DomainUid] FROM
						(
							SELECT (FilteredNodes.[Level] + 1) AS [Level], [Descriptors].[NodeUid] AS [Origin], [Descriptors].[DescriptorUid], [Descriptors].[DescriptorTypeUid], [Descriptors].[NodeUid], [Descriptors].[RelationshipUid] FROM [Descriptors] INNER JOIN (SELECT * FROM RecursiveNodeSearch WHERE RecursiveNodeSearch.[Level] < 1 AND RecursiveNodeSearch.[DomainUid] = @DomainId AND RecursiveNodeSearch.[NodeTypeUid] <> '263754C2-2F31-4D21-B9C4-6509E00A5E94') AS FilteredNodes ON [Descriptors].[NodeUid] = FilteredNodes.[NodeUid]
						) AS MatchingDescriptors
						INNER JOIN (SELECT * FROM [Relationships] WHERE [Relationships].[DomainUid] = @DomainId) AS FilteredRelationships ON MatchingDescriptors.[RelationshipUid] = [FilteredRelationships].[RelationshipUid]
					) AS MatchingRelationships INNER JOIN [Descriptors] ON MatchingRelationships.[RelationshipUid] = [Descriptors].[RelationshipUid] AND MatchingRelationships.[Origin] <> [Descriptors].[NodeUid]
				) AS SecondLevelDescriptors
				INNER JOIN (SELECT * FROM [Nodes] WHERE [Nodes].[DomainUid] = @DomainId) AS FilteredNodes ON [FilteredNodes].[NodeUid] = SecondLevelDescriptors.[NodeUid]
			)
			INSERT INTO @Nodes
				SELECT DISTINCT (@CurrentLevel + 1) AS [Level], FilteredRecursiveNodeSearch.[Origin], FilteredRecursiveNodeSearch.[RelationshipUid], FilteredRecursiveNodeSearch.[DescriptorTypeUid], FilteredRecursiveNodeSearch.[NodeUid], FilteredRecursiveNodeSearch.[NodeOriginalId], FilteredRecursiveNodeSearch.[NodeTypeUid], FilteredRecursiveNodeSearch.[DomainUid], FilteredRecursiveNodeSearch.[RootMapUid], FilteredRecursiveNodeSearch.[Created], FilteredRecursiveNodeSearch.[Modified], FilteredRecursiveNodeSearch.[CreatedBy], FilteredRecursiveNodeSearch.[ModifiedBy] FROM (SELECT * FROM RecursiveNodeSearch WHERE RecursiveNodeSearch.[NodeTypeUid] <> '263754C2-2F31-4D21-B9C4-6509E00A5E94') AS FilteredRecursiveNodeSearch LEFT JOIN @Nodes ON FilteredRecursiveNodeSearch.[NodeUid] = [@Nodes].[NodeUid] WHERE [@Nodes].[NodeUid] IS NULL
			
			DELETE FROM @NextNodesToSearch
			
			INSERT INTO @NextNodesToSearch
				SELECT DISTINCT [@Nodes].[NodeUid], [@Nodes].[NodeOriginalId], [@Nodes].[NodeTypeUid], [@Nodes].[DomainUid], [@Nodes].[RootMapUid], [@Nodes].[Created], [@Nodes].[Modified], [@Nodes].[CreatedBy], [@Nodes].[ModifiedBy] FROM @Nodes WHERE [@Nodes].[Level] = (@CurrentLevel + 1)
				EXCEPT
				SELECT DISTINCT [@Nodes].[NodeUid], [@Nodes].[NodeOriginalId], [@Nodes].[NodeTypeUid], [@Nodes].[DomainUid], [@Nodes].[RootMapUid], [@Nodes].[Created], [@Nodes].[Modified], [@Nodes].[CreatedBy], [@Nodes].[ModifiedBy] FROM @Nodes WHERE [@Nodes].[Level] = @CurrentLevel
		END
		
		IF @FullDomain = 1
		BEGIN
			WITH
			RecursiveNodeSearch
			AS
			(
				SELECT 0 AS [Level], CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) AS [Origin], CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) AS [RelationshipUid], CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) AS [DescriptorTypeUid], [@NextNodesToSearch].[NodeUid], [@NextNodesToSearch].[NodeOriginalId], [@NextNodesToSearch].[NodeTypeUid], [@NextNodesToSearch].[DomainUid], [@NextNodesToSearch].[RootMapUid], [@NextNodesToSearch].[Created], [@NextNodesToSearch].[Modified], [@NextNodesToSearch].[CreatedBy], [@NextNodesToSearch].[ModifiedBy] FROM @NextNodesToSearch
				UNION ALL
				SELECT [Level], SecondLevelDescriptors.[Origin], [SecondLevelDescriptors].[RelationshipUid], [SecondLevelDescriptors].[DescriptorTypeUid], [FilteredNodes].[NodeUid], [FilteredNodes].[NodeOriginalId], [FilteredNodes].[NodeTypeUid], [FilteredNodes].[DomainUid], [FilteredNodes].[RootMapUid], [FilteredNodes].[Created], [FilteredNodes].[Modified], [FilteredNodes].[CreatedBy], [FilteredNodes].[ModifiedBy] FROM
				(
					SELECT [Level], MatchingRelationships.[Origin], [Descriptors].[DescriptorUid], [Descriptors].[DescriptorTypeUid], [Descriptors].[NodeUid], [Descriptors].[RelationshipUid] FROM
					(
						SELECT [Level], MatchingDescriptors.[Origin], [FilteredRelationships].[RelationshipUid], [FilteredRelationships].[RelationshipOriginalId], [FilteredRelationships].[RelationshipTypeUid], [FilteredRelationships].[DomainUid] FROM
						(
							SELECT (FilteredNodes.[Level] + 1) AS [Level], [Descriptors].[NodeUid] AS [Origin], [Descriptors].[DescriptorUid], [Descriptors].[DescriptorTypeUid], [Descriptors].[NodeUid], [Descriptors].[RelationshipUid] FROM [Descriptors] INNER JOIN (SELECT * FROM RecursiveNodeSearch WHERE RecursiveNodeSearch.[Level] < 1 AND RecursiveNodeSearch.[DomainUid] = @DomainId) AS FilteredNodes ON [Descriptors].[NodeUid] = FilteredNodes.[NodeUid]
						) AS MatchingDescriptors
						INNER JOIN (SELECT * FROM [Relationships] WHERE [Relationships].[DomainUid] = @DomainId) AS FilteredRelationships ON MatchingDescriptors.[RelationshipUid] = [FilteredRelationships].[RelationshipUid]
					) AS MatchingRelationships INNER JOIN [Descriptors] ON MatchingRelationships.[RelationshipUid] = [Descriptors].[RelationshipUid] AND MatchingRelationships.[Origin] <> [Descriptors].[NodeUid]
				) AS SecondLevelDescriptors
				INNER JOIN (SELECT * FROM [Nodes] WHERE [Nodes].[DomainUid] = @DomainId) AS FilteredNodes ON [FilteredNodes].[NodeUid] = SecondLevelDescriptors.[NodeUid]
			)
			INSERT INTO @Nodes
				SELECT DISTINCT (@CurrentLevel + 1) AS [Level], RecursiveNodeSearch.[Origin], RecursiveNodeSearch.[RelationshipUid], RecursiveNodeSearch.[DescriptorTypeUid], RecursiveNodeSearch.[NodeUid], RecursiveNodeSearch.[NodeOriginalId], RecursiveNodeSearch.[NodeTypeUid], RecursiveNodeSearch.[DomainUid], RecursiveNodeSearch.[RootMapUid], RecursiveNodeSearch.[Created], RecursiveNodeSearch.[Modified], RecursiveNodeSearch.[CreatedBy], RecursiveNodeSearch.[ModifiedBy] FROM RecursiveNodeSearch LEFT JOIN @Nodes ON RecursiveNodeSearch.[NodeUid] = [@Nodes].[NodeUid] WHERE [@Nodes].[NodeUid] IS NULL
			
			DELETE FROM @NextNodesToSearch
			
			INSERT INTO @NextNodesToSearch
				SELECT DISTINCT [@Nodes].[NodeUid], [@Nodes].[NodeOriginalId], [@Nodes].[NodeTypeUid], [@Nodes].[DomainUid], [@Nodes].[RootMapUid], [@Nodes].[Created], [@Nodes].[Modified], [@Nodes].[CreatedBy], [@Nodes].[ModifiedBy] FROM @Nodes WHERE [@Nodes].[Level] = (@CurrentLevel + 1)
				EXCEPT
				SELECT DISTINCT [@Nodes].[NodeUid], [@Nodes].[NodeOriginalId], [@Nodes].[NodeTypeUid], [@Nodes].[DomainUid], [@Nodes].[RootMapUid], [@Nodes].[Created], [@Nodes].[Modified], [@Nodes].[CreatedBy], [@Nodes].[ModifiedBy] FROM @Nodes WHERE [@Nodes].[Level] = @CurrentLevel
		END
		
		SET @NodesCollectedPreviously = @NodesCollected		
		SELECT @NodesCollected = COUNT(*) FROM @Nodes
		
		SET @CurrentLevel = @CurrentLevel + 1
	END
	
	SELECT DISTINCT [@Nodes].[Level], [@Nodes].[Origin], [@Nodes].[RelationshipUid], [@Nodes].[DescriptorTypeUid], [@Nodes].[NodeUid], [@Nodes].[NodeOriginalId], [@Nodes].[NodeTypeUid], [@Nodes].[DomainUid], [@Nodes].[RootMapUid], [@Nodes].[Created], [@Nodes].[Modified], [@Nodes].[CreatedBy], [@Nodes].[ModifiedBy] FROM @Nodes WHERE [@Nodes].[Level] = @CurrentLevel;
	
	DECLARE @Descriptors TABLE
	(
		[DescriptorUid] UNIQUEIDENTIFIER,
		[DescriptorTypeUid] UNIQUEIDENTIFIER,
		[NodeUid] UNIQUEIDENTIFIER,
		[RelationshipUid] UNIQUEIDENTIFIER
	)
	
	DECLARE @Relationships TABLE
	(
		[RelationshipUid] UNIQUEIDENTIFIER,
		[RelationshipOriginalId] NVARCHAR(50),
		[RelationshipTypeUid] UNIQUEIDENTIFIER,
		[DomainUid] UNIQUEIDENTIFIER,
		[RootMapUid] [uniqueidentifier] NULL,
		[Created] [datetime2](7) NULL,
		[Modified] [datetime2](7) NULL,
		[CreatedBy] [nvarchar](100),
		[ModifiedBy] [nvarchar](100)
	)
	
	INSERT INTO @Descriptors
		SELECT DISTINCT [Descriptors].[DescriptorUid], [Descriptors].[DescriptorTypeUid], [Descriptors].[NodeUid], [Descriptors].[RelationshipUid] FROM Descriptors INNER JOIN (SELECT DISTINCT [@Nodes].[NodeUid] FROM @Nodes) DistinctRelationships ON [Descriptors].[NodeUid] = [DistinctRelationships].[NodeUid];
	
	INSERT INTO @Relationships
		SELECT DISTINCT [Relationships].[RelationshipUid], [Relationships].[RelationshipOriginalId], [Relationships].[RelationshipTypeUid], [Relationships].[DomainUid], [Relationships].[RootMapUid], [Relationships].[Created], [Relationships].[Modified], [Relationships].[CreatedBy], [Relationships].[ModifiedBy] FROM Relationships INNER JOIN (SELECT DISTINCT [@Descriptors].[RelationshipUid] FROM @Descriptors) DistinctDescriptors ON [Relationships].[RelationshipUid] = [DistinctDescriptors].[RelationshipUid] WHERE [Relationships].[DomainUid] = @DomainId;
	
	INSERT INTO @Descriptors
		SELECT DISTINCT [Descriptors].[DescriptorUid], [Descriptors].[DescriptorTypeUid], [Descriptors].[NodeUid], [Descriptors].[RelationshipUid] FROM Descriptors INNER JOIN @Relationships ON [Descriptors].[RelationshipUid] = [@Relationships].[RelationshipUid];
	
	SELECT DISTINCT [Metadata].[MetadataId], [Metadata].[MetadataTypeUid], [Metadata].[NodeUid], [Metadata].[RelationshipUid], [Metadata].[DescriptorTypeUid], [Metadata].[MetadataName], [Metadata].[MetadataValue], [Metadata].[DomainUid], [Metadata].[RootMapUid], [Metadata].[Created], [Metadata].[Modified], [Metadata].[CreatedBy], [Metadata].[ModifiedBy] FROM Metadata INNER JOIN @Nodes ON [Metadata].[NodeUid] = [@Nodes].[NodeUid]
	UNION
	SELECT DISTINCT [Metadata].[MetadataId], [Metadata].[MetadataTypeUid], [Metadata].[NodeUid], [Metadata].[RelationshipUid], [Metadata].[DescriptorTypeUid], [Metadata].[MetadataName], [Metadata].[MetadataValue], [Metadata].[DomainUid], [Metadata].[RootMapUid], [Metadata].[Created], [Metadata].[Modified], [Metadata].[CreatedBy], [Metadata].[ModifiedBy] FROM Metadata INNER JOIN @Relationships ON [Metadata].[NodeUid] = [@Relationships].[RelationshipUid] AND [Metadata].[NodeUid] IS NULL;
	
	SELECT DISTINCT * FROM @Descriptors
	
	SELECT DISTINCT * FROM @Relationships
END