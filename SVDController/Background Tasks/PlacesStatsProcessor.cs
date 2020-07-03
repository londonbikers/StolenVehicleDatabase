using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using SVD.Models;

namespace SVD.Background_Tasks
{
    /// <summary>
    /// Queries the database and generates statistics for Place objects in the Cache.
    /// </summary>
    public class PlacesStatsProcessor
    {
        #region members
        private delegate void ProcessorDelegate();
        #endregion

        #region constructors
        public PlacesStatsProcessor()
        {
            var myAction = new ProcessorDelegate(StartWork);
            myAction.BeginInvoke(null, null);
        }
        #endregion

        #region private methods
        private static void StartWork()
        {
            while (true)
            {
                // get all Place objects in the cache and zap their simple statistic property caches so they're rebuilt the next time they're requested.
                foreach (var place in from item in Controller.Instance.CacheManager.Cache
                            where item.Key.StartsWith(typeof(Place).FullName)
                            select item.Value as Place)
                {
                    place.SimpleStats = null;
                }

                Thread.Sleep(TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["BackgroundTasks.PlaceStatsProcessIntervalMins"])));
            }
        }
        #endregion
    }
}
