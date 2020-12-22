
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoroutineManager : MonoBehaviour
{
    /// <summary>
    /// 内部辅助类
    /// </summary>
    private class CoroutineTask
    {
        public Int64 Id { get; set; }
        public bool Running { get; set; }
        public bool Paused { get; set; }

        public CoroutineTask(Int64 id)
        {
            Id = id;
            Running = true;
            Paused = false;
        }

        public IEnumerator CoroutineWrapper(IEnumerator co)
        {
            IEnumerator coroutine = co;
            while (Running)
            {
                if (Paused)
                    yield return null;
                else
                {
                    if (coroutine != null && coroutine.MoveNext())
                        yield return coroutine.Current;
                    else
                        Running = false;
                }
            }
            mCoroutines.Remove(Id.ToString());
        }
    }


    private static Dictionary<string, CoroutineTask> mCoroutines;

    static CoroutineManager instance;
    public static CoroutineManager Singleton
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("_CoroutineManager_").AddComponent<CoroutineManager>();
                GameObject.DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
        //////private set { }
    }

    void Awake()
    {
        //////Singleton = this;
        mCoroutines = new Dictionary<string, CoroutineTask>();
    }

    /// <summary>
    /// 启动一个协程
    /// </summary>
    /// <param name="co"></param>
    /// <returns></returns>
    public Int64 AddCoroutine(IEnumerator co)
    {
        if (this.gameObject.activeSelf)
        {
            CoroutineTask task = new CoroutineTask(IdAssginer.GetID(IdAssginer.IdType.CoroutineId));
            mCoroutines.Add(task.Id.ToString(), task);
            StartCoroutine(task.CoroutineWrapper(co));
            return task.Id;
        }
        return -1;
    }

    /// <summary>
    /// 停止一个协程
    /// </summary>
    /// <param name="id"></param>
    public void RemoveCoroutine(Int64 id)
    {
        CoroutineTask task = null;
        mCoroutines.TryGetValue(id.ToString(), out task);
        if (task != null)
        {
            task.Running = false;
            mCoroutines.Remove(id.ToString());
        }
    }

    /// <summary>
    /// 暂停协程的运行
    /// </summary>
    /// <param name="id"></param>
    public void PauseCoroutine(Int64 id)
    {
        CoroutineTask task = null;
        mCoroutines.TryGetValue(id.ToString(), out task);
        if (task != null)
        {
            task.Paused = true;
        }
        else
        {
            Debug.LogError("coroutine: " + id.ToString() + " is not exist!");
        }
    }

    /// <summary>
    /// 恢复协程的运行
    /// </summary>
    /// <param name="id"></param>
    public void ResumeCoroutine(Int64 id)
    {
        CoroutineTask task = null;
        mCoroutines.TryGetValue(id.ToString(), out task);
        if (task != null)
        {
            task.Paused = false;
        }
        else
        {
            Debug.LogError("coroutine: " + id.ToString() + " is not exist!");
        }
    }

    private IEnumerator delayedCallImpl(float delayedTime, Action callback)
    {
        if (delayedTime >= 0)
            yield return new WaitForSeconds(delayedTime);
        callback();
    }

    public long DelayedCall(float delayedTime, Action callback)
    {
        return AddCoroutine(delayedCallImpl(delayedTime, callback));
    }

    void OnDestroy()
    {
        foreach (CoroutineTask task in mCoroutines.Values)
        {
            task.Running = false;
        }
        mCoroutines.Clear();
    }

}
