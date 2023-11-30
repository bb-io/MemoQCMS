using Apps.MemoQCMS.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MemoQCMS.Models.Requests.Jobs;

public class CreateJobRequest
{
    [Display("Job name")]
    public string Name { get; set; }
    
    [Display("Source language")]
    [DataSource(typeof(SourceLanguageDataSourceHandler))]
    public string SourceLanguage { get; set; }
    
    [Display("Target language")]
    [DataSource(typeof(TargetLanguageDataSourceHandler))]
    public string TargetLanguage { get; set; }
}