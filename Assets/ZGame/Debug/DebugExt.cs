using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZGame
{
    public class DebugExt
    {
        static int maxCount = 200;

        static Queue<string> queues = new Queue<string>();
        static string productName;
        static string deviceName;
        static bool isInit = false;
        static string pPath = Application.persistentDataPath;
        static string debugFilePath;
        static void Init()
        {
            productName = Application.productName;
            deviceName = SystemInfo.deviceName;
            debugFilePath = pPath + "/" + productName + "_" + deviceName + ".txt";


            if (File.Exists(debugFilePath))
            {
                string[] lines = File.ReadAllLines(debugFilePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    queues.Enqueue(lines[i]);
                }
            }
            isInit = true;
        }
        public static void Log(string content)
        {
            Debug.Log(TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now, true) + ":" + content);
            TraceDebug(TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now, true) + ":" + content);
        }

        public static void LogW(string content)
        {
            Debug.LogWarning(TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now, true) + ":" + content);
            TraceDebug(TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now, true) + ":" + content);
        }

        public static void LogE(string message, UnityEngine.Object content = null)
        {
            Debug.LogError(TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now, true) + ":" + message, content);
            TraceDebug(TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now, true) + ":" + message);
        }

        public static void UploadDebugToServer(string url, string fileNameExt, Action onSuccess, Action onFail)
        {
            string contents = "";

            foreach (var item in queues)
            {
                contents += item + "\r\n";
            }

            IOTools.WriteString(debugFilePath, contents);
            byte[] data = IOTools.ReadFile(debugFilePath);
            //和后端协议的表单字段为 file
            HttpTool.UploadFile(url, "file", data, productName + "_" + deviceName + "_" + fileNameExt + "_" + TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now) + ".txt", (str) =>
              {
                  Log(str);
                  onSuccess?.Invoke();
              }, (str) =>
              {
                  LogE(str);
                  onFail?.Invoke();
              });
        }

        public static void UploadPCLogFileToServer(string url, Action onSuccess, Action onFail)
        {
            string path = Application.persistentDataPath + "/Player.log";
            if (File.Exists(path))
            {
                var datas = File.ReadAllBytes(path);
                string desFileName = productName + "_" + deviceName + "_" + TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now) + "_Player" + ".txt";
                Debug.Log("desFileName:" + desFileName);

                HttpTool.UploadFile(url, "file", datas, desFileName, (str) =>
                {
                    Log(str);
                    onSuccess?.Invoke();
                }, (str) =>
                {
                    LogE(str);
                    onFail?.Invoke();
                });
            }
            else
            {
                Debug.LogError("not exist file:" + path);
            }
        }


        static void TraceDebug(string content)
        {
            if (isInit == false)
            {
                Init();
            }

            if (Config.isEnableLogTrace)
            {
                if (queues.Count == maxCount)
                {
                    queues.Dequeue();
                }
                queues.Enqueue(content);

                if (Config.isEnableLogRealtimeWriteToLocal)
                {
                    string contents = "";
                    foreach (var item in queues)
                    {
                        contents += item + "\r\n";
                    }
                    string path = pPath + "/" + productName + ".txt";
                    IOTools.WriteString(path, contents);
                }
            }
        }
    }
}