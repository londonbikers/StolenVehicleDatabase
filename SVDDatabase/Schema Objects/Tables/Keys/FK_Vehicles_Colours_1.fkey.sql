﻿ALTER TABLE [dbo].[Vehicles]
    ADD CONSTRAINT [FK_Vehicles_Colours] FOREIGN KEY ([ColourID]) REFERENCES [dbo].[Colours] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

