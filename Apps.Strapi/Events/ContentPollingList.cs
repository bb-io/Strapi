using Apps.Strapi.Events.Models;
using Apps.Strapi.Models.Responses;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Models.Responses;
using RestSharp;

namespace Apps.Strapi.Events;

[PollingEventList]
public class ContentPollingList(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [PollingEvent("On content created or updated", Description = "Polling event that periodically checks for new new or updated content. If the new or updated content is found, it will be returned as a list of content items.")]
    public async Task<PollingEventResponse<DateMemory, SearchContentResponse>> OnContentCreatedOrUpdatedAsync(PollingEventRequest<DateMemory> request,
        [PollingEventParameter] ContentFilters contentRequest)
    {
        if(request.Memory == null)
        {
            return new() 
            {
                Result = null,
                FlyBird = false,
                Memory = new DateMemory
                {
                    LastPollingTime = DateTime.UtcNow
                }
            };
        }

        var apiRequest = new RestRequest($"/api/{contentRequest.ContentTypeId}");
        if(contentRequest.Language != null)
        {
            apiRequest.AddQueryParameter("locale", contentRequest.Language);
        }

        if(contentRequest.Status != null)
        {
            apiRequest.AddQueryParameter("status", contentRequest.Status);
        }

        apiRequest.AddQueryParameter("filters[$or][0][createdAt][$gte]", request.Memory.LastPollingTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        apiRequest.AddQueryParameter("filters[$or][1][updatedAt][$gte]", request.Memory.LastPollingTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));

        var result = await Client.PaginateAsync<DocumentResponse>(apiRequest);
        return new()
        {
            Result = new(result),
            FlyBird = result.Count > 0,
            Memory = new DateMemory
            {
                LastPollingTime = DateTime.UtcNow
            }
        };
    }
}