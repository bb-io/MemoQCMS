using Apps.MemoQCMS.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MemoQCMS.Models.Identifiers;

public class OrderIdentifier
{
    [Display("Order ID")]
    [DataSource(typeof(OrderDataSourceHandler))]
    public string OrderId { get; set; }
}