using FSG.MeshAnimator.ShaderAnimated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Ress;
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
        if (info.meshName != "")
        {
            Mesh mesh = null;
            ABManager.Instance.LoadMesh(info.meshName, (m) =>
            {
                mesh = m;
                if (info.concreteCompRenderer is MeshRenderer)
                {
                    info.tran.GetComponent<MeshFilter>().sharedMesh = mesh;

                    ShaderMeshAnimator sma = info.tran.GetComponent<ShaderMeshAnimator>();
                    if (sma != null)
                    {
                        sma.baseMesh = mesh;
                    }
                }
                else if (info.concreteCompRenderer is SkinnedMeshRenderer)
                {
                    (info.concreteCompRenderer as SkinnedMeshRenderer).sharedMesh = mesh;
                }
                else if (info.concreteCompRenderer is ParticleSystemRenderer)
                {
                    (info.concreteCompRenderer as ParticleSystemRenderer).mesh = mesh;
                }
                //add mesh ref
                MeshRes meshRes = ABManager.Instance.GetCachedRes(ABType.Mesh, info.meshName) as MeshRes;
                meshRes.AddRefTrs(info.tran);

            }, false);
        }

        if (info.matIndex != -1)
        {
            Material renderMat = null;
            ABManager.Instance.LoadMatWithCheck(info.matName, (mat) =>
          {
              renderMat = mat;
              info.concreteCompRenderer.sharedMaterials[info.matIndex] = mat;
          }, info.tran);

            if (renderMat != null)
            {
                Transform childTarget = info.tran;
                //add mat ref 
                MatRes matRes = ABManager.Instance.GetCachedRes(ABType.Material, info.matName) as MatRes;
                matRes.AddRefTrs(childTarget);

                if (info.refTextures != null && info.refTextures.Count > 0)
                {
                    for (int k = 0; k < info.refTextures.Count; k++)
                    {
                        ABManager.Instance.LoadTextureToMat(childTarget, info.refTextures[k].texName, info.refTextures[k].shaderProp, renderMat, false, null);
                    }
                }
                ABManager.Instance.ResetEditorShader(childTarget, renderMat, info.shaderName);

            }
        }
    }
}


