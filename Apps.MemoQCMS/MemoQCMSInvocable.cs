using Apps.MemoQCMS.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.MemoQCMS;

public class MemoQCMSInvocable : BaseInvocable
{
    protected readonly MemoQCMSClient Client;
    
    protected MemoQCMSInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(invocationContext.AuthenticationCredentialsProviders);
    }

    protected async Task<T> ExecuteRequestAsync<T>(string endpoint, Method method, object? requestBody = null)
    {
        var request = CreateRequest(endpoint, method, requestBody);
        var response = await Client.ExecuteWithErrorHandling<T>(request);
        return response;
    }
    
    protected async Task<RestResponse> ExecuteRequestAsync(string endpoint, Method method, object? requestBody = null)
    {
        var request = CreateRequest(endpoint, method, requestBody);
        var response = await Client.ExecuteWithErrorHandling(request);
        return response;
    }

    private MemoQCMSRequest CreateRequest(string endpoint, Method method, object? requestBody = null)
    {
        var request = new MemoQCMSRequest(endpoint, method);

        if (requestBody != null)
            request.AddJsonBody(requestBody);

        return request;
    }

    protected Exception ConfigureErrorException(RestResponse response)
    {
        throw new PluginApplicationException(response?.ErrorMessage ?? response?.Content ?? string.Empty);
    }
}