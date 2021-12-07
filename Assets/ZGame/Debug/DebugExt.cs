using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    public class DebugExt
    {
        public static bool enableTrace = true;
        static Queue<string> queues = new Queue<string>();

        public static void Log(string content)
        {
            Debug.Log(content);
            TraceDebug(TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now) + ":" + content);
        }

        public static void LogW(string content)
        {
            Debug.LogWarning(content);
            TraceDebug(TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now) + ":" + content);
        }
        public static void LogE(string content)
        {
            Debug.LogError(content);
            TraceDebug(TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now) + ":" + content);
        }

        public static void UploadDebugToServer(string url, Action onSuccess, Action onFail)
        {
            string fileName = TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now) + ".txt";
            string contents = "";
            foreach (var item in queues)
            {
                contents += item + "\r\n";
            }
            string path = Application.persistentDataPath + "/" + fileName;
            IOTools.WriteString(path, contents);
            byte[] data = IOTools.ReadFile(path);
            //和后端协议的表单字段为 file
            HttpTool.UploadFile(url, "file", data, fileName, (str) =>
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
            if (enableTrace)
            {
                if (queues.Count == 1000)
                {
                    queues.Dequeue();

                }
                queues.Enqueue(content);
            }
        }
    }
}