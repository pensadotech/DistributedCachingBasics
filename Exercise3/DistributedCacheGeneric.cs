using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DistributedCacheExample.Exercise3;

/// <summary>
/// This example uses MS distributed cache. 
/// An importan observation, is that the get and set options uses json serialization. 
/// References:
///  https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache?view=net-8.0
///  https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-8.0 
///  
/// </summary>
public class DistributedCacheGeneric<T>
{
    IDistributedCache _distributedCache;

    // This initialize semaphores for a single thread at a time
    private static SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    public DistributedCacheGeneric(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T> GetOrCreateAsync(int index, string key, Func<T> createItem)
    {
        T cacheEntry;
        bool found;

        // Check if the value is already in the cache before acquiring the lock
        // for very fast systems this may never occur, but it is left here as an example
        // but this code could be removed, letting the second section handle the cache.
        (found, cacheEntry) = await TryGetValueAsync(key);
                
        if (found)
        {
            Console.WriteLine($"Agent # {index} - Avatar found in memory (before await)");
            return cacheEntry;
        }

        // each thread will wait for the semaphore to be avilable before proceeding
        await _lock.WaitAsync();

        try
        {
            // Check if the item exists in cache
            (found, cacheEntry) = await TryGetValueAsync(key);

            if (!found)
            {
                // Execute an external call, (e.g. DB or API)
                // this execute a defined lambda exppresion in sync fashion
                // storing the result in a variable type of T
                cacheEntry = await Task.FromResult(createItem());

                // Store the value in cache with a time-to-live (TTL) of 1 minute
                await SetAsync(key, cacheEntry, 1);

                Console.WriteLine($"Agent # {index} - Store new avatar in cache {cacheEntry}");

            }
            else
            {
                Console.WriteLine($"Agent # {index} - Avatar found in memory (after await)");
            }
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        { 
            _lock.Release(); 
        }    


        // Return value (based on T)
        return cacheEntry;
    }


    // Utility methods to access teh distributed cache.........................

    private async Task<(bool Found, T? Value)> TryGetValueAsync(string key)
    {
        var value = await GetAsync(key);

        return (value is not null, value);
    }

    private async Task<T?> GetAsync(string key)
    {
        // Return value from cache
        var cachedResult = await _distributedCache.GetStringAsync(key);

        return cachedResult == null ? default : DeserialiseFromString(cachedResult);
    }

    private async Task SetAsync(string key, T item, int minutesToCache)
    {
        var cacheEntryOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(10),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutesToCache)
        };

        var serialisedItemToCache = SerialiseForCaching(item);

        // Ste value on cache
        await _distributedCache.SetStringAsync(key, serialisedItemToCache, cacheEntryOptions);
    }

    private T? DeserialiseFromString(string cachedResult)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(cachedResult, new JsonSerializerOptions
            {
                MaxDepth = 10
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to deserialise from cached string");
            return default;
        }
    }

    private string? SerialiseForCaching(T item)
    {
        if (item == null)
            return null;

        try
        {
            return JsonSerializer.Serialize(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to serialise type '{0}' for caching", typeof(T).FullName);
            throw;
        }
    }


}
