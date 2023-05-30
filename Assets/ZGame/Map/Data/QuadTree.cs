using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
using System.IO;


public class LightMapInfo
{
    public string name;
    public int map_index;        //lightmapIndex
    public Vector4 map_offset;   //lightmapScaleOffset

    public LightMapInfo()
    {
        name = "";
        map_index = -1;
        map_offset = Vector4.zero;
    }
}

public class QuadCell
{
    public int index;
    public string resource;
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale;
    public Vector2 pos2;

    public List<LightMapInfo> lightMapInfos = new List<LightMapInfo>();

    //用于排序
    public float distance;


    public QuadCell()
    {
        this.index = 0;
        this.resource = "";
        this.pos = Vector3.zero;
        this.rot = Vector3.zero;
        this.scale = Vector3.zero;
        this.pos2 = Vector2.zero;
    }


    public QuadCell(Transform tf, string resource, int index)
    {
        this.index = index;
        this.resource = resource;
        this.pos = tf.position;
        this.rot = tf.eulerAngles;
        this.scale = tf.localScale;

        this.pos2 = new Vector2(this.pos.x,this.pos.z);

        MeshRenderer render = tf.gameObject.GetComponent<MeshRenderer>();
        if (render)
        {
            var info = new LightMapInfo();
            info.name = "";
            info.map_index = render.lightmapIndex;
            info.map_offset = render.lightmapScaleOffset;

            this.lightMapInfos.Add(info); 
        }

        MeshRenderer[] renders = tf.gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renders.Length; i++)
        {
            var info = new LightMapInfo();
            info.name = renders[i].gameObject.name;
            info.map_index = renders[i].lightmapIndex;
            info.map_offset = renders[i].lightmapScaleOffset;

            this.lightMapInfos.Add(info); 
        }

        
    }


     public void Write(BinaryWriter writer)
    {
        writer.Write(this.index);
        writer.Write(this.resource);
        writer.Write(this.pos.x);
        writer.Write(this.pos.y);
        writer.Write(this.pos.z);
        writer.Write(this.rot.x);
        writer.Write(this.rot.y);
        writer.Write(this.rot.z);
        writer.Write(this.scale.x);
        writer.Write(this.scale.y);
        writer.Write(this.scale.z);

        int light_count = this.lightMapInfos.Count;
        writer.Write(light_count);
        if (light_count > 0)
        {
            for (int i = 0; i < light_count; i++)
            {
                var info = this.lightMapInfos[i];
                writer.Write(info.name);
                writer.Write(info.map_index);
                writer.Write(info.map_offset.x);
                writer.Write(info.map_offset.y);
                writer.Write(info.map_offset.z);
                writer.Write(info.map_offset.w);
            }
        }
    }

    public void Read(BinaryReader reader)
    {
        this.index = reader.ReadInt32();
        this.resource = reader.ReadString();
        this.pos.x = reader.ReadSingle();
        this.pos.y = reader.ReadSingle();
        this.pos.z = reader.ReadSingle();
        this.pos2.x = this.pos.x;
        this.pos2.y = this.pos.z;
        
        this.rot.x = reader.ReadSingle();
        this.rot.y = reader.ReadSingle();
        this.rot.z = reader.ReadSingle();
        this.scale.x = reader.ReadSingle();
        this.scale.y = reader.ReadSingle();
        this.scale.z = reader.ReadSingle();

        int light_count = reader.ReadInt32();
        if (light_count > 0)
        {
            for (int i = 0; i < light_count; i++)
            {
                var info = new LightMapInfo();
                info.name = reader.ReadString();
                info.map_index = reader.ReadInt32();
                info.map_offset.x = reader.ReadSingle();
                info.map_offset.y = reader.ReadSingle();
                info.map_offset.z = reader.ReadSingle();
                info.map_offset.w = reader.ReadSingle();
                this.lightMapInfos.Add(info);
            }
        }


        
    }
}

public class QuadTree : IDisposable
{
    public Rect rect;
    public bool isLeaf;
    public int cur_layer;
    public int max_layer;
    public List<QuadCell> cells;
    public QuadTree[] childTrees;
   

    public QuadTree()
    {
        this.rect = Rect.zero;
        this.isLeaf = true;
    }

    public QuadTree(Rect rect,int cur,int max)
    {
        this.rect = rect;
        this.isLeaf = true;
        this.cur_layer = cur;
        this.max_layer = max;
#if UNITY_EDITOR
        DrawRect(Color.green);
#endif
    }

    public void AddCell(QuadCell cell)
    {
        if (isLeaf)
        {
            if (null == this.cells)
            {
                this.cells = new List<QuadCell>();
            }
            this.cells.Add(cell);
            if (this.cur_layer < this.max_layer)
            {
                this.isLeaf = false;

                for (int i = 0; i < this.cells.Count; i++)
                {
                    InsertCellToChild(this.cells[i]);
                }

                this.cells.Clear();
            }
        }
        else
        {
            InsertCellToChild(cell);
        }
    }

