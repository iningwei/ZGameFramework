using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum EChunkNear
{
    Left,
    LeftTop,
    Top,
    RightTop,
    Right,
    RightBottom,
    Bottom,
    LefBottom,
    Center,
}

public class ChunkData
{
    public int index;
    public int[] near_indexs = new int[8]{-1,-1,-1,-1,-1,-1,-1,-1};
    public Rect rect;
    public List<string> lm_col = new List<string>();
    public List<string> lm_dir = new List<string>();
    public QuadTree partTree;
    public int max_layer;


    public ChunkData()
    {
        
    }

    public ChunkData(int index, Rect rect, int max_layer)
    {
        this.max_layer = max_layer;
        this.index = index;
        this.rect = rect;
        this.partTree = new QuadTree(rect,1,max_layer);
    }

    public void SetNear(EChunkNear dir, int index)
    {
        near_indexs[(int)dir] = index;
    }

    public void SaveLightMap(List<string> col, List<string> dir)
    {
       this.lm_col = col;
       this.lm_dir = dir;
    }


    public bool IsContainsPoint(Vector2 pos)
    {
        return this.rect.Contains(pos);
    }
    

    public void AddPart(Transform ts,string resource,int index)
    {
        partTree.AddCell(new QuadCell(ts, resource, index));
    }


     public void Write(BinaryWriter writer)
    {
       writer.Write(this.index);
       for (int i = 0; i < 8; i++)
       {
            writer.Write(this.near_indexs[i]);
       }
       writer.Write(this.rect.x);
       writer.Write(this.rect.y);
       writer.Write(this.rect.width);
       writer.Write(this.rect.height);

       int count = this.lm_col.Count;
       writer.Write(count);
       if(count>0)
       {
            for (int i = 0; i < count; i++)
            {
                writer.Write(this.lm_col[i]);
            }
       }
       

        count = this.lm_dir.Count;
        writer.Write(count);
        if(count>0)
        {
            for (int i = 0; i < count; i++)
            {
                writer.Write(this.lm_dir[i]);
            }
        }

       partTree.Write(writer);
    }

    public void Read(BinaryReader reader)
    {
        this.index = reader.ReadInt32();
        for (int i = 0; i < 8; i++)
        {
                this.near_indexs[i] = reader.ReadInt32();
        }
        this.rect.x = reader.ReadSingle();
        this.rect.y = reader.ReadSingle();
        this.rect.width = reader.ReadSingle();
        this.rect.height = reader.ReadSingle();

        int count = reader.ReadInt32();
        if (count>0)
        {
            for (int i = 0; i < count; i++)
            {
                    this.lm_col.Add(reader.ReadString());
            }
        }
        
        count = reader.ReadInt32();
        if (count>0)
        {
            for (int i = 0; i < count; i++)
            {
                this.lm_dir.Add(reader.ReadString());
            }
        }
        

        this.partTree = new QuadTree();
        partTree.Read(reader);
    }
}
