// Copyright (c) Reality Collective. All rights reserved.
#if UNITY_2021_1_OR_NEWER

using System.Collections.Generic;
using UnityEngine;

namespace RealityCollective.Utilities
{
    /// <summary>
    /// Static cache for WaitForSeconds objects to avoid allocation overhead.
    /// Commonly used durations are pre-cached for maximum performance.
    /// </summary>
    public static class WaitForSecondsCache
    {
        #region Pre-cached Common Durations
        
        /// <summary>0.01 seconds wait - commonly used for frame-precise delays</summary>
        public static readonly WaitForSeconds OneHundredthSecond = new(0.01f);
        
        /// <summary>0.1 seconds wait - commonly used for short delays</summary>
        public static readonly WaitForSeconds TenthSecond = new(0.1f);
        
        /// <summary>0.25 seconds wait - commonly used for quarter-second delays</summary>
        public static readonly WaitForSeconds QuarterSecond = new(0.25f);
        
        /// <summary>0.5 seconds wait - commonly used for half-second delays</summary>
        public static readonly WaitForSeconds HalfSecond = new(0.5f);
        
        /// <summary>1 second wait - commonly used for one-second delays</summary>
        public static readonly WaitForSeconds OneSecond = new(1f);
        
        /// <summary>3 seconds wait - commonly used for loading screens</summary>
        public static readonly WaitForSeconds ThreeSeconds = new(3f);

        #endregion Pre-cached Common Durations

        #region Dynamic Cache

        private static readonly Dictionary<float, WaitForSeconds> _cache = new();
        private const int MAX_CACHE_SIZE = 100; // Prevent memory bloat from too many cached instances

        /// <summary>
        /// Gets a cached WaitForSeconds instance for the specified duration.
        /// If the duration matches a pre-cached common value, returns the pre-cached instance.
        /// Otherwise, creates and caches a new instance for future reuse.
        /// </summary>
        /// <param name="seconds">The duration in seconds to wait</param>
        /// <returns>A cached WaitForSeconds instance</returns>
        public static WaitForSeconds Get(float seconds)
        {
            // Return pre-cached common durations for maximum performance
            switch (seconds)
            {
                case 0.01f: return OneHundredthSecond;
                case 0.1f: return TenthSecond;
                case 0.25f: return QuarterSecond;
                case 0.5f: return HalfSecond;
                case 1f: return OneSecond;
                case 3f: return ThreeSeconds;
            }

            // Check dynamic cache
            if (_cache.TryGetValue(seconds, out WaitForSeconds cachedWait))
            {
                return cachedWait;
            }

            // Create new instance and cache it (if we haven't exceeded cache limit)
            var newWait = new WaitForSeconds(seconds);
            
            if (_cache.Count < MAX_CACHE_SIZE)
            {
                _cache[seconds] = newWait;
            }

            return newWait;
        }

        /// <summary>
        /// Clears the dynamic cache. Pre-cached common durations are not affected.
        /// Use this if you need to free memory from dynamically cached durations.
        /// </summary>
        public static void ClearDynamicCache()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Gets the current number of dynamically cached WaitForSeconds instances.
        /// Does not include pre-cached common durations.
        /// </summary>
        public static int DynamicCacheCount => _cache.Count;

        #endregion Dynamic Cache
    }
}
#endif
