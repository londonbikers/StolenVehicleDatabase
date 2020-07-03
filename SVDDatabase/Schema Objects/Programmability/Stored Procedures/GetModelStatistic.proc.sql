-- =============================================
-- Author:		Jay Adair
-- Create date: 04.02.2011
-- Description:	Retrieves stats for a vehicle model
-- =============================================
CREATE PROCEDURE [dbo].[GetModelStatistic]
	-- Add the parameters for the stored procedure here
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	WITH cteCount AS (
    SELECT ModelID, COUNT(*) AS [Instances]
        FROM Vehicles
        WHERE ModelID IS NOT NULL
        GROUP BY ModelID
	),
	cteRank AS (
		SELECT 
			ModelID, 
			[Instances], 
			row_number() OVER (ORDER BY [Instances] DESC) AS [Rank]
			FROM cteCount
	)
	SELECT 
		ModelID, 
		[Instances],
		[Rank]
		FROM cteRank
		WHERE ModelID = @ID
END