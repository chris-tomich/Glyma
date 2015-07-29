CREATE TABLE [dbo].[AuditLogs](
	[LogId] [bigint] IDENTITY(1,1) NOT NULL,
	[OperationName] [nvarchar](50) NULL,
	[CallingUrl] [nvarchar](255) NULL,
	[DomainUid] [uniqueidentifier] NULL,
	[NodeUid] [uniqueidentifier] NULL,
	[RootMapUid] [uniqueidentifier] NULL,
	[MaxDepth] [int] NULL,
	[ObjectIndex] [int] NULL,
	[EdgeConditions] [nvarchar](max) NULL,
	[FilterConditions] [nvarchar](max) NULL,
	[SearchConditions] [nvarchar](max) NULL,
	[PageNumber] [int] NULL,
	[PageSize] [int] NULL,
	[Timestamp] [datetime2](7) NULL,
	[User] [nvarchar](100) NULL,
 CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]