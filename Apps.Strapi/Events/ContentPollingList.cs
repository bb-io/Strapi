using Apps.Strapi.Events.Models;
using Apps.Strapi.Models.Responses;
using Apps.Strapi.Utils;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Models.Responses;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Strapi.Events;

[PollingEventList]
public class ContentPollingList(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [PollingEvent("On content created or updated", Description = "Polling event that periodically checks for new new or updated content. If the new or updated content is found, it will be returned as a list of content items.")]
    public async Task<PollingEventResponse<DateMemory, SearchContentWithTypeResponse>> OnContentCreatedOrUpdatedAsync(PollingEventRequest<DateMemory> request,
        [PollingEventParameter] ContentFilters contentRequest)
    {
        return await ProcessPollingRequest(request, contentRequest, (apiRequest, lastPollingTime) =>
        {
            apiRequest.AddQueryParameter("filters[$or][0][createdAt][$gte]", lastPollingTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            apiRequest.AddQueryParameter("filters[$or][1][updatedAt][$gte]", lastPollingTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        });
    }

    [PollingEvent("On content published", Description = "Polling event that periodically checks for newly published content. If newly published content is found, it will be returned as a list of content items.")]
    public async Task<PollingEventResponse<DateMemory, SearchContentWithTypeResponse>> OnContentPublishedAsync(PollingEventRequest<DateMemory> request,
        [PollingEventParameter] ContentFilters contentRequest)
    {
        return await ProcessPollingRequest(request, contentRequest, (apiRequest, lastPollingTime) =>
            apiRequest.AddQueryParameter("filters[$or][0][publishedAt][$gte]", lastPollingTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
    }

    private async Task<PollingEventResponse<DateMemory, SearchContentWithTypeResponse>> ProcessPollingRequest(
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

        var contentList = new List<DocumentWithContentTypeResponse>();
        foreach (var contentTypeId in contentRequest.ContentTypeIds)
        {
            try
            {
                var apiRequest = BuildBaseApiRequest(contentTypeId, contentRequest);
                addFilters.Invoke(apiRequest, request.Memory.LastPollingTime);

                var result = await Client.PaginateAsync<JObject>(apiRequest);
                var currentContentList = result.ToContentListResponse();
                var contentListWithType = currentContentList
                    .Select(content => new DocumentWithContentTypeResponse(content, contentTypeId))
                    .ToList();

                contentList.AddRange(contentListWithType);
            }
            catch (Exception ex)
            {
                InvocationContext.Logger?.LogError($"[Strapi] Error while polling content for content type ID '{contentTypeId}': {ex.Message}", []);
            }
        }

        return new()
        {
            Result = new(contentList),
            FlyBird = contentList.Count > 0,
            Memory = new DateMemory
            {
                LastPollingTime = DateTime.UtcNow
            }
        };
    }

    private RestRequest BuildBaseApiRequest(string contentTypeId, ContentFilters contentRequest)
    {
        var apiRequest = new RestRequest($"/api/{contentTypeId}");

        if (contentRequest.Language != null)
        {
            apiRequest.AddQueryParameter("locale", contentRequest.Language);
        }

        if (contentRequest.Status != null)
        {
            apiRequest.AddQueryParameter("status", contentRequest.Status);
        }

        QueryParameterBuilder.AddFieldFiltersIfAvailable(apiRequest, contentRequest.FieldNames, contentRequest.FieldValues);
        return apiRequest;
    }
}