using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;
using ZGame.TimerTween;

public class PingManager : Singleton<PingManager>
{
    Timer sendTimer;
    int sendPingIndex = 0;
    int rcvPingIndex = 0;
    long curPingTimeStamp;
    float intervalTime;

    byte pingStatus = 0;//1为开，0为关
    public void StartPing(float pingIntervalTime = 10)
    {
        Debug.Log("start Ping");
        ProtobufMessage.AddListener(ProtobufMsgIDDesUtils.GetIDStr(ProtobufMsgID.S2CPing), onS2CPing);

        intervalTime = pingIntervalTime;
        rcvPingIndex = sendPingIndex;

        sendTimer = TimerTween.Repeat(intervalTime, () =>
        {
            C2SPing();
            return true;
        }, true);
        sendTimer.Start();

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
        TimeTool.GetStamp(DateTime.Now);
        C2SPing p = new C2SPing();
        p.Index = sendPingIndex;
        p.TimeStamp = curPingTimeStamp;
        var des = ProtobufMsgIDDesUtils.GetIDDes(ProtobufMsgID.C2SPing);

        ProtobufMessage.PrintMessage(ProtobufMsgID.C2SPing, p);
        WebSocketManager.Instance.Send(des.msgId, p);
        //StarTopSocketManager.Instance.Send(des.msgId, p);
    }

    /// <summary>
    /// 收到心跳响应
    /// </summary>
    /// <param name="data"></param>
    private void onS2CPing(byte[] data)
    {
        S2CPing msg = S2CPing.Parser.ParseFrom(data);
        ProtobufMessage.PrintMessage(ProtobufMsgID.S2CPing, msg);
        rcvPingIndex = msg.Index;
    }


    private void pingTimeError()
    {
        Debug.LogError($"心跳出错，准备重连！send:{sendPingIndex}，rcv:{rcvPingIndex}");
        StopPing();
        //////WebSocketManager.Instance.Close();
        //////WebSocketManager.Instance.Reconnect();
        StarTopSocketManager.Instance.Close();
        StarTopSocketManager.Instance.Reconnect();
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

        DebugExt.Log("stop ping");
        if (sendTimer != null)
        {
            sendTimer.Cancel();
            sendTimer = null;
        }

        ProtobufMessage.RemoveListener(ProtobufMsgIDDesUtils.GetIDStr(ProtobufMsgID.S2CPing), onS2CPing);
        pingStatus = 0;
        sendPingIndex = 0;
    }


}
