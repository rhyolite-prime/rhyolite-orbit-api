using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.SubjectCategories;
using RhyoliteOrbit.Services.SchoolManager.SubjectCategories.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.SubjectCategories;

public class SubjectCategoryAppService: RhyoliteOrbitAppServiceBase
{
    
    private readonly ISubjectCategoryManager _subjectCategoryManager;

    public SubjectCategoryAppService(ISubjectCategoryManager subjectCategoryManager)
    {
        _subjectCategoryManager = subjectCategoryManager;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _subjectCategoryManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateSubjectCategoryInput input)
    {
        var obj = ObjectMapper.Map<SubjectCategory>(input);
        await _subjectCategoryManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateSubjectCategoryInput input)
    {
        var obj = ObjectMapper.Map<SubjectCategory>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _subjectCategoryManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _subjectCategoryManager.Delete(id);
    }
}