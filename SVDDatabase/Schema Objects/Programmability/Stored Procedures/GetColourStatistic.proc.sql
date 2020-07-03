-- =============================================
-- Author:		Jay Adair
-- Create date: 03.02.2011
-- Description:	Retrieves stats for colours
-- =============================================
CREATE PROCEDURE GetColourStatistic
	-- Add the parameters for the stored procedure here
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    WITH cteCount AS (
    SELECT ColourID, COUNT(*) AS [Instances]
        FROM Vehicles
        WHERE ColourID IS NOT NULL
        GROUP BY ColourID
	),
	cteRank AS (
		SELECT 
			ColourID, 
			[Instances], 
			row_number() OVER (ORDER BY [Instances] DESC) AS [Rank]
			FROM cteCount
	)
	SELECT 
		ColourID, 
		[Instances],
		[Rank]
		FROM cteRank
		WHERE ColourID = @ID
END