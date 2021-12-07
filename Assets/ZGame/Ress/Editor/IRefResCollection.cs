using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.Info;

public interface IRefResCollection
{
    public List<AssetBundleBuild> GetResMap(GameObject ob);
    public List<CompInfo> GetCompInfo(GameObject obj);
}
