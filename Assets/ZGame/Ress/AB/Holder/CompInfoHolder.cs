using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.Info;

namespace ZGame.Ress.AB.Holder
{
    public class CompInfoHolder : MonoBehaviour
    {
        [CompInfoRefCollection("BuildInCompImageCollection")]
        public List<BuildInCompImageInfo> buildInCompImageInfos;

        [CompInfoRefCollection("BuildInCompSpriteRendererCollection")]
        public List<BuildInCompSpriteRendererInfo> buildInCompSpriteRendererInfos;

        [CompInfoRefCollection("BuildInCompTextCollection")]
        public List<BuildInCompTextInfo> buildInCompTextInfos;

        [CompInfoRefCollection("BuildInCompTextMeshProUGUICollection")]
        public List<BuildInCompTextMeshProUGUIInfo> buildInCompTextMeshProUGUIInfos;

        [CompInfoRefCollection("BuildInCompRendererCollection")]
        public List<BuildInCompRendererInfo> buildInCompRendererInfos;

        [CompInfoRefCollection("ExtCompImageSequenceCollection")]
        public List<ExtCompImageSequenceInfo> extCompImageSequenceInfos;

        [CompInfoRefCollection("ExtCompSpriteSequenceCollection")]
        public List<ExtCompSpriteSequenceInfo> extCompSpriteSequenceInfos;

        [CompInfoRefCollection("ExtCompMaterialTextureSequenceCollection")]
        public List<ExtCompMaterialTextureSequenceInfo> extCompMaterialTextureSequenceInfos;

        [CompInfoRefCollection("ExtCompSwapSpriteCollection")]
        public List<ExtCompSwapSpriteInfo> extCompSwapSpriteInfos;

        [CompInfoRefCollection("ExtCompSwitch2ButtonCollection")]
        public List<ExtCompSwitch2ButtonInfo> extCompSwitch2ButtonInfos;

       


        public LightmapInfo lightmapInfo;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class CompInfoRefCollectionAttribute : Attribute
    {
        public string refCollectionClassName;
        public CompInfoRefCollectionAttribute(string refCollectionName)
        {
            this.refCollectionClassName = refCollectionName;
        }
    }
}