using Apps.MemoQCMS.Constants;
using Apps.MemoQCMS.Models.Dtos;
using Apps.MemoQCMS.Models.Identifiers;
using Apps.MemoQCMS.Models.Requests.Orders;
using Apps.MemoQCMS.Models.Responses.Orders;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.MemoQCMS.Actions;

[ActionList]
public class OrderActions : MemoQCMSInvocable
{
    public OrderActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("List orders", Description = "List orders, optionally specifying start and end dates for filtering based " +
                                         "on the creation date or deadline.")]
    public async Task<ListOrdersResponse> ListOrders([ActionParameter] ListOrdersRequest input)
    {
        var orders = await ExecuteRequestAsync<IEnumerable<OrderDto>>("/orders", Method.Get);

        if (input.CreatedAfter != null)
            orders = orders.Where(order => order.TimeCreated >= input.CreatedAfter);
        
        if (input.CreatedBefore != null)
            orders = orders.Where(order => order.TimeCreated <= input.CreatedBefore);
        
        if (input.DeadlineAfter != null)
            orders = orders.Where(order => order.Deadline == null || order.Deadline >= input.DeadlineAfter);
        
        if (input.DeadlineBefore != null)
            orders = orders.Where(order => order.Deadline == null || order.Deadline <= input.DeadlineBefore);

        return new(orders);
    }

    [Action("Get order", Description = "Retrieve information about an order.")]
    public async Task<OrderDto> GetOrder([ActionParameter] OrderIdentifier orderIdentifier)
    {
        var order = await ExecuteRequestAsync<OrderDto>($"/orders/{orderIdentifier.OrderId}", Method.Get);
        return order;
    }

    [Action("Create order", Description = "Create a new order.")]
    public async Task<OrderDto> CreateOrder([ActionParameter] CreateOrderRequest input)
    {
        var connectionKey = InvocationContext.AuthenticationCredentialsProviders.Get(CredsNames.ConnectionKey).Value;
        var requestBody = new
        {
            input.Name,
            input.Deadline,
            CallbackUrl = input.CallbackUrl ??
                          $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}{ApplicationConstants.BridgePath}"
                              .SetQueryParameter("id", connectionKey)
        };
        
        var order = await ExecuteRequestAsync<OrderDto>("/orders", Method.Post, requestBody);
        return order;
    }

    [Action("Commit order", Description = "Change the status of an order to \"Committed\".")]
    public async Task<OrderIdentifier> CommitOrder([ActionParameter] OrderIdentifier orderIdentifier)
    {
        var requestBody = new { NewStatus = "Committed" };
        await ExecuteRequestAsync($"/orders/{orderIdentifier.OrderId}", Method.Patch, requestBody);
        return orderIdentifier;
    }
}