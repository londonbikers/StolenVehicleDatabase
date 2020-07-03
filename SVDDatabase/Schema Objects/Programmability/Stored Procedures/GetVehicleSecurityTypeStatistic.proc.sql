-- =============================================
-- Author:		Jay Adair
-- Create date: 04.02.2011
-- Description:	Retrieves stats for a vehicle security type.
-- =============================================
CREATE PROCEDURE [dbo].[GetVehicleSecurityTypeStatistic]
	-- Add the parameters for the stored procedure here
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	WITH cteCount AS (
    SELECT TypeID, COUNT(*) AS [Instances]
        FROM VehicleSecurity
        GROUP BY TypeID
	),
	cteRank AS (
		SELECT 
			TypeID, 
			[Instances], 
			row_number() OVER (ORDER BY [Instances] DESC) AS [Rank]
			FROM cteCount
	)
	SELECT 
		TypeID, 
		[Instances],
		[Rank]
		FROM cteRank
		WHERE TypeID = @ID
END