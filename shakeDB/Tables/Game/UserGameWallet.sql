CREATE TABLE [dbo].[UserGameWallet]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] BIGINT NOT NULL, 
    [Wallet] INT NOT NULL DEFAULT 0, 
    [TimesBoughtIn] INT NOT NULL DEFAULT 0, 
    [Created] DATETIME NOT NULL DEFAULT GetDate()
)
