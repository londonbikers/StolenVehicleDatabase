ALTER TABLE [dbo].[VehicleLocations]
    ADD CONSTRAINT [FK_VehicleLocations_Vehicles] FOREIGN KEY ([VehicleID]) REFERENCES [dbo].[Vehicles] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

