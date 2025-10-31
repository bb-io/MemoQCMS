using System.Net;
using Apps.MemoQCMS.Callbacks.Handlers;
using Apps.MemoQCMS.Callbacks.Models.Payload;
using Apps.MemoQCMS.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Exceptions;
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
        var webhookBody = request.Body.ToString()
            ?? throw new InvalidOperationException("Webhook body is null");
        
        var payload = JsonConvert.DeserializeObject<JobStatusChangedPayload>(webhookBody,
            new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

        if (payload == null)
        {
            InvocationContext.Logger?.LogError($"[MemoqCMSHandleCallback] Received payload is null or invalid. Payload: {webhookBody}", []);
            throw new Exception($"Received payload is null or invalid. Payload: {webhookBody}");
        }

        if (payload.Payload.NewStatus != status)
        {
            return new WebhookResponse<JobDto>
            {
                HttpResponseMessage = new HttpResponseMessage(statusCode: HttpStatusCode.OK),
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };
        }
        
        var jobId = payload.Payload.TranslationJobId;
        if (string.IsNullOrEmpty(jobId))
        {
            InvocationContext.Logger?.LogError($"[MemoqCMSHandleCallback] Job ID is null or empty in the payload. Payload: {webhookBody}", []);
            throw new Exception("Job ID is null or empty in the payload.");
        }
        
        try
        {
            var job = await ExecuteRequestAsync<JobDto>($"/jobs/{payload.Payload.TranslationJobId}", Method.Get);
            return new WebhookResponse<JobDto>
            {
                HttpResponseMessage = new HttpResponseMessage(statusCode: HttpStatusCode.OK),
                Result = job
            };
        }
        catch (Exception e)
        {
            InvocationContext.Logger?.LogError($"[MemoqCMSHandleCallback] Error fetching job details for Job ID: {jobId}. Exception: {e.Message}", []);
            throw;
        }
    }
}