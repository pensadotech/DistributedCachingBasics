using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedCacheExample.DataAccess;

public class PretendDatabase
{
    /// <summary>
    /// Pretend is an access to a database to read a value
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public string GetAvatar(int index, string userId)
    {
        // Console.WriteLine("  DB: Retreive avatar");

        string dbValue = $"Image [ {userId} ]";

        Console.WriteLine($"Agent # {index} got from DB: " + dbValue);

        return dbValue;
    }

    /// <summary>
    /// This method was not used as could not integrate wuth labmda exppesion
    /// unclear what is missing
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<string> GetAvatarAsync(int index, string userId)
    {
        string dbValue;

        Task<string> task = (Task<string>)Task.Run(() =>
        {
            dbValue = $"Image [ {userId} ]";
            return dbValue;
        });

        dbValue = await task;

        Console.WriteLine($"Agent # {index} got  async from DB: " + dbValue);

        return dbValue;
    }


}
