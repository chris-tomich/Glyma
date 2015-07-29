/****** Object:  Table [dbo].[Transactions]    Script Date: 08/22/2013 22:16:21 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Transactions](
	[TransactionId] [bigint] IDENTITY(1,1) NOT NULL,
	[TransactionTimestamp] [datetime2] NULL,
	[User] [nvarchar](100) NULL,
	[SessionUid] [uniqueidentifier] NULL,
	[OperationId] [int] NULL,
	[ResponseParameterUid] [uniqueidentifier] NULL,
	[DomainParameterUid] [uniqueidentifier] NULL,
	[RootMapParameterUid] [uniqueidentifier] NULL,
	[NodeParameterUid] [uniqueidentifier] NULL,
	[DescriptorParameterUid] [uniqueidentifier] NULL,
	[RelationshipParameterUid] [uniqueidentifier] NULL,
	[MetadataParameterUid] [uniqueidentifier] NULL,
	[NodeTypeUid] [uniqueidentifier] NULL,
	[DescriptorTypeUid] [uniqueidentifier] NULL,
	[RelationshipTypeUid] [uniqueidentifier] NULL,
	[MetadataTypeUid] [uniqueidentifier] NULL,
	[MetadataName] [nvarchar](50) NULL,
	[MetadataValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_Operations] FOREIGN KEY([OperationId])
REFERENCES [dbo].[Operations] ([OperationId])

ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transactions_Operations]

ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_Parameters] FOREIGN KEY([ResponseParameterUid])
REFERENCES [dbo].[Parameters] ([ParameterUid])

ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transactions_Parameters]
