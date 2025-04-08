using Apps.MemoQCMS.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MemoQCMS.Models.Identifiers;

public class JobIdentifier
{
    [Display("Job ID")]
    [DataSource(typeof(JobDataSourceHandler))]
    public string JobId { get; set; }
}