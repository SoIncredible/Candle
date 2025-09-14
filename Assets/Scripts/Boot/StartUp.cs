using System;
using UnityEngine;

namespace Boot
{
    public class StartUp : MonoBehaviour
    {
        private void Awake()
        {
            InitSingletonManagers();
        }

        private void InitSingletonManagers()
        {
            // TODO Eddie NetManager.StartUp
            ResourceManager.Instance.Startup();
        }
    }
}