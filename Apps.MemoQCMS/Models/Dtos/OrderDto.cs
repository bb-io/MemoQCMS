using Blackbird.Applications.Sdk.Common;

namespace Apps.MemoQCMS.Models.Dtos;

public class OrderDto
{
    [Display("Order ID")]
    public string OrderId { get; set; }
    
    public string Name { get; set; }
    
    [Display("Callback URL")]
    public string CallbackUrl { get; set; }
    
    [Display("Date of creation")]
    public DateTime TimeCreated { get; set; }
    
    public DateTime? Deadline { get; set; }
    
    public string Status { get; set; }
}