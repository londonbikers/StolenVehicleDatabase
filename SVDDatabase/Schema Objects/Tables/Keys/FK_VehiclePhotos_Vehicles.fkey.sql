ALTER TABLE [dbo].[VehiclePhotos]
    ADD CONSTRAINT [FK_VehiclePhotos_Vehicles] FOREIGN KEY ([VehicleID]) REFERENCES [dbo].[Vehicles] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

