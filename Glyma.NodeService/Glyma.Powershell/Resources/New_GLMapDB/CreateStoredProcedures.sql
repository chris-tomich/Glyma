/****** Object:  StoredProcedure [dbo].[ClearDomain]    Script Date: 07/12/2012 16:51:53 ******/
-- =============================================
-- Author:		Daniel Wale
-- Create date: 27/10/2011
-- Description:	Deletes a full domain from the DB
-- =============================================
CREATE PROCEDURE [dbo].[ClearDomain] 
	@DomainUid nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM Descriptors
	FROM         Descriptors INNER JOIN
                      Nodes ON Descriptors.NodeUid = Nodes.NodeUid
	WHERE     (Nodes.DomainUid = @DomainUid)

	DELETE
	FROM         Metadata
	FROM         Metadata INNER JOIN
                      Nodes ON Metadata.NodeUid = Nodes.NodeUid
	WHERE     (Nodes.DomainUid = @DomainUid)
	
	DELETE
	FROM         Metadata
	FROM         Metadata INNER JOIN
                      Relationships ON Metadata.RelationshipUid = Relationships.RelationshipUid
	WHERE     (Relationships.DomainUid = @DomainUid)

	DELETE
	FROM         Relationships
	WHERE     (DomainUid = @DomainUid)

	DELETE
	FROM         Nodes
	WHERE     (DomainUid = @DomainUid)

	DELETE
	FROM		[Domains]
	WHERE	  (DomainUid = @DomainUid)
END