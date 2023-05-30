using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Pool;
using ZGame.Ress.AB;

public class MapController : IDisposable
{
    private string mapName;
    private MapData mapData;
    private Dictionary<int, ChunkController> chunkControllers = new Dictionary<int, ChunkController>();
    private float radius;
    private List<int> visiableIndexs = new List<int>();
    private int curIndex = -1;
    private Vector2 curPos;
    private Action initCallback;

    public MapController(string name, float radius, Action initCallback)
    {
        this.mapName = name;
        this.radius = radius;
        this.initCallback = initCallback;

        ABManager.Instance.LoadByte(name + "_map", OnDataLoadComplete, false);
    }

    private void OnDataLoadComplete(TextAsset asset)
    {
        var ms = new MemoryStream(asset.bytes);
        BinaryReader reader = new BinaryReader(ms);

        mapData = new MapData();
        mapData.Read(reader);

        reader.Close();
        ms.Close();

        foreach (var data in mapData.chunkDatas)
        {
            this.chunkControllers.Add(data.index, new ChunkController(data, mapName));
        }

        if (null != this.initCallback)
        {
            this.initCallback.Invoke();
        }
    }


    public void OnMove(Vector2 pos)
    {
        curPos = pos;
        var index = GetChunkIndex(pos);
        if (index > -1 && index != this.curIndex)
        {
            curIndex = index;

            visiableIndexs.Clear();
            visiableIndexs.Add(index);
            chunkControllers[index].LoadLightmap(ChangeLightmap);

            var nears = chunkControllers[index].GetNearIndexs();
            foreach (var item in nears)
            {
                if (item > -1 && null != chunkControllers[item])
                {
                    visiableIndexs.Add(item);
                    chunkControllers[item].LoadLightmap(ChangeLightmap);
                }
            }
        }

        UpdateVisiableCell();

    }

    public MapData GetData()
    {
        return mapData;
    }

    public int GetCurChunkIndex()
    {
        return curIndex;
    }

    public List<ChunkController> GetVisiableChunks()
    {
        List<ChunkController> list_chunks = ListPool<ChunkController>.Get();
        foreach (var item in visiableIndexs)
        {
            list_chunks.Add(chunkControllers[item]);
        }
        return list_chunks;
    }


    void ChangeLightmap()
    {
        bool load_complete = true;
        foreach (var item in visiableIndexs)
        {
            load_complete = load_complete && chunkControllers[item].lm_load_complete;
        }

        if (load_complete)
        {
            //跟换场景的光照贴图
            List<LightmapData> list_lm = ListPool<LightmapData>.Get();
            foreach (var item in visiableIndexs)
            {
                chunkControllers[item].GetLightmap(list_lm);
                chunkControllers[item].SetLightmap();
            }
            LightmapSettings.lightmaps = list_lm.ToArray();
            ListPool<LightmapData>.Release(list_lm);

            // foreach (var item in visiableIndexs)
            // {
            //     chunkControllers[item].SetLightmap();
            // }
        }

        UpdateVisiableCell();
    }


    int GetChunkIndex(Vector2 pos)
    {
        int index = -1;
        if (mapData != null && mapData.chunkDatas != null)
        {
            foreach (var item in mapData.chunkDatas)
            {
                if (item.IsContainsPoint(pos))
                {
                    index = item.index;
                    break;
                }
            }
        }


        return index;
    }


    void UpdateVisiableCell()
    {
        foreach (var index in chunkControllers.Keys)
        {
            chunkControllers[index].UpdateVisiableCell(curPos, radius);
        }
    }

    public void Dispose()
    {
        foreach (var index in chunkControllers.Keys)
        {
            chunkControllers[index].Dispose();
        }
        chunkControllers.Clear();
    }
}
