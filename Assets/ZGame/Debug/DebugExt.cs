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
        static string fileName = "StarTopDebug";
        static bool isInit = false;
        static string pPath = Application.persistentDataPath;
        static void Init()
        {
            string path = pPath + "/" + fileName + ".txt";
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
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
            string path = pPath + "/" + fileName + ".txt";
            IOTools.WriteString(path, contents);
            byte[] data = IOTools.ReadFile(path);
            //和后端协议的表单字段为 file
            HttpTool.UploadFile(url, "file", data, fileName + "_" + fileNameExt + "_" + TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now) + ".txt", (str) =>
              {
                  Log(str);
                  onSuccess?.Invoke();
              }, (str) =>
              {
                  LogE(str);
                  onFail?.Invoke();
              });
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
                    string path = pPath + "/" + fileName + ".txt";
                    IOTools.WriteString(path, contents);
                }
            }
        }
    }
}