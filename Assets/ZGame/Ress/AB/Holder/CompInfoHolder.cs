using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.Info;

namespace ZGame.Ress.AB.Holder
{
    public class CompInfoHolder : MonoBehaviour
    {
        

        public List<BuildInCompImageInfo> buildInCompImageInfos;
        public List<BuildInCompSpriteRendererInfo> buildInCompSpriteRendererInfos;
        public List<BuildInCompTextInfo> buildInCompTextInfos;
        public List<BuildInCompRendererInfo> buildInCompRendererInfos;
        
        public List<ExtCompImageSequenceInfo> extCompImageSequenceInfos;
        public List<ExtCompSpriteSequenceInfo> extCompSpriteSequenceInfos;

        public List<ExtCompSwapSpriteInfo> extCompSwapSpriteInfos;
        public List<ExtCompBoxingScenePetSettingInfo> extCompBoxingScenePetSettingInfos;
    }
}