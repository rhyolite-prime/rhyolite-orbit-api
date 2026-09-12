using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.NotificationAccounts;

public class NotificationAccountManager: DomainService, INotificationAccountManager
{
    private readonly IRepository<NotificationAccount, Guid> _repositoryNotificationAccount;

    public NotificationAccountManager(IRepository<NotificationAccount, Guid> repositoryNotificationAccount)
    {
        _repositoryNotificationAccount = repositoryNotificationAccount;
    }


    public async Task<object> GetAll(int tenantId)
    {
        return await _repositoryNotificationAccount.FirstOrDefaultAsync(a => a.TenantId == tenantId);
    }


    public async Task Create(NotificationAccount entity)
    {
        var datta = await _repositoryNotificationAccount.FirstOrDefaultAsync(x => x.TenantId == entity.TenantId);
        if (datta != null)
        {
            await _repositoryNotificationAccount.UpdateAsync(entity);
        }
        
        await _repositoryNotificationAccount.InsertAsync(entity);

    }


    public async Task Update(NotificationAccount entity)
    {
        await _repositoryNotificationAccount.UpdateAsync(entity);
    }

    public async Task Delete(Guid id)
    {
        await _repositoryNotificationAccount.DeleteAsync(id);
    }
}