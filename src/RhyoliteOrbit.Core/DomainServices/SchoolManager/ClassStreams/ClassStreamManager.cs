using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.ClassStreams;

public class ClassStreamManager: Abp.Domain.Services.DomainService, IClassStreamManager
{
    private readonly IRepository<ClassStream, Guid> _repositoryClassStream;

    public ClassStreamManager(IRepository<ClassStream, Guid> repositoryClassStream)
    {
        _repositoryClassStream = repositoryClassStream;
    }

    public async Task<object> Create(ClassStream entity)
    {
        var datta = await _repositoryClassStream.FirstOrDefaultAsync(x => x.Name == entity.Name);
        if (datta == null)
        {

            await _repositoryClassStream.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }

        throw new UserFriendlyException(400, "Duplicate records are not allowed.");

    }

    public async Task<object> ListAll()
    {
        return await _repositoryClassStream.GetAllListAsync();
    }
    public async Task Delete(Guid id)
    {
        await _repositoryClassStream.DeleteAsync(id);
    }

    public async Task Update(ClassStream entity)
    {
        await _repositoryClassStream.UpdateAsync(entity);
    }
}