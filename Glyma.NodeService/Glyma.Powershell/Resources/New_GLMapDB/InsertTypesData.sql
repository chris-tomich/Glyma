/* Insert Core Data  */

/* Disable All Constraints */
exec sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'

insert [dbo].[DescriptorTypes]([DescriptorTypeUid],[DescriptorTypeName])values('{96DA1782-058C-4F9B-BB1A-31B048F8C75A}', N'From')
insert [dbo].[DescriptorTypes]([DescriptorTypeUid],[DescriptorTypeName])values('{07C91D35-4DAC-431B-966B-64C924B8CDAB}', N'To')
insert [dbo].[DescriptorTypes]([DescriptorTypeUid],[DescriptorTypeName])values('{47107835-A485-4A23-BF0C-3FC631A07777}', N'TransclusionMap')
insert [dbo].[DescriptorTypes]([DescriptorTypeUid],[DescriptorTypeName])values('{8DDB6352-5A2B-430A-B549-7CE346DD7C3D}', N'TransclusionNode')

insert [dbo].[MetadataTypes]([MetadataTypeUid],[MetadataTypeName])values('{8D51FDF6-9D94-44F4-9C8C-79FB6F183760}', N'double')
insert [dbo].[MetadataTypes]([MetadataTypeUid],[MetadataTypeName])values('{C7628C1E-77C1-4A07-A2E8-8DE9F4E9803C}', N'string')
insert [dbo].[MetadataTypes]([MetadataTypeUid],[MetadataTypeName])values('{408C54DD-0452-44DC-8DED-96F542A43793}', N'datetime')
insert [dbo].[MetadataTypes]([MetadataTypeUid],[MetadataTypeName])values('{C98033E5-9BFC-4EC8-8B29-E60F6CAF1B00}', N'timespan')

insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{042E7E3B-8A5F-4A52-B1DD-3361A3ACD72A}', N'CompendiumArgumentNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{DA66B603-F6B3-4ECF-8ED0-AB34A288CF08}', N'CompendiumConNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{53EC78E3-F189-4340-B251-AAF9D78CF56D}', N'CompendiumDecisionNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{47B75628-7FDF-4440-BF35-8506D3FE6F2A}', N'CompendiumGenericNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{3B53600F-39EC-42FB-B08A-325062037130}', N'CompendiumIdeaNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{7D3C9B87-F31D-400F-A375-ABC0D1888625}', N'CompendiumListNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{B8C354CB-C7D0-4982-9A0F-6C4368FAB749}', N'CompendiumMapNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{84B7634B-DB8D-449B-B8CE-D3F3F80E95DD}', N'CompendiumNoteNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{084F38B7-115F-4AF6-9E30-D9D91226F86B}', N'CompendiumProNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{99FD1475-8099-45D3-BEDF-BEC396CCB4DD}', N'CompendiumQuestionNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{8F3DA942-06C4-4075-AD8B-B51361ABC900}', N'CompendiumReferenceNode')
insert [dbo].[NodeTypes]([NodeTypeUid],[NodeTypeName])values('{263754C2-2F31-4D21-B9C4-6509E00A5E94}', N'DomainNode')

insert [dbo].[RelationshipDescriptorPairs]([RelationshipTypeUid],[DescriptorTypeUid])values('{FE31AE41-5501-4B13-8F4A-AFE902A78F40}', '{96DA1782-058C-4F9B-BB1A-31B048F8C75A}')
insert [dbo].[RelationshipDescriptorPairs]([RelationshipTypeUid],[DescriptorTypeUid])values('{FE31AE41-5501-4B13-8F4A-AFE902A78F40}', '{07C91D35-4DAC-431B-966B-64C924B8CDAB}')

insert [dbo].[RelationshipTypes]([RelationshipTypeUid],[RelationshipTypeName])values('{9B64675F-D27B-42F1-BDAA-38D4697CC27A}', N'TransclusionFromToRelationship')
insert [dbo].[RelationshipTypes]([RelationshipTypeUid],[RelationshipTypeName])values('{4AFF46D7-87BE-48DD-B703-A93E38EF8FFB}', N'MapContainerRelationship')
insert [dbo].[RelationshipTypes]([RelationshipTypeUid],[RelationshipTypeName])values('{FE31AE41-5501-4B13-8F4A-AFE902A78F40}', N'FromToRelationship')

/* Enable All Constraints */
exec sp_msforeachtable 'ALTER TABLE ? CHECK CONSTRAINT all'
