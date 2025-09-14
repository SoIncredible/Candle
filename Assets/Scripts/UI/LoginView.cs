using System;
using Network;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoginView : MonoBehaviour
    {
        public Button LoginButton;

        private void Awake()
        {
            LoginButton.onClick.AddListener(Login);
        }

        private void Login()
        {
            NetManager.Connect("127.0.0.1", 8888);
        }
    }
}