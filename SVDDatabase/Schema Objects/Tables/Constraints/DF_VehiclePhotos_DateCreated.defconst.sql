ALTER TABLE [dbo].[VehiclePhotos]
    ADD CONSTRAINT [DF_VehiclePhotos_DateCreated] DEFAULT (getdate()) FOR [DateCreated];

