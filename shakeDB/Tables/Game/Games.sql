CREATE TABLE [dbo].[Games]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [TypeId] INT NOT NULL, 
    [UserId] BIGINT NOT NULL, 
    [Year] INT NOT NULL, 
    [Day] INT NOT NULL, 
	[RollsTaken] int not null default 0,
    [isClosed] BIT NOT NULL DEFAULT 0, 
    [isWinningGame] INT NOT NULL DEFAULT 0, 
    [winAmount] INT NOT NULL DEFAULT 0, 
    [AppliedToAccount] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Games_Users_ID] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([FriendlyUserId]), 
    CONSTRAINT [FK_Games_GameType_Id] FOREIGN KEY ([TypeId]) REFERENCES [GameTypes]([Id])
)
