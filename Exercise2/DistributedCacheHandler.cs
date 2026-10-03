using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedCacheExample.Exercise2;

/// <summary>
/// This example uses MS distributed cache.
/// An importan observation, is that the get and set options handle Bytes. 
/// References:
///  https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache?view=net-8.0
///  https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-8.0 
///  
/// </summary>
public class DistributedCacheHandler
{
    IDistributedCache _distributedCache;

    // This initialize semaphores for a single thread at a time
    private static SemaphoreSlim _lock = new SemaphoreSlim(1, 1); 

    public DistributedCacheHandler(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<string> GetOrCreateAsync(int index, string key, Func<string> createItem)
    {
        string cacheEntry;
        byte[]? cachedValueInBytes;

        // Check if the value is already in the cache before acquiring the lock
        // for very fast systems this may never occur, but it is left here as an example
        // but this code could be removed, letting the second section handle the cache.
        cachedValueInBytes = _distributedCache.Get(key);

        if (cachedValueInBytes != null)
        {
            // If exists, retrieve the value
            cacheEntry = Encoding.UTF8.GetString(cachedValueInBytes);
            Console.WriteLine($"Agent # {index} - Avatar found in memory (before await)");
            return cacheEntry;
        }

        // each thread will wait for the semaphore to be avilable before proceeding
        await _lock.WaitAsync();

        try
        {
            // Check if the item exists in cache
            cachedValueInBytes = _distributedCache.Get(key);

            if (cachedValueInBytes != null)
            {
                // If exists, retrieve the value
                cacheEntry = Encoding.UTF8.GetString(cachedValueInBytes);
                Console.WriteLine($"Agent # {index} - Avatar found in memory (after await)");
            }
            else
            {
                // Execute an external call, (e.g. DB or API)
                // this execute a defined lambda exppresion in sync fashion
                // storing the result in a variable type of T
                cacheEntry = await Task.FromResult(createItem());

                // Convert retreive string data to bytes
                cachedValueInBytes = Encoding.UTF8.GetBytes(cacheEntry);

                // IMPORTANT: In order to avoid hitting the external resources frequently
                // it is cirical to set the proper expiration times for the cache value
                // by default the expiration is quite brief. 
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromSeconds(10),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20)
                };

                // Store the value in cache with a time-to-live (TTL) of 1 minute
                _distributedCache.Set(key, cachedValueInBytes, cacheEntryOptions);

                Console.WriteLine($"Agent # {index} - Store new avatar in cache {cacheEntry}");

            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            // After the thread finishes, it must release teh semaphore to 
            // allow other threads to retreive the value stored in teh cache
            _lock.Release();
        }

        // Return value (as string)
        return cacheEntry;
    }


}
