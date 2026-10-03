using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedCacheExample.Exercise1;

public class RunExample1
{
    IDistributedCache _distributedCache;

    // Dependency injection of the IDistributedCache interface to use the MS Distributed caching library
    public RunExample1(IDistributedCache distributedCache) // recevies MS Distribute caching library 
    {
        // Get a reference to the distributed cache
        _distributedCache = distributedCache;
    }

    public void Execute()
    {
        // Key for the cached item
        string key = "myKey";

        // Check if the item exists in cache
        byte[]? cachedValue = _distributedCache.Get(key);

        if (cachedValue != null)
        {
            // If exists, retrieve from the cache 
            string value = System.Text.Encoding.UTF8.GetString(cachedValue);
            // Display the value
            Console.WriteLine("Retreived from the cache: " + value);
        }
        else
        {
            // If not exists, convert to bytes
            string value = "Example abc1234";
            byte[]? bytes = Encoding.UTF8.GetBytes(value);

            // Store the value in cache with a time-to-live (TTL) of 1 minute
            // the Cache options was created inside the Set method
            _distributedCache.Set(key, bytes, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

            Console.WriteLine("Stored in the cache: " + value);

        }
    }

} 
