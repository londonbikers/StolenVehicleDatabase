using System.Runtime.Caching;

namespace SVD
{
	/// <summary>
	/// Manages domain-object caching via the use of the inbuilt Framework 4 memory cache.
	/// </summary>
	public class CacheManager
	{
        #region accessors
        /// <summary>
        /// Provides direct access to the cache. Useful for enumerating it.
        /// </summary>
        public ObjectCache Cache { get; }
	    #endregion

        #region constructors
        internal CacheManager()
		{
            // note that you can create multiple MemoryCache(s) inside a single application. 
            Cache = MemoryCache.Default;
		}
		#endregion

        #region public methods
        internal void Add(string key, object value)
        {
            if (Cache.Contains(key)) return;
            var cip = new CacheItemPolicy();
            Cache.Add(key, value, cip);
        }

        internal object Get(string key)
        {
            return Cache.Get(key);
        }

        internal void Remove(string key)
        {
            Cache.Remove(key);
        }
        #endregion
    }
}