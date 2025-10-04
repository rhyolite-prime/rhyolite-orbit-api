using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.PayrollPeriods;

public class PayrollPeriodManager: Abp.Domain.Services.DomainService, IPayrollPeriodManager
{
    private readonly IRepository<PayrollPeriod, Guid> _repositoryPayrollPeriod;

    public PayrollPeriodManager(IRepository<PayrollPeriod, Guid> repositoryPayrollPeriod)
    {
        _repositoryPayrollPeriod = repositoryPayrollPeriod;
    }
    public async Task<object> GetData()
    {
        return await (await _repositoryPayrollPeriod.GetAllAsync()).FirstOrDefaultAsync();
    }


    public async Task Create(PayrollPeriod entity)
    {
        var datta = await (await _repositoryPayrollPeriod.GetAllAsync()).FirstOrDefaultAsync();
            
        if (datta != null)
        {
            datta.Month = entity.Month;
            datta.Year = entity.Year;

            await _repositoryPayrollPeriod.UpdateAsync(datta);
        }
        else
        {
            await _repositoryPayrollPeriod.InsertAsync(entity);
        }

    }
}