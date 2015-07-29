/****** Object:  Table [dbo].[Operations]    Script Date: 08/22/2013 22:14:59 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Operations](
	[OperationId] [int] NOT NULL,
	[Name] [nvarchar](25) NULL,
 CONSTRAINT [PK_Operations] PRIMARY KEY CLUSTERED 
(
	[OperationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
