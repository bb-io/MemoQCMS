using System.Net;
using Apps.MemoQCMS.Callbacks.Handlers;
using Apps.MemoQCMS.Callbacks.Models.Payload;
using Apps.MemoQCMS.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.MemoQCMS.Callbacks;

[WebhookList]
public class JobCallbacks : MemoQCMSInvocable
{
    public JobCallbacks(InvocationContext invocationContext) : base(invocationContext)
    {
    }
    
    #region Bridge callbacks

    [Webhook("On job completed", typeof(JobStatusChangedCallbackHandler), 
        Description = "This event is triggered when a job is completed.")]
    public async Task<WebhookResponse<JobDto>> OnJobCompleted(WebhookRequest request) 
        => await HandleCallback(request, "TranslationReady");
    
    [Webhook("On job cancelled", typeof(JobStatusChangedCallbackHandler), 
        Description = "This event is triggered when a job is cancelled.")]
    public async Task<WebhookResponse<JobDto>> OnJobCancelled(WebhookRequest request) 
        => await HandleCallback(request, "Cancelled");

    #endregion

    #region Manual callbacks

    [Webhook("On job completed (manual)", Description = "This manual event is triggered when a job is completed.")]
    public async Task<WebhookResponse<JobDto>> OnJobCompletedManual(WebhookRequest request) 
        => await HandleCallback(request, "TranslationReady");
    
    [Webhook("On job cancelled (manual)", Description = "This manual event is triggered when a job is cancelled.")]
    public async Task<WebhookResponse<JobDto>> OnJobCancelledManual(WebhookRequest request) 
        => await HandleCallback(request, "Cancelled");

    #endregion

    private async Task<WebhookResponse<JobDto>> HandleCallback(WebhookRequest request, string status)
    {
        var payload = JsonConvert.DeserializeObject<JobStatusChangedPayload>(request.Body.ToString(),
            new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

        if (payload.Payload.NewStatus != status)
            return new WebhookResponse<JobDto>
            {
                HttpResponseMessage = new HttpResponseMessage(statusCode: HttpStatusCode.OK),
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };

        var job = await ExecuteRequestAsync<JobDto>($"/jobs/{payload.Payload.TranslationJobId}", Method.Get);

        return new WebhookResponse<JobDto>
        {
            HttpResponseMessage = new HttpResponseMessage(statusCode: HttpStatusCode.OK),
            Result = job
        };
    }
}