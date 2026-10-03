
using DistributedCacheExample.Exercise1;
using DistributedCacheExample.Exercise2;
using DistributedCacheExample.Exercise3;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Distributed Caching example!");


// Adding DI to console program 

// Setup configuration
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

// Setup DI container
IServiceCollection services = new ServiceCollection();

// add services
services.AddDistributedMemoryCache();    // add MS distributed cache libraries
services.AddOptions();
services.AddSingleton<IConfiguration>(configuration);


// Create a service provider to resolve dependencies
ServiceProvider? serviceProvider = services.BuildServiceProvider();

// Get from the DI container the MS distributed cache library
// to be injected into the examples, below
IDistributedCache distributedCache = serviceProvider.GetService<IDistributedCache>();


// RUN EXAMPLES

// Example 1
Console.WriteLine("Example 1: Basic Memory cache operation");
var runExample1 = new RunExample1(distributedCache);
// Store in cache
runExample1.Execute();
// Retrieve from cache
runExample1.Execute();

Console.WriteLine(" ");

// Example 2
Console.WriteLine("Example 2 - Distributed cache with options");
var runExample2 = new RunExample2(distributedCache);
// Store in cache
runExample2.Execute();
// Retrieve from cache
//runExample2.Execute();

Console.WriteLine(" ");

// Example 3
Console.WriteLine("Example 3 - Distributed cache with generics");
var runExample3 = new RunExample3(distributedCache);
runExample3.Execute();


Console.WriteLine("COMPLETED!");


