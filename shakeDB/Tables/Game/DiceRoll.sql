CREATE TABLE [dbo].[DieRolls]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] BIGINT NOT NULL, 
    [RollValue] NCHAR(10) NOT NULL DEFAULT -1, 
	[GameId] bigint not null,
	[GameRollNumber] int not null,
    [CreatedDate] DATETIME NOT NULL DEFAULT GetDate(), 
    CONSTRAINT [FK_DieRolls_AspNetUsers_Id] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([FriendlyUserId])
)
