using RestSharp;

namespace Apps.MemoQCMS.Api;

public class MemoQCMSRequest : RestRequest
{
    public MemoQCMSRequest(string endpoint, Method method) : base(endpoint, method)
    {
    }
}