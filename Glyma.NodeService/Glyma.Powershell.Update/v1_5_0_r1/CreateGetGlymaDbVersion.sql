-- =============================================
-- Author:		Chris Tomich
-- Create date: 25/04/2014
-- Description:	This stored procedure will return the version of the database.
-- =============================================
CREATE PROCEDURE [dbo].[GetGlymaDbVersion]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 'v1.5.0r1'
END