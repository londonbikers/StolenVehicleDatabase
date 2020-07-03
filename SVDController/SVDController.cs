using System;
using System.Collections;
using SVD.Background_Tasks;
using SVD.Controllers;

namespace SVD
{
    /// <summary>
    /// The main interface with consuming clients, i.e. the web-application.
    /// </summary>
	public class Controller
	{
		#region members
        private static Controller _instance;
	    #endregion

		#region accessors
		public static Controller Instance => _instance ?? (_instance = new Controller());
	    internal Repository Repository { get; }
	    internal CacheManager CacheManager { get; }
	    internal StatisticsController StatisticsController { get; }
	    public VehicleController VehicleController { get; }
	    public PlacesController PlacesController { get; }
	    public MakesAndModelsController MakesAndModelsController { get; }

	    #endregion

		#region constructors
		private Controller()
		{
			Repository = new Repository();
			CacheManager = new CacheManager();
			VehicleController = new VehicleController();
		    PlacesController = new PlacesController();
            MakesAndModelsController = new MakesAndModelsController();
		    StatisticsController = new StatisticsController();
		}
		#endregion

        #region private methods
        /// <summary>
        /// Performs any start-up procedures necessary, asynchronously.
        /// </summary>
        public void Initialise()
        {
            // this was async but so much of the site relies on the data that it wasn't worth it.
            MakesAndModelsController.RetrieveHierarchy();

            // background tasks.
            new ArrayList { 
                new PlacesStatsProcessor(), 
                new InstanceStatsProcessor() };
        }

        public void Log(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }
        #endregion
    }
}