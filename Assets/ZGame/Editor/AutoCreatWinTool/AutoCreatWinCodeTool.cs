using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.X509;

namespace ZGame.RessEditor
{
    public class SurfixComponentData
    {
        public string surfix;
        public string componentName;
        public int level;//级别
        public SurfixComponentData(string surfix, string comName, int level)
        {
            this.surfix = surfix;
            this.componentName = comName;
            this.level = level;
        }
    }
    public class AutoCreatWinCodeTool : EditorWindow
    {
        string saveFolder;
        UnityEngine.GameObject windowPrefabObj;

        Dictionary<string, List<Transform>> dic = new Dictionary<string, List<Transform>>();
        string windowName;
        Dictionary<string, Transform> areas = new Dictionary<string, Transform>();
        Dictionary<string, Transform> holders = new Dictionary<string, Transform>();
        Dictionary<string, Transform> nodes = new Dictionary<string, Transform>();

        Dictionary<string, SurfixComponentData> surfixComponentMap = new Dictionary<string, SurfixComponentData>() {
            {"Txt",new SurfixComponentData("Txt","TextMeshProUGUI" ,1) } ,
            {"Btn",new SurfixComponentData("Btn","Button"  ,1) } ,
            {"Input",new SurfixComponentData("Input","TMP_InputField"  ,1) } ,
            {"Img",new SurfixComponentData("Img","Image" ,1)   } ,
            {"RawImg",new SurfixComponentData("RawImg","RawImage" ,2)   } ,
            {"Tran",new SurfixComponentData("Tran","Transform",1 )  } ,
            {"Obj",new SurfixComponentData("Obj","GameObject" ,1) } ,
            {"Area",new SurfixComponentData("Area","Transform",1 )  } ,
            {"Node",new SurfixComponentData("Node","Transform" ,1 )  } ,
            {"Holder",new SurfixComponentData("Holder","Transform" ,1)  } ,
        };


        [MenuItem("工具/强制编译")]
        static void ForcedToCompile()
        {
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        [MenuItem("工具/自动创建窗体脚本并绑定常用逻辑")]
        static void PrefabChanger()
        {
            EditorWindow.GetWindow(typeof(AutoCreatWinCodeTool));
        }
        private void OnGUI()
        {
            windowPrefabObj = EditorGUILayout.ObjectField("窗口预制体：", windowPrefabObj, typeof(UnityEngine.GameObject), false) as GameObject;
            if (windowPrefabObj == null)
                return;
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(windowPrefabObj))
            {
                Debug.LogError("该物体不是预制件,请选择预制件;");
                return;
            }
            if (!windowPrefabObj.name.EndsWith("Window"))
            {
                Debug.LogError("名称应以Window结束");
                return;
            }

            if (GUILayout.Button("Do"))
            {
                windowName = windowPrefabObj.name;
                saveFolder = Application.dataPath + "/../WindowAutoScripts/" + windowName;
                dic.Clear();
                areas.Clear();
                holders.Clear();
                nodes.Clear();

                List<Transform> rootTags = new List<Transform>();
                //
                var childs = windowPrefabObj.GetComponentsInChildren<Transform>();
                foreach (var child in childs)
                {
                    if (child.GetComponent<UIRootTag>())
                    {
                        rootTags.Add(child);
                        if (child.name.EndsWith("Area"))
                        {
                            areas.Add(child.name, child);
                        }
                        if (child.name.EndsWith("Holder"))
                        {
                            holders.Add(child.name, child);
                        }
                        if (child.name.EndsWith("Node"))
                        {
                            nodes.Add(child.name, child);
                        }
                    }
                }
                for (int i = 0; i < rootTags.Count; i++)
                {
                    this.processUIRootTag(rootTags[i]);
                }

                //////打印
                foreach (var item in dic)
                {
                    Debug.Log("----->" + item.Key);
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        Debug.Log(item.Value[i].name);
                    }
                }

                this.outputWindowCode();
                foreach (var item in areas)
                {
                    this.outputaAreaCode(item.Key);
                }
                foreach (var item in holders)
                {
                    this.outputaHolderCode(item.Key);
                    this.outputaNodeCode(item.Key.Substring(0, item.Key.IndexOf("Holder")));
                }
            }
        }

