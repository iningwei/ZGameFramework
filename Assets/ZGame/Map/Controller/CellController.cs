using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using ZGame.Ress;
using ZGame.Ress.AB;

public class CellController:IDisposable
{
    private QuadCell cellData;
    private GameObject part;
    private Transform ts_parent;
    private bool visiable;
    private int lm_offset_index;

    private Dictionary<string,MeshRenderer> dic_render;




    public CellController(QuadCell cell,Transform parent,int lm_offset_index)
    {
        this.cellData = cell;
        this.ts_parent = parent;
        this.lm_offset_index = lm_offset_index;
    }

    public void SetVisiable(bool visiable)
    {
        if (visiable != this.visiable)
        {
            this.visiable = visiable;

            if (string.IsNullOrEmpty(cellData.resource))
            {
                return;
            }

            if (visiable)
            {
                ABManager.Instance.LoadOtherPrefab(cellData.resource,OnLoadComplete,false,false);
            }
            else
            {
                ABManager.Instance.ReleasePrefab(cellData.resource,this.part);
                this.part = null;

                if (null != this.dic_render)
                {
                    DictionaryPool<string,MeshRenderer>.Release(this.dic_render);
                    this.dic_render = null;
                }
            }
        }
    }

    public bool GetVisiable()
    {
        return this.visiable;
    }

    private void OnLoadComplete(UnityEngine.Object obj)
    {
        if (obj)
        {
            this.part = obj as GameObject;
            this.part.isStatic = true;
            this.part.transform.SetParent(this.ts_parent);
            this.part.transform.position = cellData.pos;
            this.part.transform.localScale = cellData.scale;
            this.part.transform.rotation = Quaternion.Euler(cellData.rot);

            if (this.visiable)
            {
                this.part.SetActive(true);
                SetPartLightmap(lm_offset_index);
            }
            else
            {
                ABManager.Instance.ReleasePrefab(cellData.resource,this.part);
                this.part = null;
            }
        }
        else
        {
            Debug.LogWarning("load part failed: " + this.cellData.resource);
        }
    }

    public bool IsLoadComplete()
    {
        return null != this.part; 
    }

    public void SetPartLightmap(int offset)
    {
        this.lm_offset_index = offset;

        if (part)
        {
            if (null == dic_render)
            {
                //var shader = Shader.Find("My/Standard");
                dic_render = DictionaryPool<string,MeshRenderer>.Get();

                MeshRenderer render = part.GetComponent<MeshRenderer>();
                if (render)
                {
                    dic_render.Add("",render);

                    if (SystemInfo.supportsInstancing)
                    {
                        foreach (var mat in render.sharedMaterials)
                        {
                            //mat.shader = shader;
                            mat.enableInstancing = true;
                        }
                    }
                    render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                MeshRenderer[] renders = part.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < renders.Length; i++)
                {
                    dic_render.Add(renders[i].gameObject.name,renders[i]);

                    if (SystemInfo.supportsInstancing)
                    {
                        foreach (var mat in renders[i].sharedMaterials)
                        {
                            //mat.shader = shader;
                            mat.enableInstancing = true;
                        }
                    }

                    renders[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }

            foreach (var item in cellData.lightMapInfos)
            {
                if (dic_render.ContainsKey(item.name))
                {
                    var render = dic_render[item.name];
                    render.lightmapIndex = offset + item.map_index;
                    render.lightmapScaleOffset = item.map_offset;
                }
            }
        }
    }

    public QuadCell GetData()
    {
        return cellData;
    }


    public void Dispose()
    {
        if (this.part)
        {
            ABManager.Instance.ReleasePrefab(cellData.resource,this.part);
            this.part = null;
        }
    }
}
