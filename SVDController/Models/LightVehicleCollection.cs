using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SVD.Models
{
    /// <summary>
    /// Contains a collection of Vehicle objects.
    /// </summary>
    public class LightVehicleCollection : IEnumerable<Vehicle>
    {
        #region members
		private readonly List<int> _list;
		private readonly bool _validateActions;
		#endregion

		#region accessors
		/// <summary>
		/// Returns the number of vehicles in the collection.
		/// </summary>
		public int Count => _list.Count;

        /// <summary>
        /// The total non-paged number of items available for the query used to populate this collection.
        /// </summary>
        public int TotalCount { get; set; }
		#endregion

		#region constructors
		/// <summary>
		/// Creates a new LightVehicleCollection object.
		/// </summary>
		internal LightVehicleCollection() 
		{
			_list = new List<int>();
			_validateActions = false;
		}
        
		/// <summary>
        /// Creates a new LightVehicleCollection object.
		/// </summary>
		/// <param name="validateActions">
		/// Determines whether or not all additions or removals are validated. Potentially slower for large collections but of use to consumers
		/// that need to know if an item being added is a duplicate, for instance.
		/// </param>
        internal LightVehicleCollection(bool validateActions) 
		{
			_list = new List<int>();
			_validateActions = validateActions;
		}
		#endregion

		#region public methods
		/// <summary>
		/// Adds a Vehicle to the collection.
		/// </summary>
		public virtual bool Add(Vehicle vehicle) 
		{
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            return Add(vehicle.Id);
		}
        
		/// <summary>
		/// Adds a Vehicle to the collection.
		/// </summary>
		/// <param name="vehicleId">The ID for the vehicle being added.</param>
		public virtual bool Add(int vehicleId) 
		{
            if (vehicleId < 1)
                throw new ArgumentNullException(nameof(vehicleId));

			if (_validateActions)
			{
                if (_list.Any(id => id == vehicleId))
				    return false;

                _list.Add(vehicleId);
				return true;
			}

            _list.Add(vehicleId);
		    return true;
		}
        
		/// <summary>
		/// Removes a Vehicle from the collection.
		/// </summary>
		public virtual bool Remove(int vehicleId) 
		{
			for (var i = 0; i < _list.Count; i++)
			{
                if (_list[i] != vehicleId) continue;
			    _list.RemoveAt(i);
			    return true;
			}

			return false;
		}
        
		/// <summary>
		/// Removes a Vehicle from the collection.
		/// </summary>
		public virtual bool Remove(Vehicle vehicle) 
		{
            return Remove(vehicle.Id);
		}
        
		/// <summary>
		/// Determines whether or not the collection contains a specific Vehicle.
		/// </summary>
        public bool Contains(Vehicle vehicle)
		{
            return _list.Any(id => id == vehicle.Id);
		}

	    /// <summary>
		/// Public default indexer.
		/// </summary>
		public Vehicle this[int index] 
		{
			get 
			{
				if (index > _list.Count - 1)				
					throw new IndexOutOfRangeException();

			    return Controller.Instance.VehicleController.GetVehicle(_list[index]);
			}
		}

        IEnumerator<Vehicle> IEnumerable<Vehicle>.GetEnumerator()
        {
            return _list.Select(id => Controller.Instance.VehicleController.GetVehicle(id)).GetEnumerator();
        }

        /// <summary>
		/// Returns an enumerator for the collection.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
            return _list.Select(id => Controller.Instance.VehicleController.GetVehicle(id)).GetEnumerator();
		}

		/// <summary>
		/// Clears the entire contents of the collection.
		/// </summary>
		internal void Clear() 
		{
			_list.Clear();
		}
		#endregion
    }
}
