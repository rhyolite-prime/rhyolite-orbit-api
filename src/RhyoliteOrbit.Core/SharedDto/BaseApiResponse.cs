using Newtonsoft.Json;

namespace RhyoliteOrbit.SharedDto;

public class BaseApiResponse
{
    [JsonProperty("targetUrl")]
    public object TargetUrl { get; set; }

    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("error")]
    public object Error { get; set; }

    [JsonProperty("unAuthorizedRequest")]
    public bool UnAuthorizedRequest { get; set; }

    [JsonProperty("__abp")]
    public bool Abp { get; set; }
}