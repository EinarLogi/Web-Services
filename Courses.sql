﻿CREATE TABLE [dbo].[Table]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NCHAR(55) NOT NULL, 
    [StartDate] DATETIME NOT NULL, 
    [EndDate] DATETIME NOT NULL, 
    [Semester] NCHAR(15) NOT NULL
)