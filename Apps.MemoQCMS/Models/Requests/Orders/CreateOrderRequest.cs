using Blackbird.Applications.Sdk.Common;

namespace Apps.MemoQCMS.Models.Requests.Orders;

public class CreateOrderRequest
{
    [Display("Order name")]
    public string Name { get; set; }
    
    [Display("Callback URL")]
    public string? CallbackUrl { get; set; }
    
    public DateTime? Deadline { get; set; }
}