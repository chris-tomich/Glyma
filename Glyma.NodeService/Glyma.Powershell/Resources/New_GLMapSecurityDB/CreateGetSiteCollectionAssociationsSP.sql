CREATE PROCEDURE dbo.GetSiteCollectionAssociations 
	@DatabaseName NVARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT SC.SecurableContextId, SD.DatabaseName, SC.SiteSPId
	FROM SecurableContexts SC INNER JOIN SecurableContextDatabases SD ON SC.SecurableContextId = SD.SecurableContextId
	WHERE SD.DatabaseName = @DatabaseName AND
	SC.SiteSPId IS NOT NULL	
END