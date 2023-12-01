using Apps.MemoQCMS.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.MemoQCMS.DataSourceHandlers;

public class SourceLanguageDataSourceHandler : MemoQCMSInvocable, IAsyncDataSourceHandler
{
    public SourceLanguageDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var languagePairs = await ExecuteRequestAsync<IEnumerable<LanguagePairDto>>("/languagePairs", Method.Get);
        return languagePairs
            .Select(pair => pair.SourceLanguageCode)
            .Distinct()
            .Where(sourceLanguage => context.SearchString == null 
                                     || sourceLanguage.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(sourceLanguage => sourceLanguage, sourceLanguage => sourceLanguage);
    }
}