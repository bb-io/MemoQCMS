using Blackbird.Applications.Sdk.Common;

namespace Apps.MemoQCMS.Models.Dtos;

public class JobDto
{
    [Display("Job")]
    public int TranslationJobId { get; set; }
    
    public string Name { get; set; }
    
    public string Url { get; set; }
    
    [Display("Source language")]
    public string SourceLang { get; set; }
    
    [Display("Target language")]
    public string TargetLang { get; set; }
    
    [Display("File type")]
    public string FileType { get; set; }
    
    public string Status { get; set; }
    
    [Display("Order")]
    public string OrderId { get; set; }
}