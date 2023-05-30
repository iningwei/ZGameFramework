using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using ZGame.Ress.AB;

public class ChunkController:IDisposable
{
    private ChunkData chunkData;
    private string map_name;
    private int lm_start_index = 0;
    private List<LightmapData> lm_datas;
    private UnityAction load_lm_callback;
    private Dictionary<int,CellController> dic_cells = new Dictionary<int,CellController>();
    private GameObject root;

    private int load_count;
    public bool lm_load_complete;



    public ChunkController(ChunkData data,string map_name)
    {
        this.chunkData = data;
        this.map_name = map_name;

        var center = data.rect.center;

        this.root = new GameObject("chunk_"+data.index);
        this.root.transform.position = new Vector3(center.x,0,center.y);
        this.root.transform.localScale = Vector3.one;
        this.root.transform.rotation = Quaternion.Euler(Vector3.zero);
    }


    public int[] GetNearIndexs()
    {
        return chunkData.near_indexs;
    }

    public void LoadLightmap(UnityAction call_back)
    {
        if (null != lm_datas)
        {
            lm_load_complete = true;
            if (null != call_back)
            {
                call_back();
            }
        }
        else
        {
            lm_datas = new List<LightmapData>();
            lm_load_complete = false;
            load_lm_callback = call_back;
            load_count = 0;

            int lm_count = chunkData.lm_col.Count;
            for (int i = 0; i < lm_count; i++)
            {
                LightmapData lm_data = new LightmapData();
                if (!string.IsNullOrEmpty(chunkData.lm_col[i]))
                {
                    Debug.Log("----chunk:"+this.chunkData.index+" lm:" +chunkData.lm_col[i]);
                    load_count++;
                    ABManager.Instance.LoadTexture(chunkData.lm_col[i],tex=>
                    {
                        lm_data.lightmapColor = tex as Texture2D;
                        lm_load_complete = --load_count == 0;
                        if (lm_load_complete && null != load_lm_callback)
                        {
                            load_lm_callback();
                        }
                    },true);
                }

                if (!string.IsNullOrEmpty(chunkData.lm_dir[i]))
                {
                    load_count++;
                    ABManager.Instance.LoadTexture(chunkData.lm_dir[i],tex=>
                    { 
                        lm_data.lightmapDir = tex as Texture2D;
                        lm_load_complete = --load_count == 0;
                        if (lm_load_complete && null != load_lm_callback)
                        {
                            load_lm_callback();
                        }
                    },true);
                }
                
                
                lm_datas.Add(lm_data);
            }
        }
    }

    // private void OnLoadLightmapComplete(AssetBundle ab)
    // {
    //     lm_datas = new List<LightmapData>();

    //     if (ab)
    //     {
    //         int lm_count = chunkData.lm_col.Count;
    //         for (int i = 0; i < lm_count; i++)
    //         {
    //             LightmapData lm_data = new LightmapData();
    //             if (!string.IsNullOrEmpty(chunkData.lm_col[i]))
    //             {
    //                 lm_data.lightmapColor = ab.LoadAsset<Texture2D>(chunkData.lm_col[i]);
    //             }

    //             if (!string.IsNullOrEmpty(chunkData.lm_dir[i]))
    //             {
    //                 lm_data.lightmapDir = ab.LoadAsset<Texture2D>(chunkData.lm_dir[i]);
    //             }
                
                
    //             lm_datas.Add(lm_data);
    //         }
    //     }
        
    //     lm_load_complete = true;
    //     if (null != load_lm_callback)
    //     {
    //         load_lm_callback();
    //     }
    // }

    public void GetLightmap(List<LightmapData> list_lms)
    {
        lm_start_index = list_lms.Count;
        list_lms.AddRange(lm_datas);
    }

    public void SetLightmap()
    {
        foreach (int index in dic_cells.Keys)
        {
            dic_cells[index].SetPartLightmap(lm_start_index);
        }
    }

    public void UpdateVisiableCell(Vector2 pos, float radius)
    {
        var list_cells = chunkData.partTree.GetOverlapsCircleCells(pos,radius);
        int count = list_cells.Count;
        for (int i = 0; i < count; i++)
        {
            list_cells[i].distance = Vector2.Distance(pos,list_cells[i].pos2);
        }
        list_cells.Sort((a,b)=>{return a.distance.CompareTo(b.distance);});


        var dic_visiable = DictionaryPool<int,bool>.Get();
        for (int i = 0; i < count; i++)
        {
            var item = list_cells[i];
            // if (MapLoader.Instance.IsInView(item.pos))
            // {
                CellController cell = null;
                if (dic_cells.ContainsKey(item.index))
                {
                    cell = dic_cells[item.index];
                }
                else
                {
                    cell = new CellController(item,root.transform, lm_start_index);
                    dic_cells[item.index] = cell;
                }

                cell.SetVisiable(true);
                dic_visiable[item.index] = true;
            //}
        }
        // foreach (var item in list_cells)
        // {
        //     if (CameraManager.Instance.IsInView(item.pos))
        //     {
        //         CellController cell = null;
        //         if (dic_cells.ContainsKey(item.index))
        //         {
        //             cell = dic_cells[item.index];
        //         }
        //         else
        //         {
        //             cell = new CellController(item,root.transform, lm_start_index);
        //             dic_cells[item.index] = cell;
        //         }
    
        //         dic_visiable[item.index] = true;
        //     }
        // }

        foreach (int index in dic_cells.Keys)
        {
            dic_cells[index].SetVisiable(dic_visiable.ContainsKey(index));
        }

        ListPool<QuadCell>.Release(list_cells);
        DictionaryPool<int,bool>.Release(dic_visiable);

        //Debug.Log("index:"+this.chunkData.index+"---dic_visiable:"+dic_visiable.Count);
    }

     public List<CellController> GetVisiableCells()
     {
        var list_cells = ListPool<CellController>.Get();
        foreach (int index in dic_cells.Keys)
        {
            if (dic_cells[index].GetVisiable())
            {
                list_cells.Add(dic_cells[index]);
            };
        }
        return list_cells;
     }

    public ChunkData GetData()
    {
        return chunkData;
    }


     public void Dispose()
     {
        foreach (var index in dic_cells.Keys)
        {
            dic_cells[index].Dispose();
        }

        dic_cells.Clear();
     }
}
