using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ZGame.Net.Tcp
{
    public class LuaSocketManager
    {
        public int socketId;
        public ClientSocket clientSocket;

        SocketState state = SocketState.UnConnect;
        public BlockingQueue<Message> msgQueue = new BlockingQueue<Message>();


        Action<ushort, byte[]> luaMsgDispatcher;
        Action onConnected;
        Action onDisConnect;
        Action onError;

        public bool IsConnected()
        {
            return clientSocket != null && state == SocketState.Success;
        }

        public bool IsClientSocketConnected()
        {
            if (clientSocket != null)
            {
                return clientSocket.IsConnected();
            }
            return false;
        }

        public void InitLua(Action<ushort, byte[]> msgDispatcher, Action onConnected, Action onDisConnect, Action onError)
        {
            this.luaMsgDispatcher = msgDispatcher;
            this.onConnected = onConnected;
            this.onDisConnect = onDisConnect;
            this.onError = onError;
        }

        public void Update()
        {
            //sgame项目中，当断线后，就不再处理收到的拥堵消息了，其它项目如果要继续处理拥堵消息，这段代码要注释。
            if (IsConnected() == false)
            {
                return;
            }

            if (msgQueue.Count() > 0)
            {
                Message msg = msgQueue.Dequeue();
                if (msg != null)
                {

                    luaMsgDispatcher(msg.mainCmdId, msg.Content);

                }
            }
        }

        public void LateUpdate()
        {
            if (clientSocket == null)
            {
                state = SocketState.Connecting;
                return;
            }
            if (clientSocket.socketId != socketId)
            {
                state = SocketState.UnConnect;
                Close();
                return;
            }

            if (state == SocketState.Connecting)
            {
                if (clientSocket.State == SocketState.Success)
                {
                    state = SocketState.Success;
                    onConnected?.Invoke();
                }

                if (clientSocket.State == SocketState.Close)
                {
                    state = SocketState.Close;
                    clientSocket.Close();
                    clientSocket = null;
                    onDisConnect?.Invoke();
                }

                if (clientSocket.State == SocketState.Error)
                {
                    state = SocketState.Error;
                    clientSocket.Close();
                    clientSocket = null;
                    onError?.Invoke();
                }
            }
            else if (state == SocketState.Success)
            {
                if (clientSocket.State == SocketState.Close)
                {
                    state = SocketState.Close;
                    clientSocket.Close();
                    clientSocket = null;
                    onDisConnect?.Invoke();
                }
                if (clientSocket.State == SocketState.Error)
                {
                    state = SocketState.Error;
                    clientSocket.Close();
                    clientSocket = null;
                    onError?.Invoke();
                }
            }
        }


        public void Connect(string ip, int port, float timeOut)
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
            }
            msgQueue.Clear();
            state = SocketState.Connecting;
            clientSocket = new ClientSocket(++socketId, msgQueue);
            clientSocket.Connect(ip, port, timeOut);
        }





        public void Send(UInt16 mainId, byte[] data)
        {
            if (clientSocket != null)
            {
                clientSocket.Send(mainId, data);
            }
        }



        public void Close()
        {
            if (clientSocket != null)
            {
                socketId++;
                clientSocket.Close();
                clientSocket = null;
                state = SocketState.UnConnect;
            }
        }

        public void Destroy()
        {
            this.Close();
        }
    }
}