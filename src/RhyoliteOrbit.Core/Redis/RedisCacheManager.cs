using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using StackExchange.Redis;

namespace RhyoliteOrbit.Redis;

public class RedisCacheManager: DomainService, IRedisCacheManager
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisCacheManager(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    // make dbNumber an optional parameter
        

    public async Task SetValueAsync(string key, string value, int dbNumber = 0)
    {
        //if dbNumber is not provided, use 0 as default
            
        Logger.Info($"Set value={value} with Key={key}");
        var db = _connectionMultiplexer.GetDatabase(dbNumber);
        await db.StringSetAsync(key, value);
    }

    public async Task SetExpireTimeAsync(string key, TimeSpan expiryTime, int dbNumber = 0)
    {
        Logger.Info($"Set Expiry Time value={expiryTime} with Key={key}");
        var db = _connectionMultiplexer.GetDatabase(dbNumber);
        await db.KeyExpireAsync(key, expiryTime);
    }

    public async Task RemoveValueAsync(string key, int dbNumber = 0)
    {
        Logger.Info($"Remove value with Key={key}");
        var db = _connectionMultiplexer.GetDatabase(dbNumber);
        await db.KeyDeleteAsync(key);
        Logger.Info($"Key={key} was removed");
    }

    public async Task<string> GetValueAsync(string key, bool shouldRemove = false, int dbNumber = 0)
    {
        Logger.Info($"Get value with Key={key}");
        var db = _connectionMultiplexer.GetDatabase(dbNumber);

        var value = await db.StringGetAsync(key);
        if (shouldRemove)
        {
            db.KeyDelete(key);
            Logger.Info($"Key={key} was removed");

        }


        return value;
    }
}