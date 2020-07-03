-- =============================================
-- Author:		Jay Adair
-- Create date: 04.02.2011
-- Description:	Retrieves stats for a theft-method
-- =============================================
CREATE PROCEDURE [dbo].[GetTheftMethodStatistic]
	-- Add the parameters for the stored procedure here
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	WITH cteCount AS (
    SELECT TheftMethodID, COUNT(*) AS [Instances]
        FROM Vehicles
        WHERE TheftMethodID IS NOT NULL
        GROUP BY TheftMethodID
	),
	cteRank AS (
		SELECT 
			TheftMethodID, 
			[Instances], 
			row_number() OVER (ORDER BY [Instances] DESC) AS [Rank]
			FROM cteCount
	)
	SELECT 
		TheftMethodID, 
		[Instances],
		[Rank]
		FROM cteRank
		WHERE TheftMethodID = @ID
END