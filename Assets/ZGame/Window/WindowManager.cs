using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZGame.Window
{
    public class WindowInfo
    {
        public Type type;
        public string scriptName;
        public bool isLuaWindow;

        /// <summary>
        /// resource name,without suffix
        /// </summary>
        public string resName;

        public WindowInfo(Type t, string resName, bool isLuaWindow)
        {
            this.type = t;
            this.scriptName = t.Name.ToString();

            this.resName = resName;
            this.isLuaWindow = isLuaWindow;
        }
    }


    public enum WindowResType
    {
        Prefab,
        AssetBundle,
    }

    public enum WindowResSource
    {
        Unknown = 0,//unknown means error occur,such as not get window.
        Opened = 1,
        Cache = 2,
        Newborn = 3,
    }


    public class WindowManager : SingletonMonoBehaviour<WindowManager>
    {
        public Transform Canvas = null;
        //Layer Transforms
        Dictionary<string, Transform> layerDic = null;
        public Dictionary<string, Transform> LayerDic
        {
            get
            {
                if (layerDic == null)
                {
                    layerDic = new Dictionary<string, Transform>();
                    initLayerDic();
                }
                return layerDic;
            }
        }
        Dictionary<string, WindowInfo> windowInfos = null;
        public Dictionary<string, WindowInfo> WindowInfos
        {
            get
            {
                if (windowInfos == null)
                {
                    windowInfos = new Dictionary<string, WindowInfo>();
                    WindowRegister.Regist();
                }
                return windowInfos;
            }
        }

        WindowResType windowResType = WindowResType.Prefab;


        //Cached windows, all of them's active is false
        Dictionary<string, Window> cachedWindows = new Dictionary<string, Window>();

        //All opened window
        Dictionary<string, Window> openedWindows = new Dictionary<string, Window>();


        //used for visual in editor
        public List<string> cached = new List<string>();
        public List<string> opened = new List<string>();

        public WindowInfo GetWindowInfo(string windowName)
        {
            WindowInfo info = null;
            WindowInfos.TryGetValue(windowName, out info);
            return info;
        }

        void initLayerDic()
        {
            if (Canvas == null)
            {
                Canvas = GameObject.Find("Canvas").transform;
            }

            for (int i = 0; i < WindowLayer.LayerList.Count; i++)
            {
                var layerName = WindowLayer.LayerList[i];
                layerDic[layerName] = Canvas.Find(layerName);
            }
        }

        private void Update()
        {
            if (openedWindows.Count > 0)
            {
                foreach (var item in openedWindows)
                {
                    item.Value.Update();
                }
            }
        }

        public void RegisterWindowType(string name, Type type, string resName)
        {
            WindowInfo info = null;
            windowInfos.TryGetValue(name, out info);
            if (info == null)
            {
                info = new WindowInfo(type, resName, false);
                windowInfos[name] = info;
            }
            else
            {
                Debug.LogError("error,already registed WindowType:" + type.ToString());
            }
        }





        /// <summary>
        /// Update opened windows
        /// </summary>
        /// <param name="window"></param>
        /// <param name="addOrDelete">true means add window to opened dic，false means delete window from opened dic</param>
        void updateOpenedWindows(Window window, bool addOrDelete)
        {
            if (addOrDelete)
            {
                if (openedWindows.ContainsKey(window.name))
                {
                    Debug.LogWarning("warning,can not add duplicated window to opened window dic,window name:" + window.name);
                    return;
                }
                else
                {
                    openedWindows[window.name] = window;
                    opened.Add(window.name);
                }
            }
            else
            {
                if (openedWindows.ContainsKey(window.name) == false)
                {
                    Debug.LogWarning("warning,can not delete.for opened windows do not contain this window:" + window.name);
                    return;
                }
                else
                {
                    openedWindows.Remove(window.name);
                    opened.Remove(window.name);
                }
            }

        }


        /// <summary>
        /// update cached windows
        /// </summary>
        /// <param name="window"></param>
        /// <param name="addOrDelete">true means add，false means delete</param>
        void updateCachedWindows(Window window, bool addOrDelete)
        {
            if (addOrDelete)
            {
                if (cachedWindows.ContainsKey(window.name))
                {
                    Debug.LogWarning("warning,can not  add, for cached window dic already have window:" + window.name);
                    return;
                }
                else
                {
                    cachedWindows[window.name] = window;
                    cached.Add(window.name);
                }
            }
            else
            {
                if (cachedWindows.ContainsKey(window.name) == false)
                {
                    return;
                }
                else
                {
                    cachedWindows.Remove(window.name);
                    cached.Remove(window.name);
                }
            }
        }


        Window getWindowOpened(string windowName)
        {
            openedWindows.TryGetValue(windowName, out Window target);
            return target;
        }


        public Window GetWindow(string windowName)
        {
            this.openedWindows.TryGetValue(windowName, out Window target);
            if (target == null)
            {
                this.cachedWindows.TryGetValue(windowName, out target);
            }
            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        Window genTargetWindow(string windowName, ref WindowResSource source)
        {
            WindowInfo info = null;
            WindowInfos.TryGetValue(windowName, out info);
            if (info == null)
            {
                Debug.LogError("error, " + windowName + " is not registed");
                return null;
            }

            Window target = null;
            if (openedWindows.ContainsKey(windowName))
            {
                Debug.LogError("window: " + windowName + ", has already opened!");
                source = WindowResSource.Opened;
                target = openedWindows[windowName];
            }
            else
            {
                if (cachedWindows.ContainsKey(windowName))
                {
                    source = WindowResSource.Cache;
                    target = cachedWindows[windowName];
                }
                else
                {
                    GameObject uiObj = null;

                    if (windowResType == WindowResType.AssetBundle)
                    {
                        //TODO:
                        //uiObj = ABManager.Instance.LoadWindow(info.resName);
                    }
                    else if (windowResType == WindowResType.Prefab)
                    {
                        uiObj = ResHelper.Instance.LoadModel("Window/" + info.resName);
                    }

                    source = WindowResSource.Newborn;
                    Type t = Type.GetType(windowName);
                    target = Activator.CreateInstance(t, new object[] { uiObj, windowName }) as Window;
                }
            }
            if (target == null)
            {
                source = WindowResSource.Unknown;
                Debug.LogError("can not get target window:" + windowName);
            }
            return target;
        }

        public void SendWindowMessage(string windowName, int msgId, params object[] datas)
        {
            var window = getWindowOpened(windowName);
            if (window != null)
            {
                //Debug.LogError("handleWindowMessage,windowName:" + windowName + " msgId:" + msgId);
                window.HandleMessage(msgId, datas);
            }
        }





        /// <param name="name"></param>
        /// <param name="isCache">whether cache, if not,after window close,we will destroy it</param>     
        /// <param name="layerName">layer to host this window</param>
        /// <param name="datas">datas for window init use</param>
        /// <returns></returns>
        public Window ShowWindow(string name, bool isCache, string layerName, params object[] datas)
        {
            WindowResSource source = WindowResSource.Unknown;
            Window window = genTargetWindow(name, ref source);

            window.isCache = isCache;
            if (source == WindowResSource.Cache)
            {
                updateCachedWindows(window, false);
                updateOpenedWindows(window, true);
            }
            else if (source == WindowResSource.Newborn)
            {
                updateOpenedWindows(window, true);
            }


            //set parent layer
            GameObject windowObj = window.rootObj;
            windowObj.transform.SetParent(LayerDic[layerName]);

            window.Show(layerName, datas);
            return window;
        }
        public void ShowWindowAsync(string name, bool isCache, string layerName, Action onWindowShowed, bool forbidUIListener = true, params object[] datas)
        {
            //TODO:forbidUIListener

            WindowRequest wr = genTargetWindowAsync(name);

            wr.onCompleted += (ao) =>
            {
                if (ao.isDone)
                {
                    Window window = wr.asset as Window;
                    window.isCache = isCache;
                    if (wr.source == WindowResSource.Cache)
                    {
                        updateCachedWindows(window, false);
                        updateOpenedWindows(window, true);
                    }
                    else if (wr.source == WindowResSource.Newborn)
                    {
                        updateOpenedWindows(window, true);
                    }

                    GameObject windowObj = window.rootObj;
                    windowObj.transform.SetParent(LayerDic[layerName]);
                    window.Show(layerName, datas);
                    if (onWindowShowed != null)
                    {
                        onWindowShowed();
                    }
                }
            };
        }

        WindowRequest genTargetWindowAsync(string windowName)
        {

            WindowRequest wr = new WindowRequest();
            wr.source = WindowResSource.Unknown;

            ZGame.Res.AsyncOperation ao = new ZGame.Res.AsyncOperation();


            WindowInfos.TryGetValue(windowName, out WindowInfo info);
            if (info == null)
            {
                Debug.LogError("error, " + windowName + " is not registed");
                wr.source = WindowResSource.Unknown;
                return wr;
            }

            Window target = null;
            if (openedWindows.ContainsKey(windowName))
            {
                Debug.LogError("window: " + windowName + ", has already opened!");
                wr.source = WindowResSource.Opened;
                target = openedWindows[windowName];
                wr.asset = target;
                ao.isDone = true;
                wr.OnComplete(ao);
                return wr;
            }
            else
            {
                if (cachedWindows.ContainsKey(windowName))
                {
                    wr.source = WindowResSource.Cache;
                    target = cachedWindows[windowName];
                    wr.asset = target;
                    ao.isDone = true;
                    wr.OnComplete(ao);
                    return wr;
                }
                else
                {
                    return loadWindowAsync(windowName);
                }
            }
        }

        private WindowRequest loadWindowAsync(string windowName)
        {
            WindowRequest wr = new WindowRequest();
            wr.source = WindowResSource.Unknown;

            ZGame.Res.AsyncOperation ao = new ZGame.Res.AsyncOperation();
            wr.source = WindowResSource.Newborn;

            if (windowResType == WindowResType.AssetBundle)
            {
                //uiObj = ABManager.Instance.LoadWindow(info.resName);
            }
            else if (windowResType == WindowResType.Prefab)
            {
                ResHelper.Instance.LoadModelAsync("Window/" + windowName, (uiObj) =>
                 {
                     Type t = Type.GetType(windowName);
                     Window target = Activator.CreateInstance(t, new object[] { uiObj, windowName }) as Window;

                     wr.asset = target;
                     ao.isDone = true;
                     wr.OnComplete(ao);
                 });
            }

            return wr;
        }


        public void CloseWindow(string windowName, bool forceDestroy = false)
        {
            Window window = getWindowOpened(windowName);
            if (window != null)
            {
                if (window.isCache)
                {
                    if (!forceDestroy)
                    {
                        hideWindow(window);
                    }
                    else
                    {
                        destroyWindow(window);
                    }

                }
                else
                {
                    destroyWindow(window);
                }
            }
        }

        void hideWindow(Window window)
        {
            //Debug.Log("hideWindow:" + window.name);
            updateCachedWindows(window, true);
            updateOpenedWindows(window, false);
            //set parent layer hidden
            GameObject windowObj = window.rootObj;
            windowObj.transform.SetParent(LayerDic[WindowLayer.Hidden]);

            window.Hide();
        }
        void destroyWindow(Window window)
        {
            updateCachedWindows(window, false);
            updateOpenedWindows(window, false);


            window.Destroy();
        }




    }
}