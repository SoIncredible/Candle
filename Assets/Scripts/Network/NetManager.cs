using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Network
{
    public static class NetManager
    {
        private static Socket _socket;
        
        // 接收缓冲区
        private static byte[] _readBuffer = new byte[1024];
        
        // 委托类型
        public delegate void MsgListener(string str);
        
        // 监听列表
        private static Dictionary<string, MsgListener> _listeners = new Dictionary<string, MsgListener>();
        
        // 消息列表
        private static List<string> msgList  = new List<string>();


        public static void Update()
        {
            if (msgList.Count <= 0)
            {
                return;
            }
            
            var msg = msgList[0];
            msgList.RemoveAt(0);
            var split = msg.Split('|');
            var msgName = split[0];
            var msgArgs = split[1];
            if (_listeners.ContainsKey(msgName))
            {
                _listeners[msgName](msgArgs);
            }
        }
        
        /// <summary>
        /// 连接
        /// </summary>
        public static void Connect(string ip, int port)
        {
            if (_socket != null && !_socket.Connected)
            {
                Debug.LogError("[NetManager]已经连接了!");
            }
            else
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(ip, port);
                _socket.BeginReceive(_readBuffer, 0, 1024, 0, ReceiveCallback, _socket);
            }
        }

        /// <summary>
        /// 发送消息给服务端
        /// </summary>
        public static void Send(string sendSte)
        {
            if (_socket == null)
            {
                Debug.LogError("[NetManager] 发送消息失败! 连接服务器失败!");
                return;
            }

            if (!_socket.Connected)
            {
                Debug.LogError("[NetManager] 发送消息失败! 连接服务器失败!");
                return;
            }
            
            var sendBytes = System.Text.Encoding.UTF8.GetBytes(sendSte);
            _socket.Send(sendBytes);
        }

        public static void AddListener(string msgName, MsgListener listener)
        {
            _listeners[msgName] = listener;
        }

        public static void RemoveListener()
        {
            // TODO Eddie 
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var socket = (Socket)ar.AsyncState;
                var count = socket.EndReceive(ar);
                var receiveStr = System.Text.Encoding.UTF8.GetString(_readBuffer, 0, count);
                msgList.Add(receiveStr);
                _socket.BeginReceive(_readBuffer, 0, 1024, 0, ReceiveCallback, _socket);
            }
            catch (SocketException e)
            {
                Debug.LogError(e.Message);
            }
        }
        
        public static string GetDesc()
        {
            if (_socket == null)
            {
                return string.Empty;                
            }

            if (!_socket.Connected)
            {
                return string.Empty;
            }
            
            return _socket.LocalEndPoint.ToString();
        }
    }
}