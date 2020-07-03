using System;
using System.Diagnostics;

namespace SVD.Models
{
	[Serializable]
	public abstract class BaseModel
	{
		#region members
	    private Statistic _statistic;
		#endregion

		#region accessors
		public int Id { get; set; }
		public bool IsPersisted { get; set; }
		public DateTime DateModified { get; set; }
		public DateTime DateCreated { get; set; }
        public Type DerivedType { get; set; }
        public Statistic Statistic
        {
            get
            {
                if (_statistic != null)
                    return _statistic;

                Debug.WriteLine("Generating stats.");
                _statistic = Controller.Instance.StatisticsController.BuildStatistic(this);
                return _statistic;
            }
            internal set { _statistic = value; }
        }
		internal string CacheKey 
		{ 
			get 
			{
				if (!IsPersisted)
					throw new Exception("Object not persisted. Not appropriate for caching as the key won't be unique against other non-persisted objects of this type.");

				return $"{DerivedType.FullName}-{Id}"; 
			} 
		}
		#endregion

		#region contructors
	    protected BaseModel(Type derivedType, ObjectMode mode)
		{
			DateCreated = DateTime.Now;
			DateModified = DateTime.Now;
			DerivedType = derivedType;
			IsPersisted = mode != ObjectMode.New; 
		}
		#endregion

		#region public methods
		public virtual bool IsValid()
		{
			return true;
		}

		/// <summary>
		/// Clears any object-specific caches and causes them to be reloaded upon next use.
		/// </summary>
		/// <remarks>
		/// Good for reloading in-memory caches when dependencies have been updated.
		/// </remarks>
		public virtual void Reset()
		{
		}
		#endregion

		#region protected methods
		protected static string GenerateCacheKey(Type derivedType, int id)
		{
			return string.Format("{0}-{1}", derivedType.FullName, id);
		}
		#endregion
	}
}