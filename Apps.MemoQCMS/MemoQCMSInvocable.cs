using Apps.MemoQCMS.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.MemoQCMS;

public class MemoQCMSInvocable : BaseInvocable
{
    private readonly MemoQCMSClient _client;
    
    protected MemoQCMSInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        _client = new(invocationContext.AuthenticationCredentialsProviders);
    }

    protected async Task<T> ExecuteRequestAsync<T>(string endpoint, Method method, object? requestBody = null)
    {
        var request = CreateRequest(endpoint, method, requestBody);
        var response = await _client.ExecuteWithErrorHandling<T>(request);
        return response;
    }
    
    protected async Task<RestResponse> ExecuteRequestAsync(string endpoint, Method method, object? requestBody = null)
    {
        var request = CreateRequest(endpoint, method, requestBody);
        var response = await _client.ExecuteWithErrorHandling(request);
        return response;
    }

    private MemoQCMSRequest CreateRequest(string endpoint, Method method, object? requestBody = null)
    {
        var request = new MemoQCMSRequest(endpoint, method);

        if (requestBody != null)
            request.AddJsonBody(requestBody);

        return request;
    }
}