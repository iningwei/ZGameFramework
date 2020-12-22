using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using ZGame.TimerTween;

namespace ZGame.Net.Tcp
{

    public enum SocketState
    {
        UnConnect = 0,
        Connecting,
        Success,
        Error,
        DisConnect
    }

    public class ClientSocket
    {
        public int socketId = 0;
        Socket socket;

        SocketState socketState = SocketState.UnConnect;
        public SocketState State
        {
            get
            {
                return this.socketState;
            }
            set
            {
                Debug.Log("Set Socket State:" + value);
                this.socketState = value;
            }
        }



        CircularBuffer recvBuffer;
        byte[] recvBytes;
        private long timeoutCheckId = -1;
        BlockingQueue<Message> msgQueue;

        public ClientSocket(int id, BlockingQueue<Message> msgQueue)
        {
            this.socketId = id;
            this.msgQueue = msgQueue;
            recvBuffer = new CircularBuffer(1024 * 1024 * 2);
            recvBytes = new byte[1024];
        }

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string getIPv6(string mHost, string mPort);
#endif

        private static string getIPv6ByCS(string mHost, string mPort)
        {
#if UNITY_IOS && !UNITY_EDITOR
		string mIPv6 = getIPv6(mHost, mPort);
		return mIPv6;
#else
            return mHost + "&&ipv4";
#endif
        }

        void getIPType(string serverIp, string serverPorts, out string newServerIp, out AddressFamily mIPType)
        {
            mIPType = AddressFamily.InterNetwork;
            newServerIp = serverIp;
            try
            {
                string mIPv6 = getIPv6ByCS(serverIp, serverPorts);
                if (!string.IsNullOrEmpty(mIPv6))
                {
                    string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
                    if (m_StrTemp != null && m_StrTemp.Length >= 2)
                    {
                        string IPType = m_StrTemp[1];
                        if (IPType == "ipv6")
                        {
                            newServerIp = m_StrTemp[0];
                            mIPType = AddressFamily.InterNetworkV6;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("GetIPv6 error:" + e);
            }

        }


        public void Connect(string ip, int port, float timeout = 5f)
        {
            if (socket != null && socket.Connected)
            {
                Debug.LogError("can not connect socket while socket is connected");
                return;
            }

            if (socket == null || !socket.Connected)
            {
                State = SocketState.Connecting;

                AddressFamily af = AddressFamily.InterNetwork;
                string newIp;
                getIPType(ip.Trim(), port.ToString(), out newIp, out af);
                if (!string.IsNullOrEmpty(newIp))
                {
                    ip = newIp;
                }

                socket = new Socket(af, SocketType.Stream, ProtocolType.Tcp);

                IAsyncResult asyncResult = socket.BeginConnect(ip, port, connectCallback, socket);

                timeoutCheckId = CoroutineManager.Singleton.DelayedCall(timeout,
           () =>
           {
               timeoutCheckId = -1;
               if (State == SocketState.Connecting)
               {
                   State = SocketState.Error;
               }
           });
            }
        }

        void connectCallback(IAsyncResult r)
        {
            try
            {
                if (timeoutCheckId != -1)
                {
                    CoroutineManager.Singleton.RemoveCoroutine(timeoutCheckId);
                    timeoutCheckId = -1;
                }
                socket.EndConnect(r);

                if (!socket.Connected)
                {
                    State = SocketState.Error;
                }
                else
                {
                    State = SocketState.Success;
                    asyncReceive();

                    var pickupMsgThread = new Thread(loopPickupMessage);
                    pickupMsgThread.IsBackground = true;
                    pickupMsgThread.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("connectCallback error:" + ex.ToString());
                State = SocketState.Error;
            }
        }


        bool isHandHeader = true;
        Int16 curBodySize = 0;

        void loopPickupMessage()
        {
            //TODO:i删了吧。。。
            int i = 0;
            while (true)
            {
                Thread.Sleep(3);
                i = 2;
                while (i-- >= 0)
                {
                    bool readRet = false;
                    try
                    {
                        if (isHandHeader)
                        {
                            readRet = readHeader();
                        }
                        else
                        {
                            readRet = readBody();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("loopPickUpMessage error:" + ex.ToString());
                        if (State == SocketState.Connecting)
                        {
                            State = SocketState.Error;
                        }
                        return;
                    }

                    if (!readRet && (State == SocketState.Error || State == SocketState.DisConnect))
                    {
                        recvBuffer.Clear();
                        return;
                    }
                }



            }
        }

        bool readBody()
        {
            byte[] bytes = null;

            if (recvBuffer.Count < curBodySize)
                return false;

            bytes = recvBuffer.ReadBytes(curBodySize);
            processPacket(bytes);
            isHandHeader = true;
            return true;
        }


        byte[] emptyBuf = new byte[0];
        void processPacket(byte[] data)
        {
            if (data == null)
                data = emptyBuf;

            if (msgQueue == null)
                return;

            msgQueue.Enqueue(new Message(data));
        }

        bool readHeader()
        {
            if (recvBuffer.Count < 2)
            {
                return false;
            }

            //len为一个消息包的总长度，即：消息头+消息体的总长度
            //（其中消息头的2个字节长度，记录的是消息包的总长度）
            short len = recvBuffer.ReadInt16();
            if (len != 0)
            {
                curBodySize = IPAddress.NetworkToHostOrder(len);//TODO:blog
                curBodySize = (short)(curBodySize - 2);
                isHandHeader = false;
            }


            return true;
        }

        void asyncReceive()
        {
            if (socket != null && socket.Connected)
            {
                socket.BeginReceive(recvBytes, 0, recvBytes.Length, SocketFlags.None, receiveCallback, socket);
            }
        }

        void receiveCallback(IAsyncResult r)
        {
            try
            {
                if (State != SocketState.Success)
                {
                    return;
                }

                int len = socket.EndReceive(r);
                if (len <= 0)
                {
                    State = SocketState.Error;
                    return;
                }

                recvBuffer.Put(recvBytes, 0, len);
            }
            catch (Exception ex)
            {
                Debug.LogError("receive callback error:" + ex.ToString());
            }


            //continue receive
            asyncReceive();
        }




        public void Send(byte[] data)
        {
            try
            {
                if (State != SocketState.Success)
                {
                    return;
                }

                Int16 totalLen = (short)(data.Length + 2);
                byte[] sendBytes = new byte[totalLen];
                totalLen = IPAddress.HostToNetworkOrder(totalLen);

                //Write header
                Array.Copy(BitConverter.GetBytes(totalLen), 0, sendBytes, 0, 2);
                //Write data
                if (data != null)
                {
                    Array.Copy(data, 0, sendBytes, 2, data.Length);
                }

                socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);

            }
            catch (Exception ex)
            {
                Debug.LogError("send error:" + ex.ToString());
                State = SocketState.Error;
            }
        }

        public void Close()
        {
            State = SocketState.DisConnect;
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {

                State = SocketState.Error;
            }

            try
            {
                socket.Close();
            }
            catch (Exception ex)
            {

                State = SocketState.Error;
            }
        }
    }
}