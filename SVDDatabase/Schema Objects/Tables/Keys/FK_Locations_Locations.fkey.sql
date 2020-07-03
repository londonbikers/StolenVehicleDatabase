ALTER TABLE [dbo].[Locations]
    ADD CONSTRAINT [FK_Locations_Locations] FOREIGN KEY ([ParentLocationId]) REFERENCES [dbo].[Locations] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

