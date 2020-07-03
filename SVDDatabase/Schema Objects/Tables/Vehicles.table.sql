CREATE TABLE [dbo].[Vehicles] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [MemberUID]        UNIQUEIDENTIFIER NOT NULL,
    [ManufacturerID]   INT              NOT NULL,
    [ModelID]          INT              NOT NULL,
    [VehicleTypeId]    INT              NOT NULL,
    [ColourID]         INT              NULL,
    [Year]             INT              NULL,
    [Registration]     VARCHAR (50)     NULL,
    [VIN]              CHAR (17)        NULL,
    [EngineNumber]     CHAR (17)        NULL,
    [Description]      NTEXT            NULL,
    [PoliceForce]      VARCHAR (255)    NULL,
    [PolicePhoneNo]    VARCHAR (20)     NULL,
    [PoliceReference]  VARCHAR (255)    NULL,
    [TheftDescription] NTEXT            NULL,
    [TheftLongitude]   DECIMAL (18, 6)  NULL,
    [TheftLatitude]    DECIMAL (18, 6)  NULL,
    [IsLocationApprox] BIT              NULL,
    [TheftDate]        DATETIME         NULL,
    [TheftMethodID]    INT              NULL,
    [DateModified]     DATETIME         NOT NULL,
    [DateCreated]      DATETIME         NOT NULL,
    [Status]           TINYINT          NOT NULL
);













