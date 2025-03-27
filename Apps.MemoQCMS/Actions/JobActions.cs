using System.IO.Compression;
using System.Net.Mime;
using System.Text;
using Apps.MemoQCMS.Constants;
using Apps.MemoQCMS.Models;
using Apps.MemoQCMS.Models.Dtos;
using Apps.MemoQCMS.Models.Identifiers;
using Apps.MemoQCMS.Models.Requests.Jobs;
using Apps.MemoQCMS.Models.Responses.Jobs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Newtonsoft.Json;
using RestSharp;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Apps.MemoQCMS.DataSourceHandlers;


namespace Apps.MemoQCMS.Actions;

[ActionList]
public class JobActions : MemoQCMSInvocable
{
    private readonly IFileManagementClient _fileManagementClient;
    
    public JobActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
        : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
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
        // memoQ CMS does not accept these characters
        var name = input.Name.Replace("/", "_").Replace("\\", "_").Replace(":", "_");
        if (string.IsNullOrEmpty(input.Name))
        {
            throw new PluginMisconfigurationException("Input name is missing, make sure that the input values are correct."); 
        }
        if (string.IsNullOrEmpty(input.SourceLanguage))
        {
            throw new PluginMisconfigurationException("Source language is missing, make sure that the input values are correct.");

        }
        if (string.IsNullOrEmpty(input.TargetLanguage))
        {
            throw new PluginMisconfigurationException("Target language is missing, make sure that the input values are correct.");

        }

        var dataSourceContext = new DataSourceContext();
        var targetLanguageHandler = new TargetLanguageDataSourceHandler(InvocationContext);
        var allowedTargetLanguages = await targetLanguageHandler.GetDataAsync(dataSourceContext, CancellationToken.None);

        if (!allowedTargetLanguages.ContainsKey(input.TargetLanguage))
        {
            throw new PluginMisconfigurationException(
                $"Target language '{input.TargetLanguage}' is not allowed for this account. Please chekc your input and try again");
        }

        using (var httpClient = new HttpClient())
        {
            var connectionKey = InvocationContext.AuthenticationCredentialsProviders.Get(CredsNames.ConnectionKey).Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"CMSGATEWAY-API {connectionKey}");
            
            using (var content = new MultipartFormDataContent())
            {
                var fileStream = await _fileManagementClient.DownloadAsync(file.File);
                var fileBytes = await fileStream.GetByteData();
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.Add("Content-Type", file.File.ContentType);
                content.Add(fileContent, "file", file.File.Name);
            
                var translationJob = new
                {
                    Name = name,
                    SourceLang = input.SourceLanguage,
                    TargetLang = input.TargetLanguage,
                    FileType = Path.GetExtension(file.File.Name).TrimStart('.')
                };
                var translationJobJson = JsonConvert.SerializeObject(translationJob);
                content.Add(new StringContent(translationJobJson, Encoding.UTF8, "application/json"), "translationJob");
        
                var url = InvocationContext.AuthenticationCredentialsProviders.Get(CredsNames.BaseUrl).Value +
                          $"/orders/{orderIdentifier.OrderId}/jobs";
                using (var response = await httpClient.PostAsync(url, content))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var job = JsonConvert.DeserializeObject<JobDto>(result);
                        return job;
                    }
                    
                    var error = JsonConvert.DeserializeObject<ErrorDto>(result);

                    if (error?.ErrorCode == "TranslationJobSourceLangIsInvalid" || error?.ErrorCode == "TranslationJobTargetLangIsInvalid")
                    {
                        throw new PluginMisconfigurationException(error.Message);
                    }

                    throw new PluginApplicationException(error.Message);
                }
            }
        }
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
        
        var originalFilename = translationResponse?.ContentHeaders?.First(header => header.Name == "Content-Disposition")?.Value?
                                                                 .ToString()?
                                                                 .Split("filename=")[^1];
        var originalExtension = Path.GetExtension(originalFilename);
        var actualExtension = originalExtension == ".pdf" ? ".docx" : originalExtension;

        var filename =
            $"{string.Join('_', job.Name.Split(" "))}_{Path.GetFileNameWithoutExtension(originalFilename)}{actualExtension}";
        var decompressedTranslation = DecompressGZipFile(translationResponse.RawBytes);

        if (!MimeTypes.TryGetMimeType(filename, out var contentType))
            contentType = MediaTypeNames.Application.Octet;

        using var stream = new MemoryStream(decompressedTranslation);
        var file = await _fileManagementClient.UploadAsync(stream, contentType, filename);
        return new() { File = file };
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