using System;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class NetTest : MonoBehaviour
    {
        [SerializeField] private Button btn;
        [SerializeField] private Button btn2;
        
        private void Awake()
        {
            btn.onClick.AddListener(OnClickConnectBtn);
            btn2.onClick.AddListener(OnClickCloseConnectBtn);
            NetManager.AddEventListener(NetEvent.ConnectSuccess, OnConnectSuccess);
            NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
            NetManager.AddEventListener(NetEvent.ConnectClose, OnConnectClose);
        }

        private void OnClickConnectBtn()
        {
            NetManager.Connect("127.0.0.1", 8888);
        }

        private void OnClickCloseConnectBtn()
        {
            NetManager.Close();
        }
        
        private void OnConnectSuccess(string err)
        {
            Debug.Log("Connect Success ");
        }

        private void OnConnectFail(string err)
        {
            Debug.Log("Connect Fail ");
        }
        
        private void OnConnectClose(string err)
        {
            Debug.Log("Connect Close ");
        }
    }
}