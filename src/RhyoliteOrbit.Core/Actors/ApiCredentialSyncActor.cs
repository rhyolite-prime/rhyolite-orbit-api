using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace RhyoliteOrbit.Actors;

public class ApiCredentialSyncActor: ReceiveActor
{
    private readonly string _ultraMagnusBaseUrl;
    private readonly ILogger<ApiCredentialSyncActor> _logger;
    
    public ApiCredentialSyncActor(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _ultraMagnusBaseUrl = configuration["UltraMagnusBaseUrl"];
        _logger = serviceProvider.GetRequiredService<ILogger<ApiCredentialSyncActor>>();
        ReceiveAsync<object>(async a => await HandleMessage(a));
    }
    
    private async Task HandleMessage(object msg)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post,  $"{_ultraMagnusBaseUrl}/api/v1/sync/erp/api-keys");
        
        request.Content = JsonContent.Create(msg);  

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        if (response.IsSuccessStatusCode)
        {
            string responseData = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Api Credential Sync Actor: {responseData}");
        }
    }
}