ALTER TABLE [dbo].[Metadata] ADD [RootMapUid] [uniqueidentifier] NULL

ALTER TABLE [dbo].[Metadata] ADD [DomainUid] [uniqueidentifier] NULL

ALTER TABLE [dbo].[Metadata] ADD [Created] [datetime2] NULL

ALTER TABLE [dbo].[Metadata] ADD [Modified] [datetime2] NULL

ALTER TABLE [dbo].[Metadata] ADD [CreatedBy] [nvarchar](100) NULL

ALTER TABLE [dbo].[Metadata] ADD [ModifiedBy] [nvarchar](100) NULL


ALTER TABLE [dbo].[Nodes] ADD [RootMapUid] [uniqueidentifier] NULL

ALTER TABLE [dbo].[Nodes] ADD [Created] [datetime2] NULL

ALTER TABLE [dbo].[Nodes] ADD [Modified] [datetime2] NULL

ALTER TABLE [dbo].[Nodes] ADD [CreatedBy] [nvarchar](100) NULL

ALTER TABLE [dbo].[Nodes] ADD [ModifiedBy] [nvarchar](100) NULL


ALTER TABLE [dbo].[Relationships] ADD [RootMapUid] [uniqueidentifier] NULL

ALTER TABLE [dbo].[Relationships] ADD [Created] [datetime2] NULL

ALTER TABLE [dbo].[Relationships] ADD [Modified] [datetime2] NULL

ALTER TABLE [dbo].[Relationships] ADD [CreatedBy] [nvarchar](100) NULL

ALTER TABLE [dbo].[Relationships] ADD [ModifiedBy] [nvarchar](100) NULL