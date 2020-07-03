ALTER TABLE [dbo].[Vehicles]
    ADD CONSTRAINT [FK_Vehicles_TheftMethods] FOREIGN KEY ([TheftMethodID]) REFERENCES [dbo].[TheftMethods] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

