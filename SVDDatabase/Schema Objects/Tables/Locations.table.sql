CREATE TABLE [dbo].[Locations] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [Code]             VARCHAR (50)   NULL,
    [Type]             TINYINT        NOT NULL,
    [Latitude]         FLOAT          NULL,
    [Longitude]        FLOAT          NULL,
    [ParentLocationId] INT            NULL
);





