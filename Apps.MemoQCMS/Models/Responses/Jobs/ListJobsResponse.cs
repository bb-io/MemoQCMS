using Apps.MemoQCMS.Models.Dtos;

namespace Apps.MemoQCMS.Models.Responses.Jobs;

public record ListJobsResponse(IEnumerable<JobDto> Jobs);