using System;
using UnityEngine;


/// <summary>
/// Transform扩展方法
/// </summary>
public static class TransformExt
{
    public static void HideAllChilds(this Transform transform)
    {
        int count = transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public static void DestroyAllChilds(this Transform transform, bool destroyImmediate, Action beforeDestroyFunc = null)
    {
        int count = transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            if (beforeDestroyFunc != null)
            {
                beforeDestroyFunc();
            }

            if (destroyImmediate)
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
            else
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
    public static string GetHierarchy(this Transform transform)
    {
        Transform parent = transform.parent;
        string hierarchyStr = transform.name;
        while (parent != null)
        {
            hierarchyStr = parent.name + "/" + hierarchyStr;
            parent = parent.parent;
        }
        return hierarchyStr;
    }


    public static Component FindComponent(this Transform transform, Type type, string name, bool includeInactive)
    {
        Component[] targets = transform.GetComponentsInChildren(type, includeInactive);
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].name == name)
                {
                    return targets[i];
                }
            }
        }
        return null;
    }


    public static T FindChildComponent<T>(this Transform transform, string path) where T : Component
    {
        T t = null;
        var child = transform.Find(path);
        if (child != null)
        {
            t = child.GetComponent<T>();
        }
        if (t == null)
        {
            Debug.LogError("error ,FindChild of type:" + t.GetType().Name + " failed, path:" + path);
        }
        return t;
    }


    /// <summary>
    /// 获得自身坐标系下某点相对于目标物体坐标系下的坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="localPos"></param>
    /// <param name="correspondTran"></param>
    /// <returns></returns>
    public static Vector3 GetCorrespondPos(this Transform transform, Vector3 localPos, Transform correspondTran)
    {
        Vector3 correspondPos = Vector3.zero;
        var worldPos = transform.TransformPoint(localPos);
        correspondPos = correspondTran.InverseTransformPoint(worldPos);
        return correspondPos;
    }

    //public static Vector3 GetCorrespondPosUGUI()
    //{

    //}



    #region position
    //lua侧若调用obj.transform.position=CS.UnityEngine.Vector3(x,y,z);进行赋值的话，是一个很耗的操作
    //通过这种包装，可以大幅度降低消耗

    /// <summary>
    /// 设置X坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    public static void SetPosX(this Transform transform, float x, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(x, transform.position.y, transform.position.z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
                break;

        }

    }

    /// <summary>
    /// 设置Y坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="y"></param>
    public static void SetPosY(this Transform transform, float y, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(transform.position.x, y, transform.position.z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
                break;
        }

    }

    /// <summary>
    /// 设置Z坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="z"></param>
    public static void SetPosZ(this Transform transform, float z, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(transform.position.x, transform.position.y, z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
                break;

        }

    }

    /// <summary>
    /// 设置X、Y坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetPosXY(this Transform transform, float x, float y, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(x, y, transform.position.z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(x, y, transform.localPosition.z);
                break;

        }

    }

    /// <summary>
    /// 设置Y、Z坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetPosYZ(this Transform transform, float y, float z, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(transform.position.x, y, z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(transform.localPosition.x, y, z);
                break;

        }

    }

    /// <summary>
    /// 设置X、Z坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public static void SetPosXZ(this Transform transform, float x, float z, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(x, transform.position.y, z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(x, transform.localPosition.y, z);
                break;

        }

    }

    public static void SetPosXYZ(this Transform transform, float x, float y, float z, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(x, y, z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(x, y, z);
                break;

        }

    }

    public static void ResetPosition(this Transform transform, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = Vector3.zero;
                break;
            case Space.Self:
                transform.localPosition = Vector3.zero;

                break;

        }
    }

    #endregion

    #region Scale

    public static void ResetScale(this Transform transform)
    {
        transform.localScale = Vector3.one;
    }

    public static void SetScale(this Transform transform, float x, float y, float z)
    {
        transform.localScale = new Vector3(x, y, z);
    }
    #endregion


    /// <summary>
    /// 重置本地position,rotation和scale
    /// </summary>
    /// <param name="transform"></param>
    public static void ResetLocalPRS(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 重置世界坐标系下的position,rotation和scale
    /// </summary>
    /// <param name="transform"></param>
    public static void ResetGlobalPRS(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}
