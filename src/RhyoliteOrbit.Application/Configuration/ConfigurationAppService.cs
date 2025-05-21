using Abp.Authorization;
using Abp.Runtime.Session;
using RhyoliteOrbit.Configuration.Dto;
using System.Threading.Tasks;

namespace RhyoliteOrbit.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : RhyoliteOrbitAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
