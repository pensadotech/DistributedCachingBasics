# Microsoft Distributed Caching

_By Armando A. Pensado_

### Characteristics and NuGet Dependencies
Console Application using .NET 8 and C#

**NuGet Dependencies**:
 * Microsoft.Extensions.Caching.Abstractions
 * Microsoft.Extensions.Caching.Memory
 * Microsoft.Extensions.Configuration
 * Microsoft.Extensions.Configuration.Json
 * Microsoft.Extensions.DependencyInjection

---

## Description 

This project demonstrates the use of Microsoft Distributed Caching within the context of a Console application. It provides a basic-level example to illustrate the mechanics of distributed caching outside the complexities of ASP.NET Core.

The project contains three exercises, each offering a gradual approach to understanding distributed caching implementation:

1. **Example 1**: A simple implementation of distributed caching.
2. **Example 2**: A multi-threaded approach with lock controls.
3. **Example 3**: A generic multi-threaded approach implementation for caching any object type.

The project also includes a pretend database to simulate external resource access.

---

## Caching Pattern 

ASP.NET Core supports several caching mechanisms. The simplest is the `IMemoryCache`, which stores data in the memory of the web server. However, for distributed systems or multi-server environments, a distributed cache is preferred. 

The distributed cache stores data as key-value pairs, with the value limited to `byte[]`. This makes it suitable for scenarios where data needs to be shared across multiple servers or processes.

### Use Cases for Caching:
1. **User Avatars**: Store user avatars in memory to avoid repeated database calls.
2. **Weather Data**: Cache weather data that updates periodically to reduce latency.
3. **API Tokens**: Cache security tokens to minimize calls to an authorization server.

For distributed systems, Redis is often recommended as the caching solution.

---

## How It Works

The program starts by setting up a service collection and registering the distributed cache using `AddDistributedMemoryCache()`. The `IDistributedCache` instance is then passed to the `RunExample` classes, which demonstrate different caching scenarios.

### Flow:
1. **Program.cs**: Sets up dependency injection and runs the examples.
2. **RunExample Classes**: Each class encapsulates a specific caching scenario.
3. **Pretend Database**: Simulates an external resource to demonstrate caching benefits.

---

## Concepts of the Three Exercises

### Example 1: Basic Caching
- **File**: `RunExample1.cs`
- **Objective**: Demonstrates a straightforward implementation of distributed caching.
- **Details**: 
  - Stores and retrieves data from the cache.
  - No multi-threading or advanced features.
  - Ideal for understanding the basics of caching.

### Example 2: Multi-Threaded Caching with Lock Controls
- **Files**: `DistributedCacheWithOptions.cs`, `RunExample2.cs`
- **Objective**: Introduces multi-threading and lock controls to prevent race conditions.
- **Details**:
  - Uses `SemaphoreSlim` to manage concurrent access to the cache.
  - Ensures only one thread accesses the external resource at a time.
  - Demonstrates how to handle concurrent cache operations safely.

### Example 3: Generic Multi-Threaded Caching for Any Object Type
- **Files**: `DistributedCacheGeneric.cs`, `RunExample3.cs`
- **Objective**: Implements a generic approach to caching.
- **Details**:
  - Supports caching of any object type by serializing/deserializing data.
  - Separates cache access into private methods for better modularity.
  - Demonstrates a reusable and flexible caching mechanism.
  - Uses `SemaphoreSlim` to manage concurrent access to the cache.
  - Ensures only one thread accesses the external resource at a time.
  - Demonstrates how to handle concurrent cache operations safely.

---

## How Developers Can Get Started

1. Clone or download the repository.
2. Rebuild the application to restore NuGet dependencies. If dependencies are not restored automatically, use the `NuGet restore` command.
3. Run the application to explore the examples.

---

## References

- [IDistributedCache Interface](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache?view=net-8.0)
- [Distributed Caching in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-8.0)
- [SemaphoreSlim Class](https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim?view=net-8.0)
- [Async Waiting inside C# Locks](https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/)
- [Microsoft.Extensions.Caching.Distributed Namespace](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed?view=net-8.0)
- [IMemoryCache Interface](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.imemorycache?view=net-8.0)
- [MemoryCache Class](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.caching.memorycache?view=net-8.0)
- [Cache in-memory in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-8.0)
- [Cache Implementations in C# .NET](https://michaelscodingspot.com/cache-implementations-in-csharp-net/)
- [Caching Design Pattern](https://www.geeksforgeeks.org/caching-design-pattern/)
- [A Hitchhiker’s Guide to Caching Patterns](https://hazelcast.com/blog/a-hitchhikers-guide-to-caching-patterns/)
- [Redis](https://redis.io/)

---

