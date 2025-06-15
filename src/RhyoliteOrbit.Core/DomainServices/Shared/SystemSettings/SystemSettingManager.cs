using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.SystemSettings;

public class SystemSettingManager: Abp.Domain.Services.DomainService, ISystemSettingManager
{
    private readonly IRepository<SystemSetting, Guid> _repositorySystemSetting;
    private readonly IRepository<Currency, Guid> _repositoryCurrency;

    public SystemSettingManager(IRepository<SystemSetting, Guid> repositorySystemSetting, IRepository<Currency, Guid> repositoryCurrency, IUnitOfWorkManager unitOfWorkManager, IHashids hashids)
    {
        _repositorySystemSetting = repositorySystemSetting;
        _repositoryCurrency = repositoryCurrency;
        
    }
    
    public async Task<string> GetDefaultCurrencyCode()
    {
        var systemSettings =  await (await _repositorySystemSetting.GetAllAsync()).FirstOrDefaultAsync();
            
        if (systemSettings != null && systemSettings.DefaultCurrencyId != Guid.Empty)
        {
            var currencyInfo = await _repositoryCurrency.FirstOrDefaultAsync(systemSettings.DefaultCurrencyId);
            return currencyInfo.Code;
        }

        return "GHS";

    }
}