using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Actors;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.BannerSliders;

public class BannerSliderManager: DomainService, IBannerSliderManager
{
    private readonly IRepository<BannerSlider, Guid> _repositoryBannerSlider;
    private readonly IActorRef _dataSupportEntitySyncActorProvider;
    
    public BannerSliderManager(IRepository<BannerSlider, Guid> repositoryBannerSlider, TopLevelActors.DataSupportEntitySyncActorProvider dataSupportEntitySyncActorProvider)
    {
        _repositoryBannerSlider = repositoryBannerSlider;
        _dataSupportEntitySyncActorProvider = dataSupportEntitySyncActorProvider();
    }

    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<BannerSlider>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.ImageAlt.ToLower().Contains(query.ToLower()));

        }
        
        var totalCount = await _repositoryBannerSlider.CountAsync(predicate);
        
        var data = await (await _repositoryBannerSlider.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            totalCount,
            totalPages,
            data,
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
        
    }


    public async Task Create(BannerSlider entity, string dataSupportBusinessAccountId)
    {
        await _repositoryBannerSlider.InsertAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_banner_slider",
            Operation = "create",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }


    public async Task Update(BannerSlider entity, string dataSupportBusinessAccountId)
    {
        await _repositoryBannerSlider.UpdateAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_banner_slider",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }

    public async Task Delete(Guid id, string dataSupportBusinessAccountId)
    {
        var bannerInfo = await _repositoryBannerSlider.FirstOrDefaultAsync(id);
        await _repositoryBannerSlider.HardDeleteAsync(bannerInfo);
        
        var syncDataModel = new
        {
            EntityName = "pos_banner_slider",
            Operation = "delete",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = bannerInfo
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
}