using Apps.MemoQCMS.Models.Dtos;
using Apps.MemoQCMS.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.MemoQCMS.DataSourceHandlers;

public class JobDataSourceHandler : MemoQCMSInvocable, IAsyncDataSourceHandler
{
    private readonly OrderIdentifier _orderIdentifier;
    
    public JobDataSourceHandler(InvocationContext invocationContext, [ActionParameter] OrderIdentifier orderIdentifier) 
        : base(invocationContext)
    {
        _orderIdentifier = orderIdentifier;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (_orderIdentifier.OrderId == null)
            throw new Exception("Please enter order first.");
        
        var jobs = await ExecuteRequestAsync<IEnumerable<JobDto>>($"/orders/{_orderIdentifier.OrderId}/jobs", Method.Get);
        return jobs
            .Where(job => context.SearchString == null 
                          || job.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(30)
            .ToDictionary(job => job.TranslationJobId, job => job.Name);
    }
}