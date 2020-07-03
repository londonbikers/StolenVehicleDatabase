ALTER TABLE [dbo].[VehicleSecurity]
    ADD CONSTRAINT [FK_VehicleSecurity_VehicleSecurityTypes] FOREIGN KEY ([TypeID]) REFERENCES [dbo].[VehicleSecurityTypes] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

