using Apps.MemoQCMS.Constants;
using Apps.MemoQCMS.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.MemoQCMS.Api;

public class MemoQCMSClient : BlackBirdRestClient
{
    protected override JsonSerializerSettings JsonSettings =>
        new() { MissingMemberHandling = MissingMemberHandling.Ignore };

    public MemoQCMSClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        : base(new RestClientOptions
        {
            ThrowOnAnyError = false, BaseUrl = new(authenticationCredentialsProviders.Get(CredsNames.BaseUrl).Value)
        })
    {
        var connectionKey = authenticationCredentialsProviders.Get(CredsNames.ConnectionKey).Value;
        this.AddDefaultHeader("Authorization", $"CMSGATEWAY-API {connectionKey}");
    }
    
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject<ErrorDto>(response.Content);
        return new(error.Message);
    }
}