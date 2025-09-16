using Singletons;
using YooAsset;

public class ResourceManager : Singleton<ResourceManager>
{
    protected override void Init()
    {
        base.Init();
        YooAssets.Initialize();
    }

    public void LoadSceneSync(string sceneName)
    {
        
    }

    public void LoadSceneAsync(string sceneName)
    {
        
    }
}