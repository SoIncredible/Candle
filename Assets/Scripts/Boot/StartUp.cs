using System.Collections;
using UniFramework.Event;
using UnityEngine;
using YooAsset;

namespace Boot
{
    public class StartUp : MonoBehaviour
    {
        /// <summary>
        /// 资源系统运行模式
        /// </summary>
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
        
        private void Awake()
        {
            Debug.Log($"资源系统运行模式：{PlayMode}");
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            DontDestroyOnLoad(gameObject);
        }

        private IEnumerator Start()
        {
            GameManager.Instance.Startup();
            // TODO Eddie NetManager.StartUp
            
            // 初始化事件系统
            UniEvent.Initalize();
            YooAssets.Initialize();
            
            // 开始补丁更新流程
            var operation = new PatchOperation("DefaultPackage", PlayMode);
            YooAssets.StartOperation(operation);
            yield return operation;

            // 设置默认的资源包
            var gamePackage = YooAssets.GetPackage("DefaultPackage");
            YooAssets.SetDefaultPackage(gamePackage);
            ResourceManager.Instance.Startup();
            
            // 切换到主页面场景
            SceneEventDefine.ChangeToHomeScene.SendEventMessage();
        }
    }
}