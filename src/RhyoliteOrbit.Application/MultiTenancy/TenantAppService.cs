using System;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using RhyoliteOrbit.Authorization.Roles;
using RhyoliteOrbit.Authorization.Users;
using RhyoliteOrbit.Editions;
using RhyoliteOrbit.MultiTenancy.Dto;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using HashidsNet;
using Microsoft.AspNetCore.Authorization;

namespace RhyoliteOrbit.MultiTenancy;

//  [AbpAuthorize(PermissionNames.Pages_Tenants)]
[AllowAnonymous]

public class TenantAppService : AsyncCrudAppService<Tenant, TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>, ITenantAppService
{
    private readonly TenantManager _tenantManager;
    private readonly EditionManager _editionManager;
    private readonly UserManager _userManager;
    private readonly RoleManager _roleManager;
    private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public TenantAppService(
        IRepository<Tenant, int> repository,
        TenantManager tenantManager,
        EditionManager editionManager,
        UserManager userManager,
        RoleManager roleManager,
        IAbpZeroDbMigrator abpZeroDbMigrator, IUnitOfWorkManager unitOfWorkManager)
        : base(repository)
    {
        _tenantManager = tenantManager;
        _editionManager = editionManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _abpZeroDbMigrator = abpZeroDbMigrator;
        _unitOfWorkManager = unitOfWorkManager;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<TenantDto> PrivateCreate(CreateTenantDto input)
    {
        // Reuse the existing logic for tenant creation
        return await CreateAsync(input);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<object> GenerateApiKeys()
    {
        int tenantId = AbpSession.GetTenantId();
        
        var tenantInfo = await _tenantManager.FindByIdAsync(tenantId);

        if (tenantInfo != null)
        {
            var accountSecret = Guid.NewGuid().ToString();
            var hashids = new Hashids(accountSecret, 10);
                    
            var accountId = hashids.Encode(tenantId);
                
            tenantInfo.AccountId = accountId;
            tenantInfo.AccountSecret = accountSecret;
                
            await _tenantManager.UpdateAsync(tenantInfo);
            
            // push to rabbit mq consumer to sync credentials ...
                
            return new
            {
                accountId,
                accountSecret,
                businessAccountId = tenantInfo.DataSupportBusinessAccountId
            };
                
        }
             
        return new
        {
            accountId = string.Empty,
            accountSecret = string.Empty,
            businessAccountId = string.Empty
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public override async Task<TenantDto> CreateAsync(CreateTenantDto input)
    {
        CheckCreatePermission();

        // Create tenant
        var tenant = ObjectMapper.Map<Tenant>(input);
        tenant.IsActive = true; //Enable the tenant upon creation
        tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
            ? null
            : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

        var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
        if (defaultEdition != null)
        {
            tenant.EditionId = defaultEdition.Id;
        }

        await _tenantManager.CreateAsync(tenant);
        await CurrentUnitOfWork.SaveChangesAsync(); // To get new tenant's id.

        // Create tenant database
        _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

        // We are working entities of new tenant, so changing tenant filter
        using (CurrentUnitOfWork.SetTenantId(tenant.Id))
        {
            // Create static roles for new tenant
            CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));

            await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

            // Grant all permissions to admin role
            var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
            await _roleManager.GrantAllPermissionsAsync(adminRole);

            // Create admin user for the tenant
            var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
            await _userManager.InitializeOptionsAsync(tenant.Id);
            CheckErrors(await _userManager.CreateAsync(adminUser, User.DefaultPassword));
            await CurrentUnitOfWork.SaveChangesAsync(); // To get admin user's id

            // Assign admin user to role!
            CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        return MapToEntityDto(tenant);
    }

    protected override IQueryable<Tenant> CreateFilteredQuery(PagedTenantResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.TenancyName.Contains(input.Keyword) || x.Name.Contains(input.Keyword))
            .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
    }

    protected override IQueryable<Tenant> ApplySorting(IQueryable<Tenant> query, PagedTenantResultRequestDto input)
    {
        return query.OrderBy(input.Sorting);
    }

    protected override void MapToEntity(TenantDto updateInput, Tenant entity)
    {
        // Manually mapped since TenantDto contains non-editable properties too.
        entity.Name = updateInput.Name;
        entity.TenancyName = updateInput.TenancyName;
        entity.IsActive = updateInput.IsActive;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    public override async Task DeleteAsync(EntityDto<int> input)
    {
        CheckDeletePermission();

        var tenant = await _tenantManager.GetByIdAsync(input.Id);
        await _tenantManager.DeleteAsync(tenant);
    }

    private void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }
}

