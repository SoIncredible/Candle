using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// ByteArray的设计蛮体现代码技术力的.
    /// 要解决的问题
    /// 粘包问题 ✅
    /// 大端小端问题 ✅
    /// 线程冲突问题 ✅
    /// 半包问题 ✅
    /// 以及协议接收的缓冲区溢出了 自动扩容的问题 ✅
    /// TODO Eddie 需要测试 粘包、半包、线程冲突、大小端问题的处理代码是否生效
    /// </summary>
    public class ByteArray
    {
        public const int BufferDefaultLength = 1024;
        
        // 初始的大小
        public int initSize;
        
        public byte[] bytes;
        public int readIdx; // 发送的idx 
        public int writeIdx; // 写入的idx

        // 缓冲区的容量
        public int capacity;

        // 剩余空间
        public int remain => capacity - writeIdx;
        
        // 缓冲区中有效数据长度
        public int ValidDataLength => writeIdx - readIdx;
        
        public ByteArray(byte[] bytes)
        {
            this.bytes = bytes;
            readIdx = 0;
            initSize = bytes.Length;
            writeIdx = bytes.Length;
            capacity = bytes.Length;
        }

        public ByteArray(int size = BufferDefaultLength)
        {
            bytes = new byte[size];
            capacity = size;
            initSize = size;
            readIdx = 0;
            writeIdx = 0;
        }

        /// <summary>
        /// 重设尺寸
        /// </summary>
        /// <param name="size"></param>
        public void ReSize(int size)
        {
            if(size < ValidDataLength) return; // 如果要设置的尺寸比当前缓冲区中待发送的数据长度还要小 返回
            if(size < initSize) return; // 如果要设置的尺寸比初始的尺寸要小 返回
            var n = 1;
            while (n < size) n *= 2;
            capacity = n;
            var newBytes = new byte[capacity];
            Array.Copy(bytes, readIdx, newBytes, 0, ValidDataLength);
            bytes = newBytes;
            writeIdx = ValidDataLength;
            readIdx = 0;
        }

        /// <summary>
        /// 检查并移动数据 为了提高remain的长度 能缓冲进来更多的数据
        /// </summary>
        public void CheckAndMoveBytes()
        {
            // 这里将触发移动数据的长度定为8, 如果太长的话 移动数据的操作就会太耗费时间了
            if (ValidDataLength < 8)
            {
                MoveBytes();
            }
        }

        public void MoveBytes()
        {
            Array.Copy(bytes, readIdx, bytes, 0, ValidDataLength);
            writeIdx = ValidDataLength; // 只要发生了移动操作, writeIdx和readIdx要相应变化
            readIdx = 0;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="offset">在bs数组中的下标为offset的位置开始Copy</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Write(byte[] bs, int offset, int count)
        {
            // 先尝试扩展remain
            CheckAndMoveBytes();
            // 如果剩余的空间比要写入进来的数据长度小
            if (remain < count)
            {
                ReSize(ValidDataLength + count);
            }
            Array.Copy(bs, offset, bytes, writeIdx, count);
            writeIdx += count;
            return count;
        }

        /// <summary>
        /// 读取数据
        /// 这个byte[]是个值类型吧, 返回不出去啊❌ byte是值类型, byte[]是引用类型
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(byte[] bs, int offset, int count)
        {
            // 找到length和count中小的那个给到count
            count = Math.Min(ValidDataLength, count);
            Array.Copy(bytes, readIdx, bs, offset, count);
            readIdx += count;
            CheckAndMoveBytes();
            return count;
        }

        /// <summary>
        /// 注意大小端的问题
        /// </summary>
        /// <returns></returns>
        public short ReadInt16()
        {
            // BitConverter.ToInt16会自动地 只取前两个字节. 将这两个字节转成Int 
            // 遇到大小端问题了
            // 添加字节序处理
            short bodyLength;
            // 如果当前机器是大端, 则该机器的ConvertToInt16是不对的, 需要我们手动处理一下
            if (!BitConverter.IsLittleEndian)
            {
                bodyLength = (short)(bytes[readIdx + 1] << 8 | bytes[readIdx]);
            }
            else
            {
                bodyLength = BitConverter.ToInt16(bytes, readIdx);
            }
            
            CheckAndMoveBytes();
            return bodyLength;
        }
        
        /// <summary>
        /// 注意大小端的问题
        /// </summary>
        /// <returns></returns>        
        public int ReadInt32()
        {
            int bodyLength;
            // 如果当前机器是大端 则该机器的ConvertToInt32不能用, 需要我们自己处理
            if (!BitConverter.IsLittleEndian)
            {
                bodyLength = bytes[readIdx + 3] << 24 | bytes[readIdx + 2] << 16 | bytes[readIdx + 1] << 8 | bytes[readIdx];;
            }
            else
            {
                bodyLength = BitConverter.ToInt32(bytes, readIdx);
            }   
            
            CheckAndMoveBytes();
            return bodyLength;
        }
        
    }
    
    /// <summary>
    /// TODO Eddie 将NetManager改造成Singleton
    /// </summary>
    public static class NetManager
    {
        private static Socket _socket;
        
        // 接收缓冲区
        private static ByteArray _readBuffer = new ByteArray();
        
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
                _socket.BeginReceive(_readBuffer.bytes, _readBuffer.writeIdx, _readBuffer.remain, 0, ReceiveCallback, _socket);
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
                var ba = new ByteArray(sendBuffer.Length);
                ba.Write(sendBuffer, 0, sendBuffer.Length);
                _sendQueue.Enqueue(ba);
                // _sendQueue.Enqueue(new ByteArray(sendBuffer)); 这两种方法都可以
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
            
            ba.readIdx += sendCount; // 成功发送的数据长度 更新下一次的buffer中读取的idx
            if (ba.ValidDataLength == 0) // 说明完整发送了
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
            _listeners.Clear();
            _listeners = null;
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
                _readBuffer.writeIdx += count; // 更新下一次可以写入的idx
                
                OnReceiveData();

                // 如果当前缓冲区的剩余数据容量小于8个字节了, 扩容
                if (_readBuffer.remain < 8)
                {
                    _readBuffer.MoveBytes();
                    _readBuffer.ReSize(_readBuffer.capacity * 2);
                }
                _socket.BeginReceive(_readBuffer.bytes, _readBuffer.writeIdx, _readBuffer.remain, 0, ReceiveCallback, _socket);
            }
            catch (SocketException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private static void OnReceiveData()
        {
            // 长度小于协议长度位数
            if (_readBuffer.ValidDataLength <= 2)
            {
                // 啥也不做 把数据写入_bufferCount之后
                return;
            }
            
            // 不可以在ReadInt16里面先更新readIdx, 有可能在下面的if分支里面阻断
            var bodyLength = _readBuffer.ReadInt16();
                
            if (_readBuffer.ValidDataLength < bodyLength)
            {
                return;
            }

            _readBuffer.readIdx += 2;
            // 如果数据满足了, 那么就要取出来了 然后更新一下_bufferCount
            // 从缓冲里面提取出消息来
            var readByte = new byte[bodyLength];
            _readBuffer.Read(readByte, 0, bodyLength);
            
            var receiveStr = System.Text.Encoding.UTF8.GetString(readByte);
            Debug.Log($"RecvMsg: {receiveStr}");
            msgList.Add(receiveStr);
            
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