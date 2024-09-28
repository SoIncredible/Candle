using System.Collections;
// using UI.UIFramework.Core.Runtime;
using UnityEngine;

namespace Runtime
{
    public class Launcher : MonoBehaviour
    {
        private IEnumerator Start()
        {
            GameManager.Create();
            yield return null;
            GameManager.Instance.Init();
            yield return null;
            // GameManager.Instance.UIManager.OpenPage(UIType.Home);
        }
    }
}