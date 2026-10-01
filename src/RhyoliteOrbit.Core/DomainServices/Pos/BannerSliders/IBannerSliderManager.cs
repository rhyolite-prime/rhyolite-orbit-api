using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.BannerSliders;

public interface IBannerSliderManager: IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(BannerSlider input, string dataSupportBusinessAccountId);
    Task Update(BannerSlider input, string dataSupportBusinessAccountId);
    Task Delete(Guid id, string dataSupportBusinessAccountId);
}