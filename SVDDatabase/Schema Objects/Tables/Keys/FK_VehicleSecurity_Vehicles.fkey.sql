ALTER TABLE [dbo].[VehicleSecurity]
    ADD CONSTRAINT [FK_VehicleSecurity_Vehicles] FOREIGN KEY ([VehicleID]) REFERENCES [dbo].[Vehicles] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

