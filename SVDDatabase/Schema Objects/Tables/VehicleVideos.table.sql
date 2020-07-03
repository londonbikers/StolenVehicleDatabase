CREATE TABLE [dbo].[VehicleVideos] (
    [ID]        INT             IDENTITY (1, 1) NOT NULL,
    [VehicleID] INT             NOT NULL,
    [Url]       VARCHAR (255)   NOT NULL,
    [Comment]   NVARCHAR (1000) NULL
);





