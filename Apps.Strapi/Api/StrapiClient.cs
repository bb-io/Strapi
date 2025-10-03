using System.Net;
using Apps.Strapi.Constants;
using Apps.Strapi.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Strapi.Api;

public class StrapiClient : BlackBirdRestClient
{
    public StrapiClient(IEnumerable<AuthenticationCredentialsProvider> credentialsProviders) : base(new()
    {
        BaseUrl = new(credentialsProviders.Get(CredNames.BaseUrl).Value.Trim('/')),
        ThrowOnAnyError = false
    })
    {
        var apiToken = credentialsProviders.Get(CredNames.ApiToken).Value;
        this.AddDefaultHeader("Authorization", $"Bearer {apiToken}");
    }

    public async Task<List<T>> PaginateAsync<T>(RestRequest request)
    {
        var allResults = new List<T>();
        const int defaultPageSize = 100;
        
        if (!request.Parameters.Any(p => p.Name == "pagination[pageSize]"))
        {
            request.AddQueryParameter("pagination[pageSize]", defaultPageSize.ToString());
        }
        
        int currentPage = 1;
        int totalPages;
        do
        {
            var existingPageParam = request.Parameters.FirstOrDefault(p => p.Name == "pagination[page]" && p.Type == ParameterType.QueryString);
            if (existingPageParam != null)
            {
                request.RemoveParameter(existingPageParam);
            }

            request.AddQueryParameter("pagination[page]", currentPage.ToString());
            
            var paginatedResponse = await ExecuteWithErrorHandling<PaginationDto<T>>(request);
            if (paginatedResponse.Data != null && paginatedResponse.Data.Any())
            {
                allResults.AddRange(paginatedResponse.Data);
            }
            
            totalPages = paginatedResponse.Meta.Pagination.PageCount;
            currentPage++;
            
            if (currentPage > totalPages)
            {
                break;
            }
            
        } while (true);
        
        return allResults;
    }
    
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        if(string.IsNullOrEmpty(response.Content))
        {
            if(string.IsNullOrEmpty(response.ErrorMessage))
            {
                return new PluginApplicationException($"Status code: {response.StatusCode}");
            }

            return new PluginApplicationException(response.ErrorMessage);
        }
        
        if(response.StatusCode == HttpStatusCode.MethodNotAllowed && response.ContentType == "text/plain")
        {
            return new PluginApplicationException("Probably you provided wrong 'Content type ID'. Please verify that the content type ID is correct and ends with 's' (e.g., 'articles' not 'article').");
        }
        
        if(response.ContentType == "text/html")
        {
            return new PluginApplicationException($"Status code: {response.StatusCode}, content: {response.Content}");
        }

        var error = JsonConvert.DeserializeObject<ErrorDto>(response.Content!);
        if(error is null)
        {
            return new PluginApplicationException(response.Content);
        }

        return new PluginApplicationException(error.ToString());
    }
}