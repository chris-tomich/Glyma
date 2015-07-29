CREATE FUNCTION [dbo].[udf_SearchMetadataContains] 
(	
	-- Add the parameters for the function here
	@metadataName nvarchar(1000), 
	@keywords nvarchar(4000)
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT *
	FROM Metadata
	WHERE MetadataName = @metadataName AND
		CONTAINS(MetadataValue, @keywords)
)