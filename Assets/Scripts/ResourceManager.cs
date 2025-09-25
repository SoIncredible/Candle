using System.Collections;
using Singletons;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

public class ResourceManager : Singleton<ResourceManager>
{
    protected override void Init()
    {
        base.Init();
        YooAssets.GetPackage("DefaultPackage");
    }
    
    public void LoadSceneSync(string sceneName)
    {
        YooAssets.LoadSceneSync(sceneName);
    }

    // public IEnumerator LoadSceneAsync(string sceneName)
    // {
    //     return YooAssets.LoadSceneAsync(sceneName);
    // }
    //
    // public void LoadSceneAsync(string sceneName)
    // {
    //     var handle = YooAssets.LoadSceneAsync(sceneName);
    //     GameManager.Instance.StartCoroutine(handle);   
    // }

    // public GameObject InstantiateAsync(string prefabName, Transform parent, bool active = true)
    // {
    //     var handle = _package.LoadAssetAsync<GameObject>(prefabName);
    // }
    
    /// <summary>
    /// 异步实例化
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="parent"></param>
    /// <param name="active"></param>
    /// <returns></returns>
    public IEnumerator InstantiatePrefabAsync(string prefabName, Transform parent, bool active = true)
    {
        var handle = YooAssets.LoadAssetAsync<GameObject>(prefabName);
        yield return handle;
        var instHandle = handle.InstantiateAsync(parent, active);
        yield return instHandle;
    }

    /// <summary>
    /// 异步加载Sprite
    /// </summary>
    /// <param name="image"></param>
    /// <param name="imagePath"></param>
    /// <returns></returns>
    public IEnumerator LoadSpriteAsync(Image image, string imagePath)
    {
        var handle = YooAssets.LoadAssetAsync<Sprite>(imagePath);
        yield return handle;
        var sprite = handle.AssetObject as Sprite;
        image.sprite = sprite;
    }

    /// <summary>
    /// 同步加载Sprite
    /// </summary>
    /// <param name="image"></param>
    /// <param name="imagePath"></param>
    public void LoadSpriteSync(Image image, string imagePath)
    {
        var handle = YooAssets.LoadAssetSync<Sprite>(imagePath);
        image.sprite = handle.AssetObject as Sprite;;
    }
}