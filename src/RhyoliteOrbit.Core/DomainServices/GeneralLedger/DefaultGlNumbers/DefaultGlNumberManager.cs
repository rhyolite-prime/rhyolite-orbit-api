using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.DefaultGlNumbers;

public class DefaultGlNumberManager : DomainService, IDefaultGlNumberManager
{
    private readonly IRepository<DefaultGlNumber, Guid> _repositoryDefaultGlNumber;

    public DefaultGlNumberManager(IRepository<DefaultGlNumber, Guid> repositoryDefaultGlNumber)
    {
        _repositoryDefaultGlNumber = repositoryDefaultGlNumber;
    }


    public async Task<object> GetDefaultSetup()
    {
        return await (await _repositoryDefaultGlNumber.GetAllAsync()).FirstOrDefaultAsync();
    }

    public async Task Create(DefaultGlNumber input)
    {
        await _repositoryDefaultGlNumber.InsertAsync(input);
    }


    public async Task Update(DefaultGlNumber input)
    {
        await _repositoryDefaultGlNumber.UpdateAsync(input);
    }
}