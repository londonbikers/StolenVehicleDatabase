-- =============================================
-- Author:		Jay Adair
-- Create date: 04.02.2011
-- Description:	Retrieves stats for a vehicle Manufacturer
-- =============================================
CREATE PROCEDURE [dbo].[GetManufacturerStatistic]
	-- Add the parameters for the stored procedure here
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	WITH cteCount AS (
    SELECT ManufacturerID, COUNT(*) AS [Instances]
        FROM Vehicles
        WHERE ManufacturerID IS NOT NULL
        GROUP BY ManufacturerID
	),
	cteRank AS (
		SELECT 
			ManufacturerID, 
			[Instances], 
			row_number() OVER (ORDER BY [Instances] DESC) AS [Rank]
			FROM cteCount
	)
	SELECT 
		ManufacturerID, 
		[Instances],
		[Rank]
		FROM cteRank
		WHERE ManufacturerID = @ID
END