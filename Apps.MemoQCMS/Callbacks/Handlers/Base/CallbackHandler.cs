using Apps.MemoQCMS.Constants;
using Apps.MemoQCMS.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Webhooks.Bridge;
using Blackbird.Applications.Sdk.Utils.Webhooks.Bridge.Models.Request;
using RestSharp;

namespace Apps.MemoQCMS.Callbacks.Handlers.Base;

public abstract class CallbackHandler : MemoQCMSInvocable, IWebhookEventHandler
{
    protected abstract string Event { get; }
    
    protected CallbackHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> credentials, 
        Dictionary<string, string> values)
    {
        var (input, bridgeCredentials) = await GetBridgeServiceInputs(values);
        await BridgeService.Subscribe(input, bridgeCredentials);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> credentials,
        Dictionary<string, string> values)
    {
        var (input, bridgeCredentials) = await GetBridgeServiceInputs(values);
        await BridgeService.Unsubscribe(input, bridgeCredentials);
    }
    
    private async Task<(BridgeRequest webhookData, BridgeCredentials bridgeCreds)> GetBridgeServiceInputs(
        Dictionary<string, string> values)
    {
        var connectionKey = InvocationContext.AuthenticationCredentialsProviders.Get(CredsNames.ConnectionKey).Value;
        
        var webhookData = new BridgeRequest
        {
            Event = Event,
            Id = connectionKey,
            Url = values["payloadUrl"]
        };

        var bridgeCredentials = new BridgeCredentials
        {
            ServiceUrl = $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}{ApplicationConstants.BridgePath}",
            Token = ApplicationConstants.BlackbirdToken
        };

        return (webhookData, bridgeCredentials);
    }
}