using Apps.MemoQCMS.Callbacks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MemoQCMS.Callbacks.Handlers;

public class JobStatusChangedCallbackHandler : CallbackHandler
{
    protected override string Event => "TranslationJobStatusChanged";
    
    public JobStatusChangedCallbackHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}