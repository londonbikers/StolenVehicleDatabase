ALTER TABLE [dbo].[Vehicles]
    ADD CONSTRAINT [DF_Vehicles_DateModified] DEFAULT (getdate()) FOR [DateModified];

