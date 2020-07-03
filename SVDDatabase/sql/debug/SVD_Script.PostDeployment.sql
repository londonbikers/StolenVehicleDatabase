/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- ASP.net Membership default values.
if (select COUNT(0) from dbo.aspnet_SchemaVersions) = 0 begin
	insert into dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) values ('common', 1, 1)
	insert into dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) values ('health monitoring', 1, 1)
	insert into dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) values ('membership', 1, 1)
	insert into dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) values ('personalization', 1, 1)
	insert into dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) values ('profile', 1, 1)
	insert into dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) values ('role manager', 1, 1)
end
if (select COUNT(0) from dbo.aspnet_Applications) = 0 begin
	insert into dbo.aspnet_Applications (ApplicationName, LoweredApplicationName, ApplicationId) values ('SVD', 'svd', 'B2E1C10C-9F11-42AA-824F-C10D7B3E0D69')
end
if (select COUNT(0) from dbo.aspnet_Roles) = 0 begin
	insert into dbo.aspnet_Roles (ApplicationId, RoleId, RoleName, LoweredRoleName) values ('B2E1C10C-9F11-42AA-824F-C10D7B3E0D69', '44F084BF-E9A7-4F65-8211-5A5EF8FFC631', 'Administrators', 'administrators')
end

-- Insert default reference data.
if (select COUNT(0) from dbo.VehicleSecurityTypes) = 0
begin
	insert into dbo.VehicleSecurityTypes ([Name]) values ('Alarm/Imobiliser')
	insert into dbo.VehicleSecurityTypes ([Name]) values ('Free-standing Chain')
	insert into dbo.VehicleSecurityTypes ([Name]) values ('Anchored Chain')
	insert into dbo.VehicleSecurityTypes ([Name]) values ('Disc/other lock')
	insert into dbo.VehicleSecurityTypes ([Name]) values ('Cover')
	insert into dbo.VehicleSecurityTypes ([Name]) values ('Tracker')
	insert into dbo.VehicleSecurityTypes ([Name]) values ('Garaged')
end

if (select COUNT(0) from dbo.TheftMethods) = 0
begin
	insert into dbo.TheftMethods ([Name]) values ('Removed from private property')
	insert into dbo.TheftMethods ([Name]) values ('Removed from public property')
	insert into dbo.TheftMethods ([Name]) values ('Hijacked')
	insert into dbo.TheftMethods ([Name]) values ('Hot-wired')
	insert into dbo.TheftMethods ([Name]) values ('Key theft')
	insert into dbo.TheftMethods ([Name]) values ('Deception')
end

if (select COUNT(0) from dbo.Colours) = 0
begin
	-- these are the top car colours: http://en.wikipedia.org/wiki/Car_colour_popularity
	insert into dbo.Colours ([Name]) values ('Silver')
	insert into dbo.Colours ([Name]) values ('White')
	insert into dbo.Colours ([Name]) values ('Gray')
	insert into dbo.Colours ([Name]) values ('Black')
	insert into dbo.Colours ([Name]) values ('Blue')
	insert into dbo.Colours ([Name]) values ('Red')
	insert into dbo.Colours ([Name]) values ('Light brown')
	insert into dbo.Colours ([Name]) values ('Green')
	insert into dbo.Colours ([Name]) values ('White pearl')
	insert into dbo.Colours ([Name]) values ('Yellow/gold')
end
