using System.IO.Compression;
using System.Net.Mime;
using Apps.MemoQCMS.Api;
using Apps.MemoQCMS.Models;
using Apps.MemoQCMS.Models.Dtos;
using Apps.MemoQCMS.Models.Identifiers;
using Apps.MemoQCMS.Models.Requests.Jobs;
using Apps.MemoQCMS.Models.Responses.Jobs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.MemoQCMS.Actions;

[ActionList]
public class JobActions : MemoQCMSInvocable
{
    public JobActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }
    
    [Action("List jobs", Description = "List the translation jobs of the specified order.")]
    public async Task<ListJobsResponse> ListJobs([ActionParameter] OrderIdentifier orderIdentifier)
    {
        var jobs = await ExecuteRequestAsync<IEnumerable<JobDto>>($"/orders/{orderIdentifier.OrderId}/jobs", Method.Get);
        return new(jobs);
    }
    
    [Action("Get job", Description = "Retrieve information about a job.")]
    public async Task<JobDto> GetJob([ActionParameter] OrderIdentifier orderIdentifier, 
        [ActionParameter] JobIdentifier jobIdentifier)
    {
        var job = await ExecuteRequestAsync<JobDto>($"/jobs/{jobIdentifier.JobId}", Method.Get);
        return job;
    }

    [Action("Create job", Description = "Create a new translation job in the specified order.")]
    public async Task<JobDto> CreateJob([ActionParameter] OrderIdentifier orderIdentifier,
        [ActionParameter] CreateJobRequest input, [ActionParameter] FileWrapper file)
    {
        var request = new MemoQCMSRequest($"/orders/{orderIdentifier.OrderId}/jobs", Method.Post);
        request
            .AddFile("file", file.File.Bytes, file.File.Name)
            .AddParameter("translationJob", JsonConvert.SerializeObject(new
            {
                input.Name,
                SourceLang = input.SourceLanguage,
                TargetLang = input.TargetLanguage,
                FileType = Path.GetExtension(file.File.Name).TrimStart('.')
            }));

        var job = await Client.ExecuteWithErrorHandling<JobDto>(request);
        return job;
    }

    [Action("Deliver job", Description = "Change the status of a job to \"Delivered\".")]
    public async Task<JobIdentifier> DeliverJob([ActionParameter] OrderIdentifier orderIdentifier, 
        [ActionParameter] JobIdentifier jobIdentifier)
    {
        var requestBody = new { NewStatus = "DeliveredToSource" };
        await ExecuteRequestAsync($"/jobs/{jobIdentifier.JobId}", Method.Patch, requestBody);
        return jobIdentifier;
    }

    [Action("Download translation", Description = "Get the translation for the specified translation job.")]
    public async Task<FileWrapper> DownloadTranslation([ActionParameter] OrderIdentifier orderIdentifier, 
        [ActionParameter] JobIdentifier jobIdentifier)
    {
        var job = await ExecuteRequestAsync<JobDto>($"/jobs/{jobIdentifier.JobId}", Method.Get);
        var translationResponse = await ExecuteRequestAsync($"/jobs/{jobIdentifier.JobId}/translation", Method.Get);
        
        var originalFilename = translationResponse.ContentHeaders.First(header => header.Name == "Content-Disposition")
            .Value.ToString().Split("filename=")[^1];
        var originalExtension = Path.GetExtension(originalFilename);
        var actualExtension = originalExtension == ".pdf" ? ".docx" : originalExtension;

        var filename =
            $"{string.Join('_', job.Name.Split(" "))}_{Path.GetFileNameWithoutExtension(originalFilename)}{actualExtension}";
        var decompressedTranslation = DecompressGZipFile(translationResponse.RawBytes);

        if (!MimeTypes.TryGetMimeType(filename, out var contentType))
            contentType = MediaTypeNames.Application.Octet;
        
        return new()
        {
            File = new(decompressedTranslation)
            {
                Name = filename,
                ContentType = contentType
            }
        };
    }
    
    private static byte[] DecompressGZipFile(byte[] compressedData)
    {
        using (var compressedStream = new MemoryStream(compressedData))
        {
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    gzipStream.CopyTo(decompressedStream);
                    return decompressedStream.ToArray();
                }
            }
        }
    }
}