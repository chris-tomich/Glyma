ALTER TABLE [dbo].[Transactions] DROP COLUMN [TransactionTimestamp]

ALTER TABLE [dbo].[Transactions] ADD [TransactionTimestamp] [datetime2] NULL

ALTER TABLE [dbo].[Transactions] ADD [RootMapParameterUid] [uniqueidentifier] NULL