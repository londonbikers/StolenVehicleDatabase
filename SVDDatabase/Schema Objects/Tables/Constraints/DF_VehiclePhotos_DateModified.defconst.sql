ALTER TABLE [dbo].[VehiclePhotos]
    ADD CONSTRAINT [DF_VehiclePhotos_DateModified] DEFAULT (getdate()) FOR [DateModified];

