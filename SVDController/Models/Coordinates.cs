namespace SVD.Models
{
	/// <summary>
	/// Represents a location in the world.
	/// </summary>
	public struct Coordinates
	{
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		/// <summary>
		/// Low number = poor accuracy.
		/// </summary>
		public int Accuracy { get; set; }
	}
}