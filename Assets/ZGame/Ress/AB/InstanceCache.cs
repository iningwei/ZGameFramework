using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using ZGame.Ress;

public class InstanceCache
{
    struct CacheItem
    {
        public GameObject obj;
        public float endTime;

        public CacheItem(GameObject obj,float endTime)
        {
            this.obj = obj;
            this.endTime = endTime;
        }

        public void Destroy()
        {
            GameObjectHelper.DestroyImmediate((UnityEngine.Object)this.obj);
        }
    }

    string name;
    Transform root;
    List<CacheItem> caches = new List<CacheItem>();

    const float IDLE_DURATION = 30;

    public InstanceCache(string name,Transform root)
    {
        this.name = name;
        this.root = root;
    }

    public GameObject Get()
    {
        GameObject obj = null;
        if (caches.Count > 0)
        {
            obj = caches[0].obj;
            caches.RemoveAt(0);
        }

        return obj;
    }

    public void Release(GameObject obj)
    {
        obj.transform.SetParent(this.root);
        obj.SetActive(false);
        caches.Add(new CacheItem(obj,Time.time + IDLE_DURATION));
    }

    public void Update()
    {
        int count = caches.Count;
        for (int i = count - 1; i > -1; i--)
        {
            if (caches[i].endTime < Time.time)
            {
                caches[i].Destroy();
                caches.RemoveAt(i);
            }
        }
    }
}
