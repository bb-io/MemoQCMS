﻿using Apps.MemoQCMS.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.MemoQCMS.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var client = new MemoQCMSClient(authenticationCredentialsProviders);
        var request = new MemoQCMSRequest("/client", Method.Get);

        try
        {
            var res = await client.ExecuteWithErrorHandling(request);
            return new()
            {
                IsValid = res.Content?.Contains("ClientId") ?? false,
            };
        }
        catch (Exception exception)
        {
            return new()
            {
                IsValid = false,
                Message = exception.Message
            };
        } 
    }
}