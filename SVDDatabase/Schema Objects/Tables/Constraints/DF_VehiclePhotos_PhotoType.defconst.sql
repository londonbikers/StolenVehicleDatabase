ALTER TABLE [dbo].[VehiclePhotos]
    ADD CONSTRAINT [DF_VehiclePhotos_PhotoType] DEFAULT ((0)) FOR [PhotoType];

