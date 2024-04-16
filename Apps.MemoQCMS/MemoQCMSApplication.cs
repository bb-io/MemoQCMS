using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.MemoQCMS;

public class MemoQCMSApplication : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.CatAndTms];
        set { }
    }
    
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