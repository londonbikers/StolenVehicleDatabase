using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SVD.Models;

namespace SVD.Background_Tasks
{
    public class InstanceStatsProcessor
    {
        #region members
        private delegate void ProcessorDelegate();
        #endregion

        #region constructors
        public InstanceStatsProcessor()
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
                // get all objects in the cache and zap their simple statistic property caches so they're rebuilt the next time they're requested.
                foreach (var model in from item in Controller.Instance.CacheManager.Cache
                        where (item.Value as BaseModel)?.Statistic != null
                        select item.Value as BaseModel)
                {
                    model.Statistic = null;
                }

                // zap all sat objects on the VDWS objects.
                foreach (var mw in Controller.Instance.MakesAndModelsController.VehicleManufacturers)
                    mw.Statistic = null;

                foreach (var mw in Controller.Instance.MakesAndModelsController.VehicleModels)
                    mw.Statistic = null;
                
                Debug.WriteLine("Reset instance stats.");
                Thread.Sleep(TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["BackgroundTasks.InstanceStatsProcessIntervalMins"])));
            }
        }
        #endregion
    }
}
