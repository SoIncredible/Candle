// using UI.UIFramework.Core.Runtime;
using Unity.VisualScripting;
using UnityEngine;

namespace Runtime
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // public UIManager UIManager { get; private set; }

        public static void Create()
        {
            if (Instance != null)
            {
                Debug.LogError("[GameManager] Can not create duplicated GameManager Instance !!!");
                return;
            }

            GameObject gameObject = new GameObject("GameManager");
            Instance = gameObject.AddComponent<GameManager>();
            DontDestroyOnLoad(gameObject);
        }

        public void Destroy()
        {
        }

        public void Init()
        {
            // UIManager = Instance.AddComponent<UIManager>();
            // UIManager.Init();
        }

        public void Release()
        {
            // UIManager.Release();
        }
    }
}