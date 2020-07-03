-- =============================================
-- Author:		Jay Adair
-- Create date: 05.02.2011
-- Description:	Retrieves stats for a location
-- =============================================
CREATE PROCEDURE [dbo].[GetArea1Statistic]
	-- Add the parameters for the stored procedure here
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	WITH cteCount AS (
    SELECT LocationID, COUNT(*) AS [Instances]
        FROM VehicleLocations AS VL
        INNER JOIN Locations AS L ON L.ID = VL.LocationID
        WHERE L.[Type] = 1
        GROUP BY LocationID
	),
	cteRank AS (
		SELECT 
			LocationID, 
			[Instances], 
			ROW_NUMBER() OVER (ORDER BY [Instances] DESC) AS [Rank]
			FROM cteCount
	)
	SELECT 
		LocationID, 
		[Instances],
		[Rank]
		FROM cteRank
		WHERE LocationID = @ID
END