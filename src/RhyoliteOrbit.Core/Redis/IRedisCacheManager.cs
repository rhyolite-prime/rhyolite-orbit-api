using System;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace RhyoliteOrbit.Redis;

public interface IRedisCacheManager: IDomainService
{
    Task SetValueAsync(string key, string value, int dbNumber = 0);
    Task SetExpireTimeAsync(string key, TimeSpan expiryTime, int dbNumber = 0);
    Task RemoveValueAsync(string key, int dbNumber = 0);
    Task<string> GetValueAsync(string key, bool shouldRemove = false, int dbNumber = 0);
}