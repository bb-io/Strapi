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
        return await ProcessPollingRequest(request, contentRequest, (apiRequest, lastPollingTime) =>
        {
            apiRequest.AddQueryParameter("filters[$or][0][createdAt][$gte]", lastPollingTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            apiRequest.AddQueryParameter("filters[$or][1][updatedAt][$gte]", lastPollingTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        });
    }
    
    [PollingEvent("On content published", Description = "Polling event that periodically checks for newly published content. If newly published content is found, it will be returned as a list of content items.")]
    public async Task<PollingEventResponse<DateMemory, SearchContentResponse>> OnContentPublishedAsync(PollingEventRequest<DateMemory> request,
        [PollingEventParameter] ContentFilters contentRequest)
    {
        return await ProcessPollingRequest(request, contentRequest, (apiRequest, lastPollingTime) =>
            apiRequest.AddQueryParameter("filters[$or][0][publishedAt][$gte]", lastPollingTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
    }

    private async Task<PollingEventResponse<DateMemory, SearchContentResponse>> ProcessPollingRequest(
        PollingEventRequest<DateMemory> request,
        ContentFilters contentRequest,
        Action<RestRequest, DateTime> addFilters)
    {
        if (request.Memory == null)
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

        var apiRequest = BuildBaseApiRequest(contentRequest);
        addFilters.Invoke(apiRequest, request.Memory.LastPollingTime);

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

    private RestRequest BuildBaseApiRequest(ContentFilters contentRequest)
    {
        var apiRequest = new RestRequest($"/api/{contentRequest.ContentTypeId}");

        if (contentRequest.Language != null)
        {
            apiRequest.AddQueryParameter("locale", contentRequest.Language);
        }

        if (contentRequest.Status != null)
        {
            apiRequest.AddQueryParameter("status", contentRequest.Status);
        }

        return apiRequest;
    }
}