using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress.AB
{
    public enum ABType
    {
        /// <summary>
        /// 精灵
        /// </summary>
        [ABTypeDes("sprite_")]
        Sprite = 1,

        /// <summary>
        /// 纯图片
        /// </summary>
        [ABTypeDes("tex_")]
        Texture,

        /// <summary>
        /// 特效 预制件
        /// </summary>
        [ABTypeDes("eff_")]
        Effect,

        /// <summary>
        /// 窗体 预制件
        /// </summary>
        [ABTypeDes("w_")]
        Window,

        /// <summary>
        /// 其它 预制件
        /// </summary>
        [ABTypeDes("op_")]
        OtherPrefab,

        /// <summary>
        /// 场景（unity3d场景）
        /// </summary>
        [ABTypeDes("s_")]
        Scene,

        /// <summary>
        /// 音效
        /// </summary>
        [ABTypeDes("a_")]
        Audio,

        /// <summary>
        /// 视频
        /// </summary>
        [ABTypeDes("v_")]
        Video,


        /// <summary>
        /// 模型
        /// </summary>
        [ABTypeDes("m_")]
        Model,

        /// <summary>
        /// 材质球
        /// </summary>
        [ABTypeDes("mat_")]
        Material,
    }
}