/****** Object:  Table [dbo].[Parameters]    Script Date: 08/22/2013 22:16:06 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Parameters](
	[ParameterUid] [uniqueidentifier] NOT NULL,
	[Value] [uniqueidentifier] NULL,
	[SessionUid] [uniqueidentifier] NULL,
	[IsDelayed] [bit] NULL,
	[ParameterType] [int] NULL,
 CONSTRAINT [PK_Parameters] PRIMARY KEY CLUSTERED 
(
	[ParameterUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
