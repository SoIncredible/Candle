using Singletons;

public class ResourceManager : Singleton<ResourceManager>
{
    protected override void Init()
    {
        base.Init();
    }

    public void LoadSceneSync(string sceneName)
    {
        
    }

    public void LoadSceneAsync(string sceneName)
    {
        
    }
}