using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapData
{
   public List<ChunkData> chunkDatas = new List<ChunkData>();
   public List<Vector3> pathPoints = new List<Vector3>();


    public void Write(BinaryWriter writer)
    {
        int count = this.chunkDatas.Count;
        writer.Write(count);
        if(count>0)
        {
            for (int i = 0; i < count; i++)
            {
                this.chunkDatas[i].Write(writer);
            }
        }

        count = this.pathPoints.Count;
        writer.Write(count);
        if(count>0)
        {
            for (int i = 0; i < count; i++)
            {
                var pos = this.pathPoints[i];
                writer.Write(pos.x);
                writer.Write(pos.y);
                writer.Write(pos.z);
            }
        }
    }

    public void Read(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        if(count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                var chunk = new ChunkData();
                chunk.Read(reader);
                this.chunkDatas.Add(chunk);
            }
        }
       
       
       count = reader.ReadInt32();
       if(count > 0)
       {
            for (int i = 0; i < count; i++)
            {
                var pos = new Vector3();
                pos.x = reader.ReadSingle();
                pos.y = reader.ReadSingle();
                pos.z = reader.ReadSingle();
                this.pathPoints.Add(pos);
            }
       }
        
    }

   
}
