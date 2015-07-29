/****** Object:  Table [dbo].[Descriptors]    Script Date: 07/12/2012 08:34:14 ******/
CREATE TABLE [dbo].[Descriptors](
	[DescriptorUid] [uniqueidentifier] NOT NULL,
	[DescriptorTypeUid] [uniqueidentifier] NULL,
	[NodeUid] [uniqueidentifier] NULL,
	[RelationshipUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Descriptors] PRIMARY KEY CLUSTERED 
(
	[DescriptorUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[DescriptorTypes]    Script Date: 07/12/2012 08:36:05 ******/
CREATE TABLE [dbo].[DescriptorTypes](
	[DescriptorTypeUid] [uniqueidentifier] NOT NULL,
	[DescriptorTypeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_DescriptorTypes] PRIMARY KEY CLUSTERED 
(
	[DescriptorTypeUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[Domains]    Script Date: 07/12/2012 08:36:37 ******/
CREATE TABLE [dbo].[Domains](
	[DomainUid] [uniqueidentifier] NOT NULL,
	[DomainOriginalId] [nvarchar](50) NULL,
 CONSTRAINT [PK_Domains] PRIMARY KEY CLUSTERED 
(
	[DomainUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[Metadata]    Script Date: 07/12/2012 13:42:01 ******/
CREATE TABLE [dbo].[Metadata](
	[MetadataId] [uniqueidentifier] NOT NULL,
	[MetadataTypeUid] [uniqueidentifier] NULL,
	[NodeUid] [uniqueidentifier] NULL,
	[RelationshipUid] [uniqueidentifier] NULL,
	[DescriptorTypeUid] [uniqueidentifier] NULL,
	[MetadataName] [nvarchar](50) NULL,
	[MetadataValue] [nvarchar](max) NULL,
	[RootMapUid] [uniqueidentifier] NULL,
	[DomainUid] [uniqueidentifier] NULL,
	[Created] [datetime2] NULL,
	[Modified] [datetime2] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[ModifiedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_Metadata] PRIMARY KEY CLUSTERED 
(
	[MetadataId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[MetadataTypes]    Script Date: 07/12/2012 13:42:25 ******/
CREATE TABLE [dbo].[MetadataTypes](
	[MetadataTypeUid] [uniqueidentifier] NOT NULL,
	[MetadataTypeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_MetadataTypes] PRIMARY KEY CLUSTERED 
(
	[MetadataTypeUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[Nodes]    Script Date: 07/12/2012 13:42:47 ******/
CREATE TABLE [dbo].[Nodes](
	[NodeUid] [uniqueidentifier] NOT NULL,
	[NodeOriginalId] [nvarchar](50) NULL,
	[NodeTypeUid] [uniqueidentifier] NULL,
	[DomainUid] [uniqueidentifier] NULL,
	[RootMapUid] [uniqueidentifier] NULL,
	[Created] [datetime2] NULL,
	[Modified] [datetime2] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[ModifiedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_Nodes] PRIMARY KEY CLUSTERED 
(
	[NodeUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[NodeTypes]    Script Date: 07/12/2012 13:44:19 ******/
CREATE TABLE [dbo].[NodeTypes](
	[NodeTypeUid] [uniqueidentifier] NOT NULL,
	[NodeTypeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_NodeTypes] PRIMARY KEY CLUSTERED 
(
	[NodeTypeUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[RelationshipDescriptorPairs]    Script Date: 07/12/2012 13:44:49 ******/
CREATE TABLE [dbo].[RelationshipDescriptorPairs](
	[PairId] [int] IDENTITY(1,1) NOT NULL,
	[RelationshipTypeUid] [uniqueidentifier] NOT NULL,
	[DescriptorTypeUid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_RelationshipDescriptorPairs] PRIMARY KEY CLUSTERED 
(
	[PairId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[Relationships]    Script Date: 07/12/2012 13:45:45 ******/
CREATE TABLE [dbo].[Relationships](
	[RelationshipUid] [uniqueidentifier] NOT NULL,
	[RelationshipOriginalId] [nvarchar](50) NULL,
	[RelationshipTypeUid] [uniqueidentifier] NULL,
	[DomainUid] [uniqueidentifier] NULL,
	[RootMapUid] [uniqueidentifier] NULL,
	[Created] [datetime2] NULL,
	[Modified] [datetime2] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[ModifiedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_Relationships] PRIMARY KEY CLUSTERED 
(
	[RelationshipUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[RelationshipTypes]    Script Date: 07/12/2012 13:46:05 ******/
CREATE TABLE [dbo].[RelationshipTypes](
	[RelationshipTypeUid] [uniqueidentifier] NOT NULL,
	[RelationshipTypeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_RelationshipTypes] PRIMARY KEY CLUSTERED 
(
	[RelationshipTypeUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]