/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
insert into GameTypes(Id,Name,RollsPerGame,MaxPlaysPerDay)
Values(0,'Shake of The Day Classic',3,1)

insert into Wallets(UserId, WalletValue)
values(-1,250)