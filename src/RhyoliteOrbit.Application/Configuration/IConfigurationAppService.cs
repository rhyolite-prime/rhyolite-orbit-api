using RhyoliteOrbit.Configuration.Dto;
using System.Threading.Tasks;

namespace RhyoliteOrbit.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
