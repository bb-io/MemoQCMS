using Blackbird.Applications.Sdk.Common;

namespace Apps.MemoQCMS.Models.Requests.Orders;

public class ListOrdersRequest
{
    [Display("Created after")]
    public DateTime? CreatedAfter { get; set; }
    
    [Display("Created before")]
    public DateTime? CreatedBefore { get; set; }
    
    [Display("Deadline after")]
    public DateTime? DeadlineAfter { get; set; }
    
    [Display("Deadline before")]
    public DateTime? DeadlineBefore { get; set; }
}