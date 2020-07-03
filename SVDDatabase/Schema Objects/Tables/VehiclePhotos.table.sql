CREATE TABLE [dbo].[VehiclePhotos] (
    [ID]           INT             IDENTITY (1, 1) NOT NULL,
    [VehicleID]    INT             NOT NULL,
    [Filename]     NVARCHAR (255)  NOT NULL,
    [Comment]      NVARCHAR (1000) NULL,
    [DateModified] DATETIME        NOT NULL,
    [DateCreated]  DATETIME        NOT NULL,
    [PhotoType]    TINYINT         NOT NULL
);



