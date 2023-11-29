using Blackbird.Applications.Sdk.Common;

namespace Apps.MemoQCMS;

public class MemoQCMSApplication : IApplication
{
    public string Name
    {
        get => "memoQ CMS";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}