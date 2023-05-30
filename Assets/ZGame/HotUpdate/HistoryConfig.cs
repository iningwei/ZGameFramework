using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HistoryConfig
{
    public class History
    {
        public List<string> content;
    }
    public static int maxCount = 10;
    public static string path = Application.persistentDataPath+"/history.json";
    public static Queue<string> historyQueue = new Queue<string>();

    public static void AddHistory(string value)
    {
        
        var oldContent = ReadHistory().content;
        historyQueue.Clear();
        foreach (var item in oldContent)
        {
            historyQueue.Enqueue(item);
        }
        History tt = new History();
        tt.content = new List<string>();
        if (historyQueue.Count == maxCount)
        {
            historyQueue.Dequeue();
        }
        historyQueue.Enqueue(value);
        tt.content = historyQueue.ToList<string>();
        string js = JsonUtility.ToJson(tt);
        StreamWriter sw = new StreamWriter(path);
        sw.WriteLine(js);
        sw.Close();
    }

    public static History ReadHistory()
    {
        StreamReader str = File.OpenText(path);
        string readData = str.ReadToEnd();
        History tt = JsonUtility.FromJson<History>(readData);
        str.Close();
        return tt;
    }

    public static int GetHistoryCount()
    {
        int count = 0;
        count = ReadHistory().content.Count;
        return count;
    }
}
