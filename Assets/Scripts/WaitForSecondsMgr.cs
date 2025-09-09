using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class WaitForSecondsMgr
    {
        public static WaitForSecondsMgr Instance = new WaitForSecondsMgr();

        private Dictionary<float, WaitForSeconds> _waitPool = new Dictionary<float, WaitForSeconds>();
        public WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (_waitPool.TryGetValue(seconds, out var wait)) return wait;
            wait = new WaitForSeconds(seconds);
            _waitPool.Add(seconds, wait);
            return wait;
        }
    }
}