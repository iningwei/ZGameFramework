using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.Info;

public class BuildInCompRenderInfoSyncOperator : SingletonMonoBehaviour<BuildInCompRenderInfoSyncOperator>
{
    public List<BuildInCompRendererInfo> asyncReadyList = new List<BuildInCompRendererInfo>();

    public List<BuildInCompRendererInfo> asyncProcessList = new List<BuildInCompRendererInfo>();


    int asyncProcessLimit = 35;


    public void Add(BuildInCompRendererInfo info)
    {
        asyncReadyList.Add(info);
    }

    private void Update()
    {
        this.process();
        this.check();
    }

    void process()
    {
        if (asyncProcessList.Count < asyncProcessLimit)
        {
            int gap = asyncProcessLimit - asyncProcessList.Count;
            if (gap < 1)
            {
                return;
            }
            for (int i = 0; i < gap; i++)
            {
                if (asyncReadyList.Count > 0)
                {
                    var info = asyncReadyList[0];
                    if (info != null)
                    {
                        handleRenderInfo(info);
                    }

                    asyncReadyList.RemoveAt(0);
                    asyncProcessList.Add(info);
                }
            }
        }
    }

    void check()
    {
        var count = asyncProcessList.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            var info = asyncProcessList[i];
            asyncProcessList.Remove(info);
        }
    }

    void handleRenderInfo(BuildInCompRendererInfo info)
    {
        Transform childTarget = info.tran;
        for (int k = 0; k < info.refTextures.Count; k++)
        {
            if (info.mat != null && info.refTextures != null && info.refTextures.Count > 0)
            {

                ABManager.Instance.LoadTextureToMat(childTarget, info.refTextures[k].texName, info.refTextures[k].shaderProp, info.mat, false, null);
            }
        }

#if UNITY_EDITOR
        ABManager.Instance.ResetEditorShader(childTarget, info.mat, info.shaderName);
#endif
    }
}


