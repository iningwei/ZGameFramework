using AYellowpaper.SerializedCollections;
using Microsoft.Cci;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZGame.Ress;
using ZGame.Ress.AB;

namespace ZGame.Window
{
    public class WindowInfo
    {
        public string scriptName;

        /// <summary>
        /// resource name,without suffix
        /// </summary>
        public string resName;

        public WindowInfo(string scriptName, string resName, bool isLuaWindow)
        {

            this.scriptName = scriptName;

            this.resName = resName;

        }
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

        Transform rootCanvasTran = null;
        Canvas rootCanvas = null;
        Camera rootUICamera = null;


        Canvas root3DCanvas = null;

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
                    WindowRegister.Register();
                }
                return windowInfos;
            }
        }


        //Cached windows, all of them's active is false
        public Dictionary<string, Window> cachedWindows = new Dictionary<string, Window>();
        //All opened windows
        public Dictionary<string, Window> openedWindows = new Dictionary<string, Window>();
        //used for visual in editor
        public List<string> visualCachedList = new List<string>();
        public List<string> visualOpenedList = new List<string>();

        //key为layer名，value为该layer下link的窗体堆栈
        //stack在inspector不可见，故用List，方便调试
        public List<string> linkWindows = new List<string>();

        public WindowInfo GetWindowInfo(string windowName)
        {
            WindowInfo info = null;
            WindowInfos.TryGetValue(windowName, out info);
            return info;
        }
        public void Init(Transform launcherNode)
        {
            if (rootCanvasTran == null)
            {
                rootCanvasTran = launcherNode.Find("Canvas").transform;
            }

            if (rootCanvas == null)
            {
                if (rootCanvasTran != null)
                {
                    rootCanvas = rootCanvasTran.GetComponent<Canvas>();
                }
            }
            if (root3DCanvas == null)
            {
                root3DCanvas = launcherNode.Find("Canvas3D")?.GetComponent<Canvas>();
            }
            if (rootUICamera == null)
            {
                rootUICamera = launcherNode.Find("UICamera").GetComponent<Camera>();
            }

        }

        void initLayerDic()
        {
            for (int i = 0; i < WindowLayer.LayerList.Count; i++)
            {
                var layerName = WindowLayer.LayerList[i];
                LayerDic[layerName] = rootCanvasTran.Find(layerName);
            }
        }


        /// <summary>
        /// UI竖屏自适应PAD
        /// </summary>
        public void UIVerticalFitPad()
        {
            //自适应基准分辨率比例为 Config.gameDesignRatio
            //即对于竖屏，当比这个分辨率比例更宽的机型，才对其左右进行黑边填充
            float gapRatio = Config.gameDesignRatio.x / Config.gameDesignRatio.y;
            Debug.Log("gameDesignRatio,x:" + Config.gameDesignRatio.x + ", y:" + Config.gameDesignRatio.y);

            //int screenWidth = Screen.width;
            //int screenHeight = Screen.height;
            //这里不能使用屏幕的宽高来算后续偏移，需要取主Canvas的宽高来算
            float screenWidth = rootCanvasTran.GetComponent<RectTransform>().sizeDelta.x;
            float screenHeight = rootCanvasTran.GetComponent<RectTransform>().sizeDelta.y;
            //需要注意的是，在PC上的软件，若分辨率大于屏幕的分辨率，那么会导致部分不显示。那么这里算出来的值实际上是显示区域的值。并不包含未显示区域的。


            float screenRatio = (float)screenWidth / screenHeight;
            if (screenRatio > gapRatio)
            {
                float suitWidth = gapRatio * screenHeight;
                float offsetX = (screenWidth - suitWidth) / 2;
                Debug.LogError($"do vertical fit pad, screenWidth:{screenWidth},screenHeight:{screenHeight},gapRatio:{gapRatio}, suitWidth:{suitWidth},offsetX:{offsetX}", this);
                //设置内置Layer层的左右偏移
                for (int i = 0; i < WindowLayer.LayerList.Count; i++)
                {
                    var layerName = WindowLayer.LayerList[i];
                    var layerTran = LayerDic[layerName];
                    var rectTran = layerTran.GetComponent<RectTransform>();
                    rectTran.offsetMin = new Vector2(offsetX, 0);
                    rectTran.offsetMax = new Vector2(-offsetX, 0);
                }


                //添加遮罩层，并设置同样的左右偏移值
                GameObject padFitMaskObj = new GameObject();
                padFitMaskObj.name = "padFitMask";
                padFitMaskObj.transform.SetParent(rootCanvasTran);
                RectTransform padFitRectTran = padFitMaskObj.AddComponent<RectTransform>();
                padFitRectTran.localPosition = Vector3.zero;
                padFitRectTran.localScale = Vector3.one;
                padFitRectTran.anchorMin = Vector2.zero;
                padFitRectTran.anchorMax = Vector2.one;
                padFitRectTran.offsetMin = new Vector2(offsetX, 0);
                padFitRectTran.offsetMax = new Vector2(-offsetX, 0);

                //为遮罩层添加左右两边的黑色遮罩
                GameObject leftObj = new GameObject();
                leftObj.transform.SetParent(padFitRectTran);
                RectTransform leftTran = leftObj.AddComponent<RectTransform>();
                leftTran.localScale = Vector3.one;
                leftTran.localPosition = Vector3.zero;

                leftTran.anchorMin = new Vector2(0, 0);
                leftTran.anchorMax = new Vector2(0, 1);
                leftTran.pivot = new Vector2(1, 0.5f);
                leftTran.sizeDelta = new Vector2(5000, 0);
                leftTran.SetSiblingIndex(0);
                leftTran.name = "leftFitPadTran";
                var leftImg = leftTran.gameObject.AddComponent<Image>();
                leftImg.color = Color.black;

                GameObject rightObj = new GameObject();
                rightObj.transform.SetParent(padFitRectTran);
                RectTransform rightTran = rightObj.AddComponent<RectTransform>();
                rightTran.localScale = Vector3.one;
                rightTran.localPosition = Vector3.zero;

                rightTran.anchorMin = new Vector2(1, 0);
                rightTran.anchorMax = new Vector2(1, 1);
                rightTran.pivot = new Vector2(0, 0.5f);
                rightTran.sizeDelta = new Vector2(5000, 0);
                rightTran.SetSiblingIndex(1);
                rightTran.name = "rightFitPadTran";
                var rightImg = rightTran.gameObject.AddComponent<Image>();
                rightImg.color = Color.black;
            }
            else
            {
                Debug.Log("not do vertical fit pad");
            }

        }


        Rect getSafeArea()
        {
            Rect rec = Screen.safeArea;

            return rec;
        }




        /// <summary>
        /// UI竖屏适配 安全区
        /// https://blog.csdn.net/qq_39108767/article/details/114396833
        /// https://zhuanlan.zhihu.com/p/124246847
        /// android碎片化，下述代码可能会在有些机型上有问题：https://zhuanlan.zhihu.com/p/126699544
        /// </summary>
        public void UIVerticalFitSafeArea()
        {
            return;//不需要对层节点设置offset，通过对各个window的unsafeBg来进行偏移实现自适应
            //顶部偏移
            int topOffset = GetVerticalAppSafeAreaTopOffset();
            //底部偏移
            int bottomOffset = GetVerticalAppSafeAreaBottomOffset();


            Debug.Log("safe area, topOffset:" + topOffset + ", bottomOffset:" + bottomOffset);
            if (topOffset > 0 || bottomOffset > 0)
            {
                //设置内置Layer层的上下偏移
                for (int i = 0; i < WindowLayer.LayerList.Count; i++)
                {
                    var layerName = WindowLayer.LayerList[i];
                    var layerTran = LayerDic[layerName];
                    var rectTran = layerTran.GetComponent<RectTransform>();
                    rectTran.offsetMin = new Vector2(rectTran.offsetMin.x, rectTran.offsetMin.y + bottomOffset);
                    rectTran.offsetMax = new Vector2(rectTran.offsetMax.x, rectTran.offsetMax.y - topOffset);
                }
            }
            else
            {
                Debug.Log("no need set safe area！！");
            }
        }


        //经过实测，iphone11顶部非安全区高度为88，底部为68； iphone xs max顶部非安全区高度为132，底部为102
        //特别是iphone xs max若用实际的尺寸顶部会偏移很大，因此这里做个限定:上下非安全区最大高度分别为80、60
        /// <summary>
        /// 对于竖屏应用，获得其安全区顶部偏移
        /// </summary>
        /// <returns></returns>
        public int GetVerticalAppSafeAreaTopOffset()
        {
            int topOffset = (int)(Screen.height - getSafeArea().yMax);

            if (topOffset < 0)
            {
                topOffset = 0;
            }
            if (topOffset > 80)
            {
                topOffset = 80;
            }
            //模拟顶部有偏移
            //topOffset = 88;
            return topOffset;
        }

        /// <summary>
        /// 对于竖屏应用，获得其安全区底部偏移
        /// </summary>
        /// <returns></returns>
        public int GetVerticalAppSafeAreaBottomOffset()
        {
            int bottomOffset = (int)getSafeArea().yMin;
            if (bottomOffset < 0)
            {
                bottomOffset = 0;
            }
            if (bottomOffset > 60)
            {
                bottomOffset = 60;
            }
            //模拟底部有偏移
            //bottomOffset = 38;
            return bottomOffset;
        }

        private void Update()
        {
            if (openedWindows.Count > 0)
            {
                //foreach (var item in openedWindows)
                //{
                //    item.Value.Update();
                //}
                //上述使用foreach遍历，偶尔会报错如下：
                //InvalidOperationException: Collection was modified; enumeration operation may not execute.

                List<string> keys = new List<string>(openedWindows.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    if (openedWindows[keys[i]] != null)
                    {
                        openedWindows[keys[i]].Update();
                    }
                }
            }
        }
        private void FixedUpdate()
        {
            if (openedWindows.Count > 0)
            {

                foreach (var item in openedWindows)
                {
                    item.Value.FixedUpdate();
                }
            }
        }
        private void LateUpdate()
        {
            if (openedWindows.Count > 0)
            {

                foreach (var item in openedWindows)
                {
                    item.Value.LateUpdate();
                }
            }
        }

        public void RegisterCallbackOnWindowHide(string windowName, Action callback)
        {
            var window = GetWindow(windowName);
            if (window != null)
            {
                window.RegisterCallbackOnWindowHide(callback);
            }
        }
        public void RegisterCallbackOnWindowDestroy(string windowName, Action callback)
        {
            var window = GetWindow(windowName);
            if (window != null)
            {
                window.RegisterCallbackOnWindowDestroy(callback);
            }
        }

        public void RegisterWindowType(string name, string scriptName, string resName)
        {
            WindowInfos.TryGetValue(name, out WindowInfo info);
            if (info == null)
            {
                info = new WindowInfo(scriptName, resName, false);
                WindowInfos[name] = info;
            }
            else
            {
                Debug.LogError("error,already registed C# WindowType:" + scriptName);
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
                    return;
                }
                else
                {
                    openedWindows[window.name] = window;
                    visualOpenedList.Add(window.name);
                }
            }
            else
            {
                if (openedWindows.ContainsKey(window.name) == false)
                {
                    return;
                }
                else
                {
                    openedWindows.Remove(window.name);
                    visualOpenedList.Remove(window.name);
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
                    visualCachedList.Add(window.name);
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
                    visualCachedList.Remove(window.name);
                }
            }
        }


        Window getWindowOpened(string windowName)
        {
            openedWindows.TryGetValue(windowName, out Window target);
            return target;
        }

        Window getWindowCached(string windowName)
        {
            cachedWindows.TryGetValue(windowName, out Window target);
            return target;
        }

        public bool IsWindowOpen(string windowName)
        {
            openedWindows.TryGetValue(windowName, out Window target);
            return target == null ? false : true;
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

        public string GetWindowLayer(string windowName)
        {
            var window = GetWindow(windowName);
            if (window != null)
            {
                return window.windowLayer;
            }
            Debug.LogError("can not get layer of window:" + windowName);
            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        Window getExistWindow(string windowName, ref WindowResSource source, out WindowInfo info)
        {
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
            }

            return target;
        }

        public void SendWindowMessage(string windowName, int msgId, params object[] datas)
        {
            var window = getWindowOpened(windowName);
            if (window != null)
            {
                window.HandleMessage(msgId, datas);
            }

        }
        public void SendMessageToAllOpenedWindow(int msgId, params object[] datas)
        {
            foreach (var item in this.openedWindows)
            {
                item.Value.HandleMessage(msgId, datas);
            }
        }


        public void CloseAllWindow()
        {
            List<Window> windows = new List<Window>(this.openedWindows.Values);
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                if (windows[i].neverClose)
                {
                    continue;
                }

                CloseWindow(windows[i].name);
            }
        }


        public void ShowWindow(string name, string layerName, bool isLinkWindow, bool neverClose, bool sync, Action<GameObject> onWindowShowed, params object[] datas)
        {
            if (string.IsNullOrEmpty(layerName))
            {
                Debug.LogError("error, layerName is null");
            }

            WindowResSource source = WindowResSource.Unknown;
            WindowInfo info;
            Window window = getExistWindow(name, ref source, out info);
            if (window == null)
            {
                if (Config.resLoadType == (int)ResLoadType.AssetBundle)
                {
                    ABManager.Instance.LoadWindow(info.resName, (obj) =>
                    {
                        window = createWindow(name, obj as GameObject, info);
                        source = WindowResSource.Newborn;
                        updateOpenedWindows(window, true);


                        showWindow(window, layerName, isLinkWindow, neverClose, onWindowShowed, datas);
                        window.RegisterCallbackOnWindowClose(() =>
                        {
                            this.closeWindow(window);
                        });
                        window.RegisterCallbackOnWindowHide(() =>
                        {
                            this.hideWindow(window);
                        });

                        window.RegisterCallbackOnWindowHideForLink(() =>
                        {
                            this.hideForLinkWindow(window);
                        });
                        window.RegisterCallbackOnWindowBack(() =>
                        {
                            this.backWindow(window);
                        });
                    },
                    sync
                     );
                }
                else if (Config.resLoadType == (int)ResLoadType.Resources)
                {
                    var obj = ResourcesManager.Instance.LoadObj("Window/" + info.resName);
                    window = createWindow(name, obj, info);
                    source = WindowResSource.Newborn;
                    updateOpenedWindows(window, true);
                    showWindow(window, layerName, isLinkWindow, neverClose, onWindowShowed, datas);
                    window.RegisterCallbackOnWindowClose(() =>
                    {
                        this.closeWindow(window);
                    });
                    window.RegisterCallbackOnWindowHide(() =>
                    {
                        this.hideWindow(window);
                    });
                    window.RegisterCallbackOnWindowHideForLink(() =>
                    {
                        this.hideForLinkWindow(window);
                    });
                    window.RegisterCallbackOnWindowBack(() =>
                    {
                        this.backWindow(window);
                    });
                }
                else
                {
                    Debug.LogError("unsupported resLoadType:" + Config.resLoadType);
                }
            }
            else
            {
                if (openedWindows.ContainsKey(window.name))
                {
                    return;
                }
                if (cachedWindows.ContainsKey(window.name))
                {
                    this.ReShowCachedWindow(window.name, layerName, true);
                }
            }
        }



        private void showWindow(Window window, string layerName, bool inLinkWindow, bool neverClose, Action<GameObject> onWindowShowed, params object[] datas)
        {
            window.isLinkWindow = inLinkWindow;
            window.neverClose = neverClose;

            //set parent layer
            GameObject windowObj = window.rootObj;
            windowObj.transform.SetParent(LayerDic[layerName]);

            window.Show(layerName, datas);

            onWindowShowed?.Invoke(windowObj);

        }

        private void addToLinkWindow(string name)
        {
            this.linkWindows.Add(name);
        }

        private Window createWindow(string windowName, GameObject uiObj, WindowInfo info)
        {
            Window target = null;
            Type t = Type.GetType(windowName);
            target = Activator.CreateInstance(t, new object[] { uiObj, windowName }) as Window;

            return target;
        }


        public void CloseWindow(string windowName)
        {
            Window window = getWindowOpened(windowName);
            if (window != null)
            {
                closeWindow(window);
            }
            else
            {
                window = getWindowCached(windowName);
                if (window != null)
                {
                    closeWindow(window);
                }
            }
        }

        public void ClearLinkedWindows()
        {
            while (linkWindows.Count > 0)
            {
                string winName = linkWindows[linkWindows.Count - 1];
                linkWindows.RemoveAt(linkWindows.Count - 1);

                Window w = getWindowOpened(winName);
                this.closeWindow(w);
            }
        }
        void reshowLinkedWindow()
        {
            if (linkWindows.Count > 0)
            {
                string previousName = linkWindows[linkWindows.Count - 1];
                linkWindows.RemoveAt(linkWindows.Count - 1);

                Window w = getWindowOpened(previousName);
                if (w != null)
                {
                    w.rootObj.transform.localScale = Vector3.one;
                }
            }
        }
        void closeWindow(Window window)
        {
            updateCachedWindows(window, false);
            updateOpenedWindows(window, false);

            window.Destroy();
        }


        void hideWindow(Window window)
        {
            updateCachedWindows(window, true);
            updateOpenedWindows(window, false);
            //set parent layer hidden
            GameObject windowObj = window.rootObj;
            windowObj.transform.SetParent(LayerDic[WindowLayer.Hidden]);
            windowObj.SetActive(false);
        }

        public Window HideWindow(string windowName)
        {
            Window window = getWindowOpened(windowName);
            if (window != null)
            {
                this.hideWindow(window);
            }
            return window;
        }
        public Window HideForLinkWindow(string windowName)
        {
            Window window = getWindowOpened(windowName);
            if (window != null)
            {
                this.hideForLinkWindow(window);
            }
            return window;
        }
        void hideForLinkWindow(Window window)
        {
            this.addToLinkWindow(window.name);

            GameObject windowObj = window.rootObj;
            windowObj.transform.localScale = Vector3.zero;
        }

        public void backWindow(Window window)
        {
            if (window.isLinkWindow)
            {
                this.closeWindow(window);
                this.reshowLinkedWindow();
            }
            else
            {
                Debug.LogError(window.name + " not link window,can not call Back");
            }
        }
        public void ReShowCachedWindow(string windowName, string layerName, bool isForbidAnimatorReplay = true)
        {
            var window = getWindowCached(windowName);
            if (window != null)
            {
                //set parent layer
                GameObject windowObj = window.rootObj;
                windowObj.transform.SetParent(LayerDic[layerName]);

                var animators = windowObj.GetComponentsInChildren<Animator>();
                if (animators != null)
                {
                    var count = animators.Length;
                    for (int i = 0; i < count; i++)
                    {
                        if (isForbidAnimatorReplay == false)
                        {
                            animators[i].enabled = true;
                        }
                        else
                        {
                            animators[i].enabled = false;
                        }
                    }
                }

                windowObj.SetActive(true);

                updateCachedWindows(window, false);
                updateOpenedWindows(window, true);
            }
        }



        public Transform GetRootCanvasTran()
        {
            return this.rootCanvasTran;
        }
        public Canvas GetRootCanvas()
        {
            return this.rootCanvas;
        }
        public Canvas GetRoot3DCanvas()
        {
            return this.root3DCanvas;
        }

        public Camera GetRootUICamera()
        {
            return this.rootUICamera;
        }

    }
}