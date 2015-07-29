-- ================================
-- Create User-defined Table Type
-- ================================
-- Create the NodesToSearch data type
CREATE TYPE [dbo].[NodesList] AS TABLE 
(
	[NodeUid] UNIQUEIDENTIFIER
)
