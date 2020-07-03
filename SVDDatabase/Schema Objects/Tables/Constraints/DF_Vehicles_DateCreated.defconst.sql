ALTER TABLE [dbo].[Vehicles]
    ADD CONSTRAINT [DF_Vehicles_DateCreated] DEFAULT (getdate()) FOR [DateCreated];

