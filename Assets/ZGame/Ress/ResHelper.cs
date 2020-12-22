using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResHelper : Singleton<ResHelper>
{
    public GameObject LoadModel(string path)
    {
        GameObject p = Resources.Load(path) as GameObject;
        var g = GameObject.Instantiate(p);
        return g;
    }

    public void LoadModelAsync(string path, Action<GameObject> onLoaded)
    {
        ResourceRequest rr = Resources.LoadAsync(path);
        rr.completed += (ao) =>
        {
            if (ao.isDone)
            {
                GameObject p = (rr.asset) as GameObject;
                GameObject g = GameObject.Instantiate(p);
                if (onLoaded != null)
                {
                    onLoaded(g);
                }
            }
        };
    }
}
