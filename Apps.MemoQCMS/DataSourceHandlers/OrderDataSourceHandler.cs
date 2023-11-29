using Apps.MemoQCMS.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.MemoQCMS.DataSourceHandlers;

public class OrderDataSourceHandler : MemoQCMSInvocable, IAsyncDataSourceHandler
{
    public OrderDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var orders = await ExecuteRequestAsync<IEnumerable<OrderDto>>("/orders", Method.Get);
        return orders
            .Where(order => context.SearchString == null 
                            || order.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(order => order.TimeCreated)
            .Take(30)
            .ToDictionary(order => order.OrderId, order => order.Name);
    }
}