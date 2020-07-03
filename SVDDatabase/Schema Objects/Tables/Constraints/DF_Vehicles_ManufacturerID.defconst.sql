ALTER TABLE [dbo].[Vehicles]
    ADD CONSTRAINT [DF_Vehicles_ManufacturerID] DEFAULT ((0)) FOR [ManufacturerID];

