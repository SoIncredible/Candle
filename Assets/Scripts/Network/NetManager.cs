using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// 要解决的问题
    /// 粘包问题 ✅
    /// 大端小端问题 ✅
    /// 线程冲突问题 ✅
    /// 半包问题 ✅
    /// 以及协议接收的缓冲区溢出了 自动扩容的问题 ⭕️
    /// TODO Eddie 需要测试 粘包、半包、线程冲突、大小端问题的处理代码是否生效
    /// </summary>
    public class ByteArray
    {
        public byte[] bytes;
        public int readIdx; // 发送的idx 
        public int writeIdx; // 写入的idx
        public int length => writeIdx - readIdx;

        public ByteArray(byte[] bytes)
        {
            this.bytes = bytes;
            readIdx = 0;
            writeIdx = bytes.Length;
        }
    }
    
    /// <summary>
    /// TODO Eddie 将NetManager改造成Singleton
    /// TODO Eddie 干掉所有的数组Copy操作
    /// </summary>
    public static class NetManager
    {
        public const int ReadBufferLength = 1024;
        public const int SendBufferLength = 1024;
        
        private static Socket _socket;
        
        // 接收缓冲区
        private static byte[] _readBuffer = new byte[ReadBufferLength];
        private static int _readBufferCount; // 有点像是下标的意思
        
        // 发送队列
        private static Queue<ByteArray> _sendQueue = new Queue<ByteArray>();
        
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
            else
            {
                Debug.LogError($"Unknown Msg: {msgName}");
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
                // TODO Eddie 改造成 BeginConnect
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(ip, port);
                _socket.BeginReceive(_readBuffer, 0, ReadBufferLength, 0, ReceiveCallback, _socket);
            }
        }

        /// <summary>
        /// 发送消息给服务端
        /// </summary>
        public static void Send(string sendStr)
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

            // 改用 协议长度+协议内容的方式
            Debug.Log("[NetManager] SendMsg:" + sendStr);            
            // bodyBytes字段与大小端没有关系, 或者说, System.Text.Encoding.UTF8.GetBytes内部帮我们处理了大小端\
            // 所以只对表示协议长度的部分进行大小端的处理就可以了.
            var bodyBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
            var sendBytesLen = (short)bodyBytes.Length;
            var lenBytes = BitConverter.GetBytes(sendBytesLen);
            if (!BitConverter.IsLittleEndian)
            {
                // 这里只转换了表示协议长度的位?
                Debug.Log("[Send] Reverse lenBytes");
                lenBytes.Reverse();
            }
            
            var sendBuffer = lenBytes.Concat(bodyBytes).ToArray();

            int count;

            lock (_sendQueue)
            {
                _sendQueue.Enqueue(new ByteArray(sendBuffer));
                count = _sendQueue.Count;
            }
            
            // 如果当前的发送队列里面只有这一个待发送的数据, 调用Send
            if (count == 1)
            {
                _socket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, SendCallback, _socket);
            }
           
            // _socket.Send(sendBytes);
        }

        /// <summary>
        /// 这段代码蛮体现技术力的.
        /// </summary>
        /// <param name="ar"></param>
        private static void SendCallback(IAsyncResult ar)
        {
            // 拿到这次发送的长度, 根据发送成功的字节数 去除驻留在buffer中的数据
            var sendCount = _socket.EndSend(ar);
            // 如果实际发送的长度 小于应该发送的长度 则需要再次发送数据
            ByteArray ba;
            lock (_sendQueue)
            {
                ba = _sendQueue.First();
            }
            
            ba.readIdx += sendCount;
            if (ba.length == 0) // 说明完整发送了
            {
                lock (_sendQueue)
                {
                    _sendQueue.Dequeue(); // 第一个队列元素出队
                    _sendQueue.TryPeek(out ba);
                }
            }

            if (ba != null) // 发送不完整, 或者发送完整且存在第二条数据.
            {
                _socket.BeginSend(ba.bytes, ba.readIdx, ba.writeIdx, 0, SendCallback, ba);
            }
        }

        public static void AddListener(string msgName, MsgListener listener)
        {
            _listeners[msgName] = listener;
        }

        public static void RemoveListener()
        {
            // TODO Eddie 
        }

        /// <summary>
        /// 接收到的数据有三种情况
        /// 1. 长度小于代表协议长度的位数 这时候什么也不做 等下一次收到新的协议的时候再次尝试解析
        /// 2. 长度大于协议长度位, 但是不足以组成一条消息
        /// 3. 长度大于等于一条完整的消息
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var socket = (Socket)ar.AsyncState;
                // 先把bufferCount数据更新
                var count = socket.EndReceive(ar);
                _readBufferCount += count; // 更新下一次可以接收的地方
                
                OnReceiveData();
                
                _socket.BeginReceive(_readBuffer, _readBufferCount, ReadBufferLength - _readBufferCount, 0, ReceiveCallback, _socket);
            }
            catch (SocketException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private static void OnReceiveData()
        {
            // 长度小于协议长度位数
            if (_readBufferCount <= 2)
            {
                // 啥也不做 把数据写入_bufferCount之后
                return;
            }

            // BitConverter.ToInt16会自动地 只取前两个字节. 将这两个字节转成Int 
            // 遇到大小端问题了
            // 添加字节序处理
            short bodyLength;
            if (!BitConverter.IsLittleEndian)
            {
                bodyLength = (short)((_readBuffer[1] << 8) | _readBuffer[0]);
            }
            else
            {
                bodyLength = BitConverter.ToInt16(_readBuffer, 0);
            }
                
            if (_readBufferCount < 2 + bodyLength)
            {
                return;
            }

            // 如果数据满足了, 那么就要取出来了 然后更新一下_bufferCount
            // 从缓冲里面提取出消息来
            var receiveStr = System.Text.Encoding.UTF8.GetString(_readBuffer, 2, bodyLength);
            Debug.Log($"RecvMsg: {receiveStr}");
            msgList.Add(receiveStr);
                    
            // 缓冲区里面的数据需要更新一下
            var start = 2 + bodyLength;
            _readBufferCount -= start;
            Array.Copy(_readBuffer, start, _readBuffer, 0, _readBufferCount);
            
            OnReceiveData();
        }

        // TODO Eddie 让Socket关闭连接逻辑健壮
        public static void Destroy()
        {
            _socket.Close();
            
            RemoveListener();
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