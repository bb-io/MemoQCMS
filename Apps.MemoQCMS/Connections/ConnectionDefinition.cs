using Apps.MemoQCMS.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.MemoQCMS.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "Connection key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.ConnectionKey) { DisplayName = "Connection key", Sensitive = true },
                new(CredsNames.BaseUrl) { DisplayName = "Base URL" }
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        var connectionKey = values.First(v => v.Key == CredsNames.ConnectionKey);
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            connectionKey.Key,
            connectionKey.Value
        );

        var baseUrl = new Uri(values.First(v => v.Key == CredsNames.BaseUrl).Value).GetLeftPart(UriPartial.Authority) +
                      "/memoqservercmsgateway/v1";
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            CredsNames.BaseUrl,
            baseUrl
        );
    }
}