namespace SVD
{
	/// <summary>
	/// Defines how a class is instantiated.
	/// </summary>
	public enum ObjectMode
	{
		New,
		Existing
	}

    public enum PhotoType
    {
        HighResolution = 0,
        ProfilePicture = 1
    }

    public enum VehicleStatus
    {
        Active = 0,
        Retrieved = 1,
        Archived = 2
    }

    public enum PlaceType
    {
        Country = 0,
        AdministrativeAreaLevel1 = 1,
        /// <summary>
        /// City, normally.
        /// </summary>
        AdministrativeAreaLevel2 = 2,
        Locality = 3,
        SubLocality = 4,
        Route = 5
    }
}