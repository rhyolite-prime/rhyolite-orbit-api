using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.InitializePayMonths;

public class InitializePayMonthManager: Abp.Domain.Services.DomainService, IInitializePayMonthManager
{
    private readonly IRepository<InitializePayMonth, Guid> _repositoryInitializePayMonth;

    public InitializePayMonthManager(IRepository<InitializePayMonth, Guid> repositoryInitializePayMonth)
    {
        _repositoryInitializePayMonth = repositoryInitializePayMonth;
    }
    public async Task<object> GetData()
    {
        return await (await _repositoryInitializePayMonth.GetAllAsync()).FirstOrDefaultAsync();
    }


    public async Task Create(InitializePayMonth entity)
    {
        var datta = await (await _repositoryInitializePayMonth.GetAllAsync()).FirstOrDefaultAsync();
            
        if (datta != null)
        {
            datta.Month = entity.Month;
            datta.Year = entity.Year;

            await _repositoryInitializePayMonth.UpdateAsync(datta);
        }
        else
        {
            await _repositoryInitializePayMonth.InsertAsync(entity);
        }

    }
}