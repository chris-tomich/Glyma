CREATE PROCEDURE dbo.GetAllowedGroups 
	@DomainId UNIQUEIDENTIFIER,
	@MapId UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @DomainId IS NOT NULL
	BEGIN
		IF @MapId IS NULL
		BEGIN
			SELECT GRA.SecurableContextId, G.GroupId, G.DisplayName, G.GroupSPId, SC.SiteSPId, GRA.SecurableParentUid, GRA.SecurableObjectUid 
			FROM GroupAssociations GRA INNER JOIN 
			Groups G ON GRA.GroupId = G.GroupId INNER JOIN 
			SecurableContexts SC ON SC.SecurableContextId = GRA.SecurableContextId
			WHERE GRA.SecurableParentUid IS NULL AND
			GRA.SecurableObjectUid = @DomainId AND
			SC.SiteSPId IS NOT NULL AND
			G.GroupSPId IS NOT NULL		
		END
		ELSE IF @MapId IS NOT NULL
		BEGIN
			-- Insert statements for procedure here
			SELECT GRA.SecurableContextId, G.GroupId, G.DisplayName, G.GroupSPId, SC.SiteSPId, GRA.SecurableParentUid, GRA.SecurableObjectUid 
			FROM GroupAssociations GRA INNER JOIN 
			Groups G ON GRA.GroupId = G.GroupId INNER JOIN 
			SecurableContexts SC ON SC.SecurableContextId = GRA.SecurableContextId
			WHERE (GRA.SecurableParentUid = @DomainId AND
			GRA.SecurableObjectUid = @MapId OR 
			GRA.SecurableParentUid IS NULL AND GRA.SecurableObjectUid = @DomainId) AND
			SC.SiteSPId IS NOT NULL AND
			G.GroupSPId IS NOT NULL
		END
	END
END