    void InsertCellToChild(QuadCell cell)
    {
        if (null == this.childTrees)
        {
            this.childTrees = new QuadTree[4];
            for (int i = 0; i < 4; i++)
            {
                var newRect = Rect.zero;
                newRect.width = this.rect.width * 0.5f;
                newRect.height = this.rect.height * 0.5f;
                newRect.x = this.rect.center.x + newRect.width * (i % 3 == 0 ? -1 : 0);
                newRect.y = this.rect.center.y + newRect.height * (i < 2 ? 0 : -1);
                this.childTrees[i] = new QuadTree(newRect,this.cur_layer+1,this.max_layer);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (this.childTrees[i].IsContainsPoint(cell.pos2))
            {
                this.childTrees[i].AddCell(cell);
                break;
            }
        }
    }


    public bool IsContainsPoint(Vector2 pos)
    {
        return this.rect.Contains(pos);
    }
    

    public bool IsOverlaps(Rect other)
    {
        return this.rect.Overlaps(other);
    }

    ///是否和目标圆形区域有重叠
    Vector2 c = Vector2.zero;
    Vector2 h = Vector2.zero;
    Vector2 v = Vector2.zero;
    Vector2 u = Vector2.zero;
    public bool IsOverlapsCircle(Vector2 p,float r)
    {
        h.x = rect.width * 0.5f;
        h.y = rect.height * 0.5f;
        c.x = rect.x + h.x;
        c.y = rect.y + h.y;
        v.x = Mathf.Abs(p.x - c.x);
        v.y = Mathf.Abs(p.y - c.y);
        u.x = Mathf.Max(v.x - h.x, 0);
        u.y = Mathf.Max(v.y - h.y, 0);
       
        // Vector2 c = rect.center;
        // Vector2 h = new Vector2(rect.width*0.5f,rect.height*0.5f);
        // Vector2 v = new Vector2(Mathf.Abs(p.x-c.x),Mathf.Abs(p.y-c.y));
        // Vector2 u = new Vector2(Mathf.Max(v.x-h.x,0),Mathf.Max(v.y-h.y,0));
        
        return Vector2.Dot(u,u) <= r*r;
    }

    public List<QuadCell> GetOverlapsCells(Rect other)
    {
        var over_cells = new List<QuadCell>();
        if(this.IsOverlaps(other))
        {
            if (this.isLeaf)
            {
                if(null != this.cells) over_cells.AddRange(this.cells);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    var childCells = this.childTrees[i].GetOverlapsCells(other);
                    over_cells.AddRange(childCells);
                }
            
            }
#if UNITY_EDITOR
            DrawRect(Color.red);
#endif
        }
        return over_cells;
    }


    public List<QuadCell> GetOverlapsCircleCells(Vector2 p,float r)
    {
        var over_cells = ListPool<QuadCell>.Get();
        if(this.IsOverlapsCircle(p,r))
        {
            if (this.isLeaf)
            {
                if(null != this.cells) over_cells.AddRange(this.cells);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    var childCells = this.childTrees[i].GetOverlapsCircleCells(p,r);
                    over_cells.AddRange(childCells);
                    ListPool<QuadCell>.Release(childCells);
                }
            
            }
#if UNITY_EDITOR
            DrawRect(Color.red);
#endif
        }
        return over_cells;
    }


    public void Dispose()
    {
        this.childTrees = null;
        this.cells.Clear();
        this.cells = null;
    }


    public void Write(BinaryWriter writer)
    {
        writer.Write(this.rect.x);
        writer.Write(this.rect.y);
        writer.Write(this.rect.width);
        writer.Write(this.rect.height);
        writer.Write(this.isLeaf);

        int cells_count = 0;
        if (null != this.cells)
        {
            cells_count = this.cells.Count;
        }

        writer.Write(cells_count);

        if (cells_count > 0)
        {
            for (int i = 0; i < cells_count; i++)
            {
                this.cells[i].Write(writer);
            }
        }

        int childCount = 0;
        if (null != childTrees)
        {
            childCount = 4;
        }

        writer.Write(childCount);
        for (int i = 0; i < childCount; i++)
        {
            this.childTrees[i].Write(writer);
        }
    }

    public void Read(BinaryReader reader)
    {
        this.rect.x = reader.ReadSingle();
        this.rect.y = reader.ReadSingle();
        this.rect.width = reader.ReadSingle();
        this.rect.height = reader.ReadSingle();
        this.isLeaf = reader.ReadBoolean();

        int cells_count = reader.ReadInt32();
        if (cells_count > 0)
        {
            this.cells = new List<QuadCell>();
            for (int i = 0; i < cells_count; i++)
            {
                var newCell = new QuadCell();
                newCell.Read(reader);
                this.cells.Add(newCell);
            }
        }

        int childCount = reader.ReadInt32();
        if (childCount > 0)
        {
            this.childTrees = new QuadTree[childCount];
            for (int i = 0; i < childCount; i++)
            {
                var newTree = new QuadTree();
                newTree.Read(reader);
                this.childTrees[i] = newTree;
            }
        }
#if UNITY_EDITOR
       DrawRect(Color.green);
#endif
    }

#if UNITY_EDITOR
    public void DrawRect(Color color)
    {
        // Vector3 v_c = this.rect.center;
        // float w = this.rect.width;
        // float h = this.rect.height;
        // Vector3[] pos = new Vector3[4];
        // pos[0] = new Vector3(v_c.x-w*0.5f,30,v_c.y-h*0.5f);
        // pos[1] = new Vector3(v_c.x-w*0.5f,30,v_c.y+h*0.5f);
        // pos[2] = new Vector3(v_c.x+w*0.5f,30,v_c.y+h*0.5f);
        // pos[3] = new Vector3(v_c.x+w*0.5f,30,v_c.y-h*0.5f);

        // Debug.DrawLine(pos[0],pos[1],color,10);
        // Debug.DrawLine(pos[1],pos[2],color,10);
        // Debug.DrawLine(pos[2],pos[3],color,10);
        // Debug.DrawLine(pos[3],pos[0],color,10);
    }
#endif
}
