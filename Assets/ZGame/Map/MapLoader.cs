using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MapLoader : SingletonMonoBehaviour<MapLoader>
{
    string map_name = "";
    float radius = 50;
    float check_distance = 1;
    
    MapController mapController;
    Vector2 cur_pos = Vector2.zero;
    Vector2 last_pos = Vector2.zero;
    Vector2 init_pos = Vector2.zero;
    

    //Camera mainCamera;
    Transform target;
    // Vector2 hExtend = new Vector2(-10,10);
    // Vector2 vExtend = new Vector2(-10,10);
    // float dExtend = 10;

    bool in_preload;
    Action preload_callback;
    List<CellController> preload_cells;

    bool is_dispose;





    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(string name,float radius = 50)
    {
        this.map_name = name;
        this.radius = radius;
        this.preload_cells = null;
        this.mapController = new MapController(map_name,radius,OnInit);
        this.is_dispose = false;

        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = radius - 10;
        RenderSettings.fogEndDistance = radius;
    }

    private void OnInit()
    {
        if (in_preload)
        {
            mapController.OnMove(init_pos);

            this.preload_cells = ListPool<CellController>.Get();
            var visiable_chunks = mapController.GetVisiableChunks();
            foreach (var item in visiable_chunks)
            {
                if (null != item)
                {
                    var cells = item.GetVisiableCells();
                    this.preload_cells.AddRange(cells);
                    ListPool<CellController>.Release(cells);
                }
            }
        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Dispose()
    {
        if (null != mapController)  
        {
            mapController.Dispose();
        }
        mapController = null;
        //target = null;
        is_dispose = true;
    }



    /// <summary>
    /// 设置镜头
    /// </summary>
    public void SetTarget(Transform trans)
    {
        target = trans;
    }

    /// <summary>
    /// 预加载
    /// </summary>
    public void PreLoad(Vector3 init_pos, Action callback)
    {
        this.preload_callback = callback;
        this.init_pos = new Vector2(init_pos.x,init_pos.z);
        this.in_preload = true;
    }


    // Update is called once per frame
    //void Update()
    void FixedUpdate()
    {
        if (!is_dispose)
        {
            if (target && null != mapController)
            {
                cur_pos.x = target.position.x;
                cur_pos.y = target.position.z;
                if (Vector2.Distance(cur_pos,last_pos) > check_distance)
                {
                    last_pos = cur_pos;
                    mapController.OnMove(cur_pos);
                }
            }

            //预加载进度
            if (in_preload && null != this.preload_cells)
            {
                int cur = 0;
                int total = this.preload_cells.Count;
                for (int i = 0; i < total; i++)
                {
                    if (this.preload_cells[i].IsLoadComplete())
                    {
                        cur++;
                    }
                }

                if (cur == total)
                {
                    in_preload = false;
                    ListPool<CellController>.Release(this.preload_cells);
                    this.preload_cells = null;

                    if ( null != this.preload_callback)
                    {
                        this.preload_callback.Invoke();
                    }
                    this.preload_callback = null;
                }
            }
        }
    }


    /// <summary>
    /// 是否处于视锥体视野范围内
    /// </summary>
    // public bool IsInView(Vector3 pos)
    // {
    //     if (Vector3.Dot((pos - mainCamera.transform.position),mainCamera.transform.forward) > 0)
    //     {
    //         Vector3 w_pos = mainCamera.WorldToScreenPoint(pos + mainCamera.transform.forward * dExtend);
    //         if (w_pos.x > hExtend.x && w_pos.x < Screen.width + hExtend.y &&
    //             w_pos.y > vExtend.x && w_pos.y < Screen.height + vExtend.y)
    //         {
    //             return true;
    //         } 
    //     }
    //     return false;
    //     return true;
    // }

#if UNITY_EDITOR
    Vector3 pos = Vector3.zero;
    Vector3 size = Vector3.zero;
    void OnDrawGizmos()
    {
        if (null != mapController)
        {
            var mapData = mapController.GetData();
            if (null != mapData)
            {
                Gizmos.color = Color.black;
                foreach (var item in mapData.chunkDatas)
                {
                    pos.x = item.rect.center.x;
                    pos.y = 0;
                    pos.z = item.rect.center.y;

                    size.x = item.rect.width;
                    size.y = 100;
                    size.z = item.rect.height;
                    Gizmos.DrawWireCube(pos, size);
                }

                var cur_chunk_index = mapController.GetCurChunkIndex();
                var visiable_chunks = mapController.GetVisiableChunks();
                foreach (var item in visiable_chunks)
                {
                    if (null != item)
                    {
                        var chunk_data = item.GetData();
                        Gizmos.color = chunk_data.index == cur_chunk_index ? Color.green : Color.yellow;

                        pos.x = chunk_data.rect.center.x;
                        pos.y = 0;
                        pos.z = chunk_data.rect.center.y;

                        size.x = chunk_data.rect.width-1;
                        size.y = 100;
                        size.z = chunk_data.rect.height-1;
                        Gizmos.DrawWireCube(pos, size);
                    }
                }
                ListPool<ChunkController>.Release(visiable_chunks);
            }
        }

        if (target)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(target.position, radius);
        }
    }
#endif
}
