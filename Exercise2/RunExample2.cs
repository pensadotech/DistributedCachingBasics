using DistributedCacheExample.DataAccess;
using DistributedCacheExample.Exercise1;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DistributedCacheExample.Exercise2;

public class RunExample2
{
    IDistributedCache _distributedCache;

    public RunExample2(IDistributedCache distributedCache)
    {
        // Get a reference to the distributed cache
        _distributedCache = distributedCache;
    }

    public async void Execute()
    {
        // Userid to retreive an avatar.
        string userId = "exp002";

        // The pretend DB holds the users' avaaters
        var _database = new PretendDatabase();

        // Initialize auxilary class that will handle the cache with options
        // and a semaphore to avoid multiple threads accessing the cache at the same time.
        var _cacheHdlr = new DistributedCacheHandler(_distributedCache);

        // Consider several taks to simulate a mutli-threding scenario
        List<Task> tasks = new List<Task>();

        Console.WriteLine("Starte three concurrent accesses");

        // Simulate multiple simultanous retreivals from the cache
        for (int i = 0; i < 3; i++)
        {
            int index = i; // Capture the loop variable

            // Muti-thred task to access the cache
            Task<string> task = Task.Run(async () =>
            {
                Console.WriteLine($"Agent # {index} - Started ");

                // Call the method inside the cache, pasing user id and Functin expresion that will access
                // the database for retreiving the avatar
                string myAvatar = await _cacheHdlr.GetOrCreateAsync(index, userId, () => _database.GetAvatar(index, userId));

                Console.WriteLine($"Agent # {index} final Avatar: {myAvatar}");

                return myAvatar;
            });
        }

        // Wait for a second, to allow all the tasks to start and block.
        Thread.Sleep(1000);

        // wait for all task to complete
        Console.WriteLine("Waiting for all agents tasks to complete...");
        await Task.WhenAll(tasks);
        // Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10)); // This is an alternative for waiting for all atsk to complete

        Console.WriteLine("All tasks completed!");
    }

}
