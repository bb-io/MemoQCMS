using Apps.MemoQCMS.Models.Dtos;

namespace Apps.MemoQCMS.Models.Responses.Orders;

public record ListOrdersResponse(IEnumerable<OrderDto> Orders);