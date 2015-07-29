ALTER TABLE [dbo].[Metadata] WITH CHECK ADD CONSTRAINT [FK_Metadata_Domains] FOREIGN KEY([DomainUid])
REFERENCES [dbo].[Domains] ([DomainUid])

ALTER TABLE [dbo].[Metadata] CHECK CONSTRAINT [FK_Metadata_Domains]


ALTER TABLE [dbo].[Metadata] WITH CHECK ADD CONSTRAINT [FK_Metadata_RootMapUid] FOREIGN KEY([RootMapUid])
REFERENCES [dbo].[Nodes] ([NodeUid])

ALTER TABLE [dbo].[Metadata] CHECK CONSTRAINT [FK_Metadata_RootMapUid]


ALTER TABLE [dbo].[Nodes] WITH CHECK ADD CONSTRAINT [FK_Nodes_RootMapUid] FOREIGN KEY([RootMapUid])
REFERENCES [dbo].[Nodes] ([NodeUid])

ALTER TABLE [dbo].[Nodes] CHECK CONSTRAINT [FK_Nodes_RootMapUid]


ALTER TABLE [dbo].[Relationships] WITH CHECK ADD CONSTRAINT [FK_Relationships_RootMapUid] FOREIGN KEY([RootMapUid])
REFERENCES [dbo].[Nodes] ([NodeUid])

ALTER TABLE [dbo].[Relationships] CHECK CONSTRAINT [FK_Relationships_RootMapUid]