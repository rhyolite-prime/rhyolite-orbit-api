using System.Threading.Tasks;
using Abp.Domain.Services;

namespace RhyoliteOrbit.ViewRenderer;

public interface IViewRenderService: IDomainService
{
    Task<string> RenderToStringAsync<TModel>(string viewName, TModel model);
}