ALTER TABLE [dbo].[Vehicles]
    ADD CONSTRAINT [DF_Vehicles_Status] DEFAULT ((0)) FOR [Status];

