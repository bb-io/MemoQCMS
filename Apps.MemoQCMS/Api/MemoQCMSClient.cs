using Apps.MemoQCMS.Constants;
using Apps.MemoQCMS.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.MemoQCMS.Api;

public class MemoQCMSClient : BlackBirdRestClient
{
    protected override JsonSerializerSettings JsonSettings =>
        new() { MissingMemberHandling = MissingMemberHandling.Ignore };

    public MemoQCMSClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        : base(new RestClientOptions
        {
            ThrowOnAnyError = false, BaseUrl = new(CreateBaseUrl(authenticationCredentialsProviders.Get(CredsNames.BaseUrl).Value)),
            MaxTimeout = 180000
        })
    {
        var connectionKey = authenticationCredentialsProviders.Get(CredsNames.ConnectionKey).Value;
        this.AddDefaultHeader("Authorization", $"CMSGATEWAY-API {connectionKey}");
    }

    private static string CreateBaseUrl(string url)
    {
        return new Uri(url).GetLeftPart(UriPartial.Authority) + "/memoqservercmsgateway/v1";
    }
    
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject<ErrorDto>(response.Content);
        return new(error.Message);
    }
}