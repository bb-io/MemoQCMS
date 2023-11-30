using Apps.MemoQCMS.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.MemoQCMS.DataSourceHandlers;

public class TargetLanguageDataSourceHandler : MemoQCMSInvocable, IAsyncDataSourceHandler
{
    public TargetLanguageDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var languagePairs = await ExecuteRequestAsync<IEnumerable<LanguagePairDto>>("/languagePairs", Method.Get);
        return languagePairs
            .Select(pair => pair.TargetLanguageCode)
            .Distinct()
            .Where(targetLanguage => context.SearchString == null 
                                     || targetLanguage.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(targetLanguage => targetLanguage, targetLanguage => targetLanguage);
    }
}