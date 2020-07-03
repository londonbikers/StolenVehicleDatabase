ALTER TABLE [dbo].[VehicleVideos]
    ADD CONSTRAINT [FK_VehicleVideos_Vehicles] FOREIGN KEY ([VehicleID]) REFERENCES [dbo].[Vehicles] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