        bool extractFieldStr(Transform tmpTran, out string fieldStr)
        {
            fieldStr = "";
            List<SurfixComponentData> datas = new List<SurfixComponentData>();
            foreach (var item in surfixComponentMap)
            {
                if (tmpTran.name.EndsWith(item.Key))
                {
                    datas.Add(item.Value);
                }
            }

            if (datas.Count > 0)
            {
                SurfixComponentData targetData = datas[0];
                for (int i = 0; i < datas.Count; i++)
                {
                    if (datas[i].level > targetData.level)
                    {
                        targetData = datas[i];
                    }
                }
                fieldStr = $"    public {targetData.componentName} ui_" + tmpTran.name + ";\r\n";
                return true;
            }

            return false;
        }
        private void outputaNodeCode(string nodeName)
        {
            string codeStr = "";
            codeStr += "using TMPro;\r\n";
            codeStr += "using UnityEngine;\r\n";
            codeStr += "using UnityEngine.UI;\r\n";
            codeStr += "using ZGame.Window;\r\n";

            codeStr += "public class " + nodeName + " : Node\r\n";
            codeStr += "{\r\n";

            //属性
            List<Transform> usedTrans = dic[nodeName];
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];
                if (extractFieldStr(tmpTran, out string fieldStr))
                {
                    codeStr += fieldStr;
                }
                //////foreach (var item in surfixComponentMap)
                //////{
                //////    if (tmpTran.name.EndsWith(item.Key))
                //////    {
                //////        codeStr += $"    public {item.Value.componentName} ui_" + tmpTran.name + ";\r\n";
                //////    }
                //////}
            }

            //构造函数
            codeStr += "    public " + nodeName + "(GameObject obj, Transform parent, params object[] paras) : base(obj, parent, paras)\r\n";
            codeStr += "    {\r\n";
            codeStr += "    }\r\n";
            //Init
            codeStr += $"    public override void Init(params object[] paras)\r\n";
            codeStr += "    {\r\n";
            codeStr += $"        base.Init(paras);\r\n";
            codeStr += "    }\r\n";

            //Show
            codeStr += "     public override void Show(params object[] paras)\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.Show(paras);\r\n";
            codeStr += "    }\r\n";

            //AddEventListener
            codeStr += "     public override void AddEventListener()\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.AddEventListener();\r\n";
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"        this.ui_{tmpTran.name}.onClick.AddListener(this.on{tmpTran.name}Clicked);\r\n";
                }
            }
            codeStr += "    }\r\n";

            //按钮点击事件
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"    private void on{tmpTran.name}Clicked()\r\n";
                    codeStr += "    {\r\n";
                    codeStr += "    }\r\n";
                }
            }

            //RemoveEventListener
            codeStr += "     public override void RemoveEventListener()\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.RemoveEventListener();\r\n";
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"        this.ui_{tmpTran.name}.onClick.RemoveAllListeners();\r\n";
                }
            }
            codeStr += "    }\r\n";


            //大括号
            codeStr += "}";

            //打印脚本内容
            Debug.Log(codeStr);
            IOTools.WriteString(saveFolder + $"/{nodeName}.cs", codeStr);
        }

        private void outputaHolderCode(string holderName)
        {
            string codeStr = "";
            codeStr += "using TMPro;\r\n";
            codeStr += "using UnityEngine;\r\n";
            codeStr += "using UnityEngine.UI;\r\n";
            codeStr += "using ZGame.Window;\r\n";

            codeStr += "public class " + holderName + " : NodeHolder\r\n";
            codeStr += "{\r\n";

            //属性
            List<Transform> usedTrans = dic[holderName];
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];
                if (extractFieldStr(tmpTran, out string fieldStr))
                {
                    codeStr += fieldStr;
                }
                //foreach (var item in surfixComponentMap)
                //{
                //    if (tmpTran.name.EndsWith(item.Key))
                //    {
                //        codeStr += $"    public {item.Value.componentName} ui_" + tmpTran.name + ";\r\n";
                //    }
                //}
            }

            //构造函数
            codeStr += "    public " + holderName + "(GameObject holderObj, Window window, GameObject nodeItemObj, params object[] paras) : base(holderObj, window, nodeItemObj, paras)\r\n";
            codeStr += "    {\r\n";
            codeStr += "    }\r\n";
            //Init
            codeStr += $"    public override void Init(params object[] paras)\r\n";
            codeStr += "    {\r\n";
            codeStr += $"        base.Init(paras);\r\n";
            codeStr += "    }\r\n";

            //Show
            codeStr += "     public override void Show(params object[] paras)\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.Show(paras);\r\n";
            codeStr += "    }\r\n";
            //FillItems
            codeStr += "     public override void FillItems()\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.FillItems();\r\n";
            codeStr += "        //TODO:\r\n";
            codeStr += "    }\r\n";



            //AddEventListener
            codeStr += "     public override void AddEventListener()\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.AddEventListener();\r\n";
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"        this.ui_{tmpTran.name}.onClick.AddListener(this.on{tmpTran.name}Clicked);\r\n";
                }
            }
            codeStr += "    }\r\n";

            //按钮点击事件
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"    private void on{tmpTran.name}Clicked()\r\n";
                    codeStr += "    {\r\n";
                    codeStr += "    }\r\n";
                }
            }

            //RemoveEventListener
            codeStr += "     public override void RemoveEventListener()\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.RemoveEventListener();\r\n";
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"        this.ui_{tmpTran.name}.onClick.RemoveAllListeners();\r\n";
                }
            }
            codeStr += "    }\r\n";


            //大括号
            codeStr += "}";

            //打印脚本内容
            Debug.Log(codeStr);
            IOTools.WriteString(saveFolder + $"/{holderName}.cs", codeStr);
        }

        private void outputaAreaCode(string areaName)
        {
            string codeStr = "";
            codeStr += "using TMPro;\r\n";
            codeStr += "using UnityEngine;\r\n";
            codeStr += "using UnityEngine.UI;\r\n";
            codeStr += "using ZGame.Window;\r\n";

            codeStr += "public class " + areaName + " : Area\r\n";
            codeStr += "{\r\n";

            //属性
            List<Transform> usedTrans = dic[areaName];
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];
                if (extractFieldStr(tmpTran, out string fieldStr))
                {
                    codeStr += fieldStr;
                }
                //foreach (var item in surfixComponentMap)
                //{
                //    if (tmpTran.name.EndsWith(item.Key))
                //    {
                //        codeStr += $"    public {item.Value.componentName} ui_" + tmpTran.name + ";\r\n";
                //    }
                //}
            }

            //构造函数
            codeStr += "    public " + areaName + "(GameObject obj, Window window, bool initVisible, params object[] paras) : base(obj, window, initVisible, paras)\r\n";
            codeStr += "    {\r\n";
            codeStr += "    }\r\n";
            //Init
            codeStr += $"    public override void Init(params object[] paras)\r\n";
            codeStr += "    {\r\n";
            codeStr += $"        base.Init(paras);\r\n";
            codeStr += "    }\r\n";

            //Show
            codeStr += "     public override void Show(params object[] paras)\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.Show(paras);\r\n";
            codeStr += "    }\r\n";
            //AddEventListener
            codeStr += "     public override void AddEventListener()\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.AddEventListener();\r\n";
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"        this.ui_{tmpTran.name}.onClick.AddListener(this.on{tmpTran.name}Clicked);\r\n";
                }
            }
            codeStr += "    }\r\n";

            //按钮点击事件
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"    private void on{tmpTran.name}Clicked()\r\n";
                    codeStr += "    {\r\n";
                    codeStr += "    }\r\n";
                }
            }

            //RemoveEventListener
            codeStr += "     public override void RemoveEventListener()\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.RemoveEventListener();\r\n";
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"        this.ui_{tmpTran.name}.onClick.RemoveAllListeners();\r\n";
                }
            }
            codeStr += "    }\r\n";


            //大括号
            codeStr += "}";

            //打印脚本内容
            Debug.Log(codeStr);
            IOTools.WriteString(saveFolder + $"/{areaName}.cs", codeStr);
        }

        private void outputWindowCode()
        {
            string codeStr = "";
            codeStr += "using TMPro;\r\n";
            codeStr += "using UnityEngine;\r\n";
            codeStr += "using UnityEngine.UI;\r\n";
            codeStr += "using ZGame.Window;\r\n";

            codeStr += "public class " + windowName + " : Window\r\n";
            codeStr += "{\r\n";

            //属性
            List<Transform> usedTrans = dic[windowName];
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];
                if (extractFieldStr(tmpTran, out string fieldStr))
                {
                    codeStr += fieldStr;
                }
                //foreach (var item in surfixComponentMap)
                //{
                //    if (tmpTran.name.EndsWith(item.Key))
                //    {
                //        codeStr += $"    public {item.Value.componentName} ui_" + tmpTran.name + ";\r\n";
                //    }
                //}
            }

            //areas 
            foreach (var item in areas)
            {
                codeStr += $"    {item.Key} {item.Key.FirstCharToLower()};\r\n";
            }

            //holders 
            foreach (var item in holders)
            {
                codeStr += $"    {item.Key} {item.Key.FirstCharToLower()};\r\n";
            }


            //构造函数
            codeStr += "    public " + windowName + "(GameObject obj, string windowName) : base(obj, windowName)\r\n";
            codeStr += "    {\r\n";
            codeStr += "    }\r\n";
            //Init
            codeStr += $"    public override void Init(string windowName, GameObject obj)\r\n";
            codeStr += "    {\r\n";
            codeStr += $"        base.Init(windowName, obj);\r\n";
            foreach (var item in areas)
            {
                codeStr += $"        {item.Key.FirstCharToLower()} = new {item.Key}(ui_{item.Key}.gameObject, this, true);\r\n";
            }
            foreach (var item in holders)
            {
                codeStr += $"        {item.Key.FirstCharToLower()} = new {item.Key}(ui_{item.Key}.gameObject, this,  ui_{item.Value.name}.gameObject);\r\n";
            }
            codeStr += "    }\r\n";

            //Show
            codeStr += "     public override void Show(string layerName, params object[] datas)\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.Show(layerName, datas);\r\n";
            codeStr += "    }\r\n";
            //AddEventListener
            codeStr += "     public override void AddEventListener()\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.AddEventListener();\r\n";
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"        this.ui_{tmpTran.name}.onClick.AddListener(this.on{tmpTran.name}Clicked);\r\n";
                }
            }
            codeStr += "    }\r\n";

            //按钮点击事件
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"    private void on{tmpTran.name}Clicked()\r\n";
                    codeStr += "    {\r\n";
                    codeStr += "    }\r\n";
                }
            }

            //RemoveEventListener
            codeStr += "     public override void RemoveEventListener()\r\n";
            codeStr += "    {\r\n";
            codeStr += "        base.RemoveEventListener();\r\n";
            for (int i = 0; i < usedTrans.Count; i++)
            {
                Transform tmpTran = usedTrans[i];

                if (tmpTran.name.EndsWith("Btn"))
                {
                    codeStr += $"        this.ui_{tmpTran.name}.onClick.RemoveAllListeners();\r\n";
                }
            }
            codeStr += "    }\r\n";


            //大括号
            codeStr += "}";

            //打印脚本内容
            Debug.Log(codeStr);
            IOTools.WriteString(saveFolder + $"/{windowName}.cs", codeStr);
        }

        void processUIRootTag(Transform rootTagTran)
        {
            if (rootTagTran.name.EndsWith("Window") ||
                rootTagTran.name.EndsWith("Area") ||
                rootTagTran.name.EndsWith("Holder") ||
                rootTagTran.name.EndsWith("Node"))
            {
                dic[rootTagTran.name] = new List<Transform>();
            }



            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(rootTagTran.transform);

            while (queue.Count > 0)
            {
                Transform current = queue.Dequeue();

                if (isNameSuit(current, rootTagTran))
                {
                    dic[rootTagTran.name].Add(current);
                }
                if (current == rootTagTran || current.GetComponent<UIRootTag>() == null)
                {
                    for (int i = 0; i < current.childCount; i++)
                    {
                        Transform child = current.GetChild(i);

                        queue.Enqueue(child);

                    }
                }
            }

        }
        bool isNameSuit(Transform trans, Transform rootTagTran)
        {
            if (rootTagTran.name.EndsWith("Window"))
            {
                if (trans.name.EndsWith("Txt") ||
              trans.name.EndsWith("Btn") ||
              trans.name.EndsWith("Input") ||
              trans.name.EndsWith("Img") ||
              trans.name.EndsWith("Tran") ||
              trans.name.EndsWith("Obj") ||
              (trans.name.EndsWith("Area") && trans.name != "Text Area") ||//去除InputField下的子物体Text Area的干扰
              trans.name.EndsWith("Node") ||
              trans.name.EndsWith("Holder"))
                {
                    return true;
                }
            }
            if (rootTagTran.name.EndsWith("Area"))
            {
                if (trans.name.EndsWith("Txt") ||
              trans.name.EndsWith("Btn") ||
              trans.name.EndsWith("Input") ||
              trans.name.EndsWith("Img") ||
              trans.name.EndsWith("Tran") ||
              trans.name.EndsWith("Obj") ||
              trans.name.EndsWith("Node") ||
              trans.name.EndsWith("Holder"))
                {
                    return true;
                }
            }
            if (rootTagTran.name.EndsWith("Node") || rootTagTran.name.EndsWith("Holder"))
            {
                if (trans.name.EndsWith("Txt") ||
              trans.name.EndsWith("Btn") ||
              trans.name.EndsWith("Input") ||
              trans.name.EndsWith("Img") ||
              trans.name.EndsWith("Tran") ||
              trans.name.EndsWith("Obj"))
                {
                    return true;
                }
            }

            return false;
        }



    }
}