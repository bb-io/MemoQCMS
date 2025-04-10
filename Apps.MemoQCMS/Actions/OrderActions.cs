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
    public async Task<OrderResponse> GetOrder([ActionParameter] OrderIdentifier orderIdentifier)
    {
        var order = await ExecuteRequestAsync<OrderDto>($"/orders/{orderIdentifier.OrderId}", Method.Get);

        var jobs = await ExecuteRequestAsync<IEnumerable<JobDto>>($"/orders/{orderIdentifier.OrderId}/jobs", Method.Get);
        string jobsCompletion;

        if (jobs != null && jobs.Any())
        {
            if (jobs.All(job => job.Status == "DeliveredToSource"))
            {
                jobsCompletion = "Delivered to source";
            }
            else if (jobs.All(job => job.Status == "Cancelled"))
            {
                jobsCompletion = "Cancelled";
            }
            else if (jobs.All(job => job.Status == "DeliveredToSource" || job.Status == "Cancelled"))
            {
                jobsCompletion = "Delivered to source or cancelled";
            }
            else
            {
                jobsCompletion = "In progress";
            }
        }
        else
        {
            jobsCompletion = "None";
        }

        var orderResponse = new OrderResponse
        {
            OrderId = order.OrderId,
            Name = order.Name,
            CallbackUrl = order.CallbackUrl,
            TimeCreated = order.TimeCreated,
            Deadline = order.Deadline,
            Status = order.Status,
            JobsCompletion = jobsCompletion
        };

        return orderResponse;
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