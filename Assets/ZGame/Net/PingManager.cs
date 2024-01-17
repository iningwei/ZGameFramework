using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;
using ZGame.TimerTween;

public class PingManager : Singleton<PingManager>
{
    Timer sendTimer;
    long sendTimerId;

    int sendPingIndex = 0;
    int rcvPingIndex = 0;
    long curPingTimeStamp;
    float intervalTime;

    byte pingStatus = 0;//1为开，0为关


    ProtobufMsgID c2sMsgId;
    ProtobufMsgID s2cMsgId;

    Func<int, long, IMessage> c2sAssemble;
    Func<byte[], int> s2cHandle;
    public void StartPing(ProtobufMsgID c2sID, ProtobufMsgID s2cID, Func<int, long, IMessage> c2sAssemble, Func<byte[], int> s2cHandle, float pingIntervalTime = 10)
    {
        Debug.Log("start Ping");
        ProtobufMessage.AddListener(s2cID, onS2CPing);

        this.c2sMsgId = c2sID;
        this.s2cMsgId = s2cID;
        this.c2sAssemble = c2sAssemble;
        this.s2cHandle = s2cHandle;

        intervalTime = pingIntervalTime;
        rcvPingIndex = sendPingIndex;

        sendTimer = TimerTween.Repeat(intervalTime, () =>
        {
            C2SPing();
            return true;
        }, true).SetTag("Send-Ping");
        sendTimer.Start(out sendTimerId);

        pingStatus = 1;
    }

    /// <summary>
    /// 发送心跳
    /// </summary>
    private void C2SPing()
    {
        if (rcvPingIndex != sendPingIndex)
        {
            pingTimeError();
            return;
        }
        sendPingIndex++;
        curPingTimeStamp = TimeTool.GetNowStamp();

        if (c2sAssemble != null)
        {
            SocketManager.Instance.Send(c2sMsgId, c2sAssemble(sendPingIndex, curPingTimeStamp));
        }
    }

    /// <summary>
    /// 收到心跳响应
    /// </summary>
    /// <param name="data"></param>
    private void onS2CPing(byte[] data)
    {
        if (this.s2cHandle != null)
        {
            rcvPingIndex = this.s2cHandle(data);
        }
    }

    private void pingTimeError()
    {
        Debug.LogError($"心跳出错，准备重连！send:{sendPingIndex}，rcv:{rcvPingIndex}");
        StopPing();

        this.StopPing();
        SocketManager.Instance.Reconnect();
    }

    /// <summary>
    /// 停止心跳
    /// </summary>
    public void StopPing()
    {
        if (pingStatus == 0)
        {
            return;
        }

        Debug.Log("stop ping");
        TimerTween.Cancel(sendTimer, sendTimerId);

        ProtobufMessage.RemoveListener(s2cMsgId, onS2CPing);
        this.c2sAssemble = null;
        this.s2cHandle = null;
        pingStatus = 0;
        sendPingIndex = 0;
    }
}
