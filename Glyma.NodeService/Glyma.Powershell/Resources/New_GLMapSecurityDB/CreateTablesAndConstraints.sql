CREATE TABLE [dbo].[SecurableContexts](
	[SecurableContextId] [int] IDENTITY(1,1) NOT NULL,
	[SecurableContextName] [nvarchar](max) NULL,
	[SecurableContextUid] [uniqueidentifier] NOT NULL,
	[SiteSPID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_SecurableContexts] PRIMARY KEY CLUSTERED 
(
	[SecurableContextId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[SecurableContextDatabases](
	[SecurableContextId] [int] NOT NULL,
	[DatabaseName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_SecurableContextDatabases] PRIMARY KEY CLUSTERED 
(
	[SecurableContextId] ASC,
	[DatabaseName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[SecurableContextDatabases]  WITH CHECK ADD  CONSTRAINT [FK_SecurableContextDatabases_SecurableContexts] FOREIGN KEY([SecurableContextId])
REFERENCES [dbo].[SecurableContexts] ([SecurableContextId])

ALTER TABLE [dbo].[SecurableContextDatabases] CHECK CONSTRAINT [FK_SecurableContextDatabases_SecurableContexts]

CREATE TABLE [dbo].[Groups](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](max) NULL,
	[SecurableContextId] [int] NOT NULL,
	[GroupSPID] [int] NOT NULL,
	[WebSPID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[SecurableObjects](
	[SecurableObjectUid] [uniqueidentifier] NOT NULL,
	[SecurableContextId] [int] NOT NULL,
	[BreaksInheritance] [bit] NOT NULL,
 CONSTRAINT [PK_SecurableObjects] PRIMARY KEY CLUSTERED 
(
	[SecurableObjectUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[SecurableObjects] ADD  CONSTRAINT [DF_SecurableObjects_BreaksInheritance]  DEFAULT ((0)) FOR [BreaksInheritance]

ALTER TABLE [dbo].[SecurableObjects]  WITH CHECK ADD  CONSTRAINT [FK_SecurableObjects_SecurableContexts] FOREIGN KEY([SecurableContextId])
REFERENCES [dbo].[SecurableContexts] ([SecurableContextId])

ALTER TABLE [dbo].[SecurableObjects] CHECK CONSTRAINT [FK_SecurableObjects_SecurableContexts]

CREATE TABLE [dbo].[GroupAssociations](
	[GroupAssociationId] [bigint] IDENTITY(1,1) NOT NULL,
	[GroupId] [int] NOT NULL,
	[SecurableParentUid] [uniqueidentifier] NULL,
	[SecurableObjectUid] [uniqueidentifier] NOT NULL,
	[SecurableContextId] [int] NOT NULL,
 CONSTRAINT [PK_RoleAssociations] PRIMARY KEY CLUSTERED 
(
	[GroupAssociationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[GroupAssociations]  WITH CHECK ADD  CONSTRAINT [FK_GroupAssociations_Groups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Groups] ([GroupId])

ALTER TABLE [dbo].[GroupAssociations] CHECK CONSTRAINT [FK_GroupAssociations_Groups]

ALTER TABLE [dbo].[GroupAssociations]  WITH CHECK ADD  CONSTRAINT [FK_GroupAssociations_SecurableContexts] FOREIGN KEY([SecurableContextId])
REFERENCES [dbo].[SecurableContexts] ([SecurableContextId])

ALTER TABLE [dbo].[GroupAssociations] CHECK CONSTRAINT [FK_GroupAssociations_SecurableContexts]

ALTER TABLE [dbo].[GroupAssociations]  WITH CHECK ADD  CONSTRAINT [FK_GroupAssociations_SecurableObjects] FOREIGN KEY([SecurableObjectUid])
REFERENCES [dbo].[SecurableObjects] ([SecurableObjectUid])

ALTER TABLE [dbo].[GroupAssociations] CHECK CONSTRAINT [FK_GroupAssociations_SecurableObjects]