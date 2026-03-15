using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using RhyoliteOrbit.DomainServices.Quartz.Dto;

namespace RhyoliteOrbit.DomainServices.Quartz;

public class QuartzJobScheduler: DomainService, IQuartzJobScheduler
{
    private readonly string _apiBaseUrl;

    public QuartzJobScheduler(IConfiguration config)
    {
        _apiBaseUrl = config["QuartzJobSchedulerApi:BaseUrl"];

    }
    
    public async Task<object> CreateJob(JobModel jobModel)
    {
        Logger.Info($"starting call to create quartz job, quartz_request => {JsonConvert.SerializeObject(jobModel)}");

        try
        {
            var client = new RestClient(_apiBaseUrl);
            var request = new RestRequest("", Method.Post);
        
            // Add JSON body
            request.AddJsonBody(jobModel);
        
            // Execute the request without throwing exceptions on HTTP errors
            var response = await client.ExecuteAsync(request);
        
            Logger.Info($"successfully created quartz job, quartz_response => {response.Content}");

            var apiResponse = JsonConvert.DeserializeObject<object>(response.Content);

            return apiResponse;
        }
        catch (Exception e)  // RestSharp typically doesn't throw HTTP exceptions by default
        {
            Logger.Error($"creating quartz_job exception. => {e.Message}, {e.StackTrace}");
            return null;
        }
    }
 
    public async Task<bool> DeleteJob(string jobName)
    {
        try
        {
            Logger.Info($"starting call to delete schedule message, scheduleId => {jobName}");

            var client = new RestClient(_apiBaseUrl);
            var request = new RestRequest(jobName, Method.Delete);
        
            var response = await client.ExecuteAsync(request);
        
            if (!response.IsSuccessful)
            {
                Logger.Error($"HTTP error {(int)response.StatusCode}: {response.Content}");
            }
        
            Logger.Error($"delete scheduled message response => {response.Content}, scheduleId => {jobName}");

            return response.IsSuccessful;
        }
        catch (Exception e)
        {
            Logger.Error($"creating quartz_job exception. => {e.Message}, {e.StackTrace}");
            return false;
        }
    }
}