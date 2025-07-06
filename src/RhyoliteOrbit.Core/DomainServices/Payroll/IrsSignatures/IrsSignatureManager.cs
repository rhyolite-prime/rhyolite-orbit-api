using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.IrsSignatures;

public class IrsSignatureManager: Abp.Domain.Services.DomainService, IIrsSignatureManager
{
    private readonly IRepository<IrsSignature, Guid> _repositoryIrsSignature;

    public IrsSignatureManager(IRepository<IrsSignature, Guid> repositoryIrsSignature)
    {
        _repositoryIrsSignature = repositoryIrsSignature;
    }

    public async Task Create(IrsSignature entity)
    {
        var datta = await (await _repositoryIrsSignature.GetAllAsync()).FirstOrDefaultAsync();

        if (datta != null)
        {
            datta.Name = entity.Name;
            datta.Title = entity.Title;

            await _repositoryIrsSignature.UpdateAsync(datta);
        }
        else
        {
            await _repositoryIrsSignature.InsertAsync(entity);
        }
    }

    public async Task<object> GetSignature()
    {
        return await (await _repositoryIrsSignature.GetAllAsync()).FirstOrDefaultAsync();
    }
}