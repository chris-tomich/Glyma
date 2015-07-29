/* Setup Descriptors Table */
ALTER TABLE [dbo].[Descriptors]  WITH CHECK ADD  CONSTRAINT [FK_Descriptors_DescriptorTypes] FOREIGN KEY([NodeUid])
REFERENCES [dbo].[Nodes] ([NodeUid])


ALTER TABLE [dbo].[Descriptors] CHECK CONSTRAINT [FK_Descriptors_DescriptorTypes]


ALTER TABLE [dbo].[Descriptors]  WITH CHECK ADD  CONSTRAINT [FK_Descriptors_DescriptorTypes1] FOREIGN KEY([DescriptorTypeUid])
REFERENCES [dbo].[DescriptorTypes] ([DescriptorTypeUid])


ALTER TABLE [dbo].[Descriptors] CHECK CONSTRAINT [FK_Descriptors_DescriptorTypes1]


ALTER TABLE [dbo].[Descriptors]  WITH CHECK ADD  CONSTRAINT [FK_Descriptors_Relationships] FOREIGN KEY([RelationshipUid])
REFERENCES [dbo].[Relationships] ([RelationshipUid])


ALTER TABLE [dbo].[Descriptors] CHECK CONSTRAINT [FK_Descriptors_Relationships]


/* Setup Metadata Table */
ALTER TABLE [dbo].[Metadata]  WITH CHECK ADD  CONSTRAINT [FK_Metadata_Descriptors] FOREIGN KEY([DescriptorTypeUid])
REFERENCES [dbo].[DescriptorTypes] ([DescriptorTypeUid])


ALTER TABLE [dbo].[Metadata] CHECK CONSTRAINT [FK_Metadata_Descriptors]


ALTER TABLE [dbo].[Metadata]  WITH CHECK ADD  CONSTRAINT [FK_Metadata_MetadataTypes] FOREIGN KEY([MetadataTypeUid])
REFERENCES [dbo].[MetadataTypes] ([MetadataTypeUid])


ALTER TABLE [dbo].[Metadata] CHECK CONSTRAINT [FK_Metadata_MetadataTypes]


ALTER TABLE [dbo].[Metadata]  WITH CHECK ADD  CONSTRAINT [FK_Metadata_Nodes] FOREIGN KEY([NodeUid])
REFERENCES [dbo].[Nodes] ([NodeUid])


ALTER TABLE [dbo].[Metadata] CHECK CONSTRAINT [FK_Metadata_Nodes]


ALTER TABLE [dbo].[Metadata]  WITH CHECK ADD  CONSTRAINT [FK_Metadata_Relationships] FOREIGN KEY([RelationshipUid])
REFERENCES [dbo].[Relationships] ([RelationshipUid])


ALTER TABLE [dbo].[Metadata] CHECK CONSTRAINT [FK_Metadata_Relationships]


ALTER TABLE [dbo].[Metadata] ADD  CONSTRAINT [DF_Metadata_MetadataId]  DEFAULT (newid()) FOR [MetadataId]


ALTER TABLE [dbo].[Metadata] WITH CHECK ADD CONSTRAINT [FK_Metadata_Domains] FOREIGN KEY([DomainUid])
REFERENCES [dbo].[Domains] ([DomainUid])

ALTER TABLE [dbo].[Metadata] CHECK CONSTRAINT [FK_Metadata_Domains]


ALTER TABLE [dbo].[Metadata] WITH CHECK ADD CONSTRAINT [FK_Metadata_RootMapUid] FOREIGN KEY([RootMapUid])
REFERENCES [dbo].[Nodes] ([NodeUid])

ALTER TABLE [dbo].[Metadata] CHECK CONSTRAINT [FK_Metadata_RootMapUid]


/* Setup Nodes Table */
ALTER TABLE [dbo].[Nodes]  WITH CHECK ADD  CONSTRAINT [FK_Nodes_Domains] FOREIGN KEY([DomainUid])
REFERENCES [dbo].[Domains] ([DomainUid])


ALTER TABLE [dbo].[Nodes] CHECK CONSTRAINT [FK_Nodes_Domains]


ALTER TABLE [dbo].[Nodes]  WITH CHECK ADD  CONSTRAINT [FK_Nodes_NodeTypes] FOREIGN KEY([NodeTypeUid])
REFERENCES [dbo].[NodeTypes] ([NodeTypeUid])


ALTER TABLE [dbo].[Nodes] CHECK CONSTRAINT [FK_Nodes_NodeTypes]

ALTER TABLE [dbo].[Nodes] WITH CHECK ADD CONSTRAINT [FK_Nodes_RootMapUid] FOREIGN KEY([RootMapUid])
REFERENCES [dbo].[Nodes] ([NodeUid])

ALTER TABLE [dbo].[Nodes] CHECK CONSTRAINT [FK_Nodes_RootMapUid]


/* Setup RelationshipDescriptorPairs Table */
ALTER TABLE [dbo].[RelationshipDescriptorPairs]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipDescriptorPairs_DescriptorTypes] FOREIGN KEY([DescriptorTypeUid])
REFERENCES [dbo].[DescriptorTypes] ([DescriptorTypeUid])


ALTER TABLE [dbo].[RelationshipDescriptorPairs] CHECK CONSTRAINT [FK_RelationshipDescriptorPairs_DescriptorTypes]


ALTER TABLE [dbo].[RelationshipDescriptorPairs]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipDescriptorPairs_RelationshipTypes] FOREIGN KEY([RelationshipTypeUid])
REFERENCES [dbo].[RelationshipTypes] ([RelationshipTypeUid])


ALTER TABLE [dbo].[RelationshipDescriptorPairs] CHECK CONSTRAINT [FK_RelationshipDescriptorPairs_RelationshipTypes]


/* Setup Relationships Table */
ALTER TABLE [dbo].[Relationships]  WITH CHECK ADD  CONSTRAINT [FK_Relationships_Domains] FOREIGN KEY([DomainUid])
REFERENCES [dbo].[Domains] ([DomainUid])


ALTER TABLE [dbo].[Relationships] CHECK CONSTRAINT [FK_Relationships_Domains]


ALTER TABLE [dbo].[Relationships]  WITH CHECK ADD  CONSTRAINT [FK_Relationships_RelationshipTypes] FOREIGN KEY([RelationshipTypeUid])
REFERENCES [dbo].[RelationshipTypes] ([RelationshipTypeUid])


ALTER TABLE [dbo].[Relationships] CHECK CONSTRAINT [FK_Relationships_RelationshipTypes]

ALTER TABLE [dbo].[Relationships] WITH CHECK ADD CONSTRAINT [FK_Relationships_RootMapUid] FOREIGN KEY([RootMapUid])
REFERENCES [dbo].[Nodes] ([NodeUid])

ALTER TABLE [dbo].[Relationships] CHECK CONSTRAINT [FK_Relationships_RootMapUid]