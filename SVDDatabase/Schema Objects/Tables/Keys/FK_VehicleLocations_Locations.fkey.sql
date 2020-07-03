ALTER TABLE [dbo].[VehicleLocations]
    ADD CONSTRAINT [FK_VehicleLocations_Locations] FOREIGN KEY ([LocationID]) REFERENCES [dbo].[Locations] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